using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DatabaseExtension.SQLite.Demo.Models;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;

namespace DatabaseExtension.SQLite.Demo.ViewModels {
    public class MainWindowViewModel : ViewModel {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        public void Initialize() {
        }
        public MainWindowViewModel() : base() {
            this.Parameter = new UserSearchParameter();
            this.Users = new DispatcherCollection<User>(DispatcherHelper.UIDispatcher);
            this.Users.CollectionChanged += (sender, e) => {
                RaisePropertyChanged(nameof(Users));
            };
        }

        private UserSearchParameter _Parameter;

        public UserSearchParameter Parameter {
            get {
                return _Parameter;
            }
            private set { 
                if (_Parameter == value) {
                    return;
                }
                _Parameter = value;
                RaisePropertyChanged(nameof(Parameter));
            }
        }


        private ViewModelCommand _SearchCommand;

        public ViewModelCommand SearchCommand {
            get {
                if (_SearchCommand == null) {
                    _SearchCommand = new ViewModelCommand(Search);
                }
                return _SearchCommand;
            }
        }

        public void Search() {
            this.Users.Clear();
            foreach (var user in User.GetAll(this.Parameter)) {
                this.Users.Add(user);
            }
        }



        public DispatcherCollection<User> Users {
            get;
        }


        private ListenerCommand<User> _UpdateCommand;

        public ListenerCommand<User> UpdateCommand {
            get {
                if (_UpdateCommand == null) {
                    _UpdateCommand = new ListenerCommand<User>(Update);
                }
                return _UpdateCommand;
            }
        }

        public void Update(User parameter) {
            parameter.Update();
        }


        private ListenerCommand<User> _DeleteCommand;

        public ListenerCommand<User> DeleteCommand {
            get {
                if (_DeleteCommand == null) {
                    _DeleteCommand = new ListenerCommand<User>(Delete, CanDelete);
                }
                return _DeleteCommand;
            }
        }

        public bool CanDelete() {
            return true;
        }

        public void Delete(User parameter) {
            parameter.Delete();
            this.Users.Remove(parameter);
        }

    }
}

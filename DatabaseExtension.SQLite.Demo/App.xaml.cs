using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Livet;

namespace DatabaseExtension.SQLite.Demo {
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            DispatcherHelper.UIDispatcher = Dispatcher;
            SQLiteQuery.SetConnectionString($"Data Source={this.SQLiteDbPath};");
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private string SQLiteDbPath {
            get {
                return System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "users.db"
                    );
            }
        }
        // Application level error handling
        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    //TODO: Logging
        //    MessageBox.Show(
        //        "Something errors were occurred.",
        //        "Error",
        //        MessageBoxButton.OK,
        //        MessageBoxImage.Error);
        //
        //    Environment.Exit(1);
        //}
    }
}

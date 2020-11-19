using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace DatabaseExtension.SQLite.Demo.Models {
    public class User  {
        public static IEnumerable<User> GetAll(UserSearchParameter p) {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine(" cd");
            sb.AppendLine(",name");
            sb.AppendLine(",real_name");
            sb.AppendLine(",age");
            sb.AppendLine(",height");
            sb.AppendLine(",weight");
            sb.AppendLine(",blood_type");
            sb.AppendLine(",race");
            sb.AppendLine(",guild_name");
            sb.AppendLine(",hobby");
            sb.AppendLine(",character_voice");
            sb.AppendLine(",birth_month");
            sb.AppendLine(",birth_day");
            sb.AppendLine("FROM");
            sb.AppendLine(" users");
            var param = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(p.QueryString)) {
                sb.AppendLine("WHERE");
                sb.AppendLine("(");
                sb.AppendLine("    name LIKE @NAME");
                sb.AppendLine("    OR real_name LIKE @NAME");
                sb.AppendLine(")");
                param.Add("NAME", $"%{p.QueryString}%");
            }
            return new SQLiteQuery().GetSqlResult(sb.ToString(), param).Rows.Select(x => x.Create<User>());
        }
        [DbColumn("cd")]
        public int Cd {
            get;
            private set;
        }
        [DbColumn("name")]
        public string Name {
            get;
            set;
        }
        [DbColumn("real_name")]
        public string RealName {
            get;
            set;
        }
        [DbColumn("age")]
        public int? Age {
            get;
            set;
        }
        [DbColumn("height")]
        public int? Height {
            get;
            set;
        }
        [DbColumn("weight")]
        public int? Weight {
            get;
            set;
        }
        [DbColumn("blood_type")]
        public string BloodType {
            get;
            set;
        }
        [DbColumn("race")]
        public string Race {
            get;
            set;
        }
        [DbColumn("guild_name")]
        public string GuildName {
            get;
            set;
        }

        internal void Update() {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("UPDATE users SET");
            sb.AppendLine(" name = @name");
            sb.AppendLine(",real_name = @real_name");
            sb.AppendLine(",age = @age");
            sb.AppendLine(",height = @height");
            sb.AppendLine(",weight = @weight");
            sb.AppendLine(",blood_type = @blood_type");
            sb.AppendLine(",race = @race");
            sb.AppendLine(",guild_name = @guild_name");
            sb.AppendLine(",hobby = @hobby");
            sb.AppendLine(",character_voice = @character_voice");
            sb.AppendLine(",birth_month = @birth_month");
            sb.AppendLine(",birth_day = @birth_day");
            sb.AppendLine("WHERE");
            sb.AppendLine("cd = @cd");
            var param = new Dictionary<string, object> {
                { "name",this.Name },
                { "real_name",this.RealName },
                { "age",this.Age },
                {"height",this.Height },
                {"weight",this.Weight },
                {"blood_type",this.BloodType },
                {"race",this.Race },
                {"guild_name",this.GuildName},
                {"hobby",this.Hobby },
                {"character_voice",this.CV },
                {"birth_month",this.BirthMonth },
                {"birth_day",this.BirthDay }
            };
            using(var q=new SQLiteQuery())
            using(var trans = q.BeginTransaction()) {
                try {
                    q.ExecuteNonQuery(sb.ToString(), param);
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        internal void Delete() {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("DELETE FROM users");
            sb.AppendLine("WHERE");
            sb.AppendLine("cd = @CD");
            
            using (var q=new SQLiteQuery())
            using (var trans = q.BeginTransaction()) {
                try {
                    q.ExecuteNonQuery(sb.ToString(), new Dictionary<string, object> { { "cd", this.Cd } });
                    trans.Commit();
                } catch  {
                    trans.Rollback();
                    throw;
                }
            }
        }

        [DbColumn("hobby")]
        public string Hobby {
            get;
            set;
        }
        [DbColumn("character_voice")]
        public string CV {
            get;
            set;
        }
        [DbColumn("birth_month")]
        public int? BirthMonth {
            get;
            set;
        }
        [DbColumn("birth_day")]
        public int? BirthDay {
            get;
            set;
        }
        public string Birthday {
            get {
                if(this.BirthMonth.HasValue && this.BirthDay.HasValue) {
                    return $"{this.BirthMonth:00}/{this.BirthDay:00}";
                } else {
                    return "";
                }
            }
        }
       
    }
}

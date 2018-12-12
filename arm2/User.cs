using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace arm2
{
    class User
    {
        private DB db = null;
        public int ID = 0;
        public string Name = "";
        public string Login = "";
        public bool IsAdmin = false;
        public DateTime Created = new DateTime();
        public DateTime Updated = new DateTime();
        public DateTime Deleted = new DateTime();
        public string[] Header = new string[] { "ID", "Имя", "Логин", "Пароль", "Админ?", "Создан", "Изменени", "Удален" };
        public User(int id = -1)
        {
            db = new DB();
            if (id > 0)
            {
                User[] u = Where(id: id);
                if (u.Length > 0)
                {
                    ID = u[0].ID;
                    Name = u[0].Name;
                    Login = u[0].Login;
                    IsAdmin = u[0].IsAdmin;
                    Created = u[0].Created;
                    Updated = u[0].Updated;
                    Deleted = u[0].Deleted;
                }
            }
        }
        public User[] Where(int id = -1, string name = null, string login = null, bool is_admin = true, string password = null, bool active = true)
        {
            string sql = "select * from users ";
            string and = " where ";
            if (id > 0)
            {
                sql += and + "uid=" + id.ToString();
                and = " and ";
            }
            if (name != null)
            {
                sql += and + "uname='" + name + "'";
                and = " and ";
            }
            if (login != null)
            {
                sql += and + "ulogin='" + login + "'";
                and = " and ";
            }
            if (is_admin == false)
            {
                sql += and + "is_admin=0";
                and = " and ";
            }
            if (active == false)
            {
                sql += and + "deleted_at is not null";
                and = " and ";
            }
            else
            {
                sql += and + "deleted_at is null";
                and = " and ";
            }
            if (password != null)
            {
                sql += and + "upassword='" + db.toMD5(password) + "'";
                and = " and ";
            }
            string[,] arr = db.Select(sql);
            User[] uarr = new User[arr.GetLength(0)];
            for(int i = 0; i < uarr.Length; i++)
            {
                uarr[i] = new User();
                uarr[i].ID = Convert.ToInt32(arr[i, 0]);
                uarr[i].Name = arr[i, 1];
                uarr[i].Login = arr[i, 2];
                uarr[i].IsAdmin = arr[i, 2] == "1";
                uarr[i].Created = db.StrToDate(arr[i, 5]);
                uarr[i].Updated = db.StrToDate(arr[i, 6]);
                uarr[i].Deleted = db.StrToDate(arr[i, 7]);
            }
            return uarr;
        }
        public bool Save()
        {
            return db.Update("users",new Dictionary<string, string> {
                { "uname",Name },
                { "ulogin",Login},
                { "isadmin",IsAdmin?"1":"0"},
                { "deleted_at",db.DateToStr(Deleted)}
            }, new Dictionary<string, string> {
                { "uid", ID.ToString() }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.Globalization;

namespace arm2
{
    class DB
    {
        private MySqlConnection connection = null;
        private string server = "";
        private string port = "";
        private string datebase = "";
        private string login = "";
        private string password = "";
        public DB()
        {
            server = ConfigurationManager.AppSettings["server"];
            port = ConfigurationManager.AppSettings.Get("port");
            datebase = ConfigurationManager.AppSettings.Get("datebase");
            login = ConfigurationManager.AppSettings.Get("login");
            password = ConfigurationManager.AppSettings.Get("password");
            connection = new MySqlConnection("Server="+ server+";Database="+ datebase + ";port="+ port + ";User Id="+ login + ";password="+ password);
        }
        public string[,] Select(string sql)
        {
           
            string[,] arr = new string[0,0];
            connection.Open();
            try
            {
                List<List<String>> list = new List<List<String>>();
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                int rows = 0;
                int colls = 0;
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        List<String> row = new List<String>();
                        for (int i = 0; i < reader.FieldCount; i++) {
                            row.Add(reader.GetValue(i).ToString());
                        }
                        list.Add(row);
                        rows++;
                        colls = reader.FieldCount;
                    }
                }
                arr = new string[rows, colls];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < colls; j++)
                    {
                        arr[i, j] = list[i][j];
                    }
                }
                connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                arr = new string[0, 0];
            }
            connection.Close();
            return arr;
        }
        public int Insert(string table,string[] fields, string[] values)
        {
            int res = 0;
            connection.Open();
            try
            {
                for(int i=0;i<values.Length;i++)
                {
                    values[i] = "'" + values[i] + "'";
                }
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO " + table + "(" + string.Join(", ", fields) + ") VALUES(" + string.Join(", ", values) + ")";
                cmd.ExecuteNonQuery();
                res = (int)(cmd.LastInsertedId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                res = 0;
            }
            connection.Close();
            return res;
        }
        public bool Update(string table, Dictionary<string,string> data, Dictionary<string, string> where)
        {
            bool res = false;
            connection.Open();
            try
            {
                string sql = "update " + table + " set ";
                bool first = true;
                foreach (var d in data)
                {
                    sql += (first ? "" : ", ") + d.Key + "='" + d.Value + "'";
                    first = false;
                }
                first = true;
                foreach (var w in where)
                {
                    sql += (first ? " where " : " and ") + w.Key + "='" + w.Value + "' ";
                    first = false;
                }
                MessageBox.Show(sql);
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                res = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                res = false;
            }
            connection.Close();
            return res;
        }
        public string toMD5(string s = "")
        {
            byte[] hash = Encoding.ASCII.GetBytes(s);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashenc = md5.ComputeHash(hash);
            string result = "";
            foreach (var b in hashenc)
            {
                result += b.ToString("x2");
            }
            return result;
        }
        public DateTime StrToDate(string s)
        {
            DateTime v;
            DateTime.TryParseExact(s, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,out v);
            return v;
        }
        public string DateToStr(DateTime d)
        {
            return d==null ? "null" : "STR_TO_DATE('" + d.ToString("dd.MM.yyyy hh:mm:ss") + "', '%d.%m.%Y %h:%i:%s')";
        }
    }
}

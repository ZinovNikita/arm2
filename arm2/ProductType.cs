using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace arm2
{
    public class ProductType
    {
        private DB db = null;
        public int ID = 0;
        public string Name = "";
        public string Unit = "";
        public float Cost = 0f;
        public DateTime Created = new DateTime();
        public DateTime Updated = new DateTime();
        public DateTime Deleted = new DateTime();
        public string[] Header = new string[] { "ID", "Название", "Ед.измерения", "Кол-во", "Создан", "Изменени", "Удален" };
        public ProductType(int id = -1)
        {
            db = new DB();
            if (id > 0)
            {
                string[,] arr = Where(id: id);
                if (arr.GetLength(0)>0)
                {
                    ID = Convert.ToInt32(arr[0, 0]);
                    Name = arr[0, 1];
                    Unit = arr[0, 2];
                    Cost = (float)Convert.ToDouble(arr[0, 3]);
                    Created = db.StrToDate(arr[0, 4]);
                    Updated = db.StrToDate(arr[0, 5]);
                    Deleted = db.StrToDate(arr[0, 6]);
                }
            }
        }
        public string[,] Where(
            int id = -1,
            string name = null,
            string unit = null,
            float cost = -1f,
            DateTime created = new DateTime(),
            DateTime updated = new DateTime(),
            DateTime deleted = new DateTime()
        ){
            string sql = "select * from product_types ";
            string and = " where ";
            if (id > 0)
            {
                sql += and + "ptid=" + id.ToString();
                and = " and ";
            }
            if (name != null)
            {
                sql += and + "ptname='" + name + "'";
                and = " and ";
            }
            if (unit != null)
            {
                sql += and + "ptunit='" + unit + "'";
                and = " and ";
            }
            if (cost != -1f)
            {
                sql += and + "ptcost=0";
                and = " and ";
            }
            if (created != null)
            {
                sql += and + "created_at >= " + db.DateToStr(created) + " and created_at < date_add(" + db.DateToStr(created) + ", interval 1 day)";
                and = " and ";
            }
            if (updated != null)
            {
                sql += and + "updated_at >= " + db.DateToStr(updated) + " and updated_at < date_add(" + db.DateToStr(updated) + ", interval 1 day)";
                and = " and ";
            }
            if (deleted != null)
            {
                sql += and + "deleted_at >= " + db.DateToStr(deleted) + " and deleted_at < date_add(" + db.DateToStr(deleted) + ", interval 1 day)";
                and = " and ";
            }
            return db.Select(sql);
        }
        public string[,] Add(string name = null, string unit = null, float cost = 0f)
        {
            int id = db.Insert("product_types", new string[] { "ptname", "ptunit", "ptcost" }, new string[] { name, unit, cost.ToString() });
            return Where(id: id);
        }
        public bool Save()
        {
            return db.Update("product_types", new Dictionary<string, string> {
                { "ptname",Name },
                { "ptunit",Unit },
                { "ptcost",Cost.ToString() },
                { "deleted_at",db.DateToStr(Deleted) }
            }, new Dictionary<string, string> {
                { "ptid", ID.ToString() }
            });
        }
    }
}

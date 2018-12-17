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
        public int ID = -1;
        public string Name = "";
        public string Unit = "";
        public float Cost = 0f;
        public DateTime Created = new DateTime();
        public DateTime Updated = new DateTime();
        public DateTime Deleted = new DateTime();
        public Dictionary<string, string> Header = new Dictionary<string, string>() {
            { "ID","ID" },
            { "Name","Название" },
            { "Unit","Валюта" },
            { "Cost","Стоимость" },
            { "Created","Создан" },
            { "Updated","Изменени" },
            { "Deleted","Удален" }
        };
        public ProductType(int id = -1)
        {
            db = new DB();
            if (id > 0)
            {
                ProductType[] arr = Where(id: id);
                if (arr.GetLength(0) > 0)
                {
                    ID = arr[0].ID;
                    Name = arr[0].Name;
                    Unit = arr[0].Unit;
                    Cost = arr[0].Cost;
                    Created = arr[0].Created;
                    Updated = arr[0].Updated;
                    Deleted = arr[0].Deleted;
                }
            }
        }
        public ProductType[] Where(
            string sql = null,
            int id = -1,
            string name = null,
            string unit = null,
            float cost = -1f,
            DateTime created = new DateTime(),
            DateTime updated = new DateTime(),
            DateTime deleted = new DateTime()
        ){
            if (sql == null)
            {
                sql = "select * from product_types ";
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
                if (created != DateTime.MinValue)
                {
                    sql += and + "created_at >= " + db.DateToStr(created) + " and created_at < date_add(" + db.DateToStr(created) + ", interval 1 day)";
                    and = " and ";
                }
                if (updated != DateTime.MinValue)
                {
                    sql += and + "updated_at >= " + db.DateToStr(updated) + " and updated_at < date_add(" + db.DateToStr(updated) + ", interval 1 day)";
                    and = " and ";
                }
                if (deleted != DateTime.MinValue)
                {
                    sql += and + "deleted_at >= " + db.DateToStr(deleted) + " and deleted_at < date_add(" + db.DateToStr(deleted) + ", interval 1 day)";
                    and = " and ";
                }
            }
            string[,] arr = db.Select(sql);
            ProductType[] pts = new ProductType[arr.GetLength(0)];
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                pts[i] = new ProductType();
                pts[i].ID = Convert.ToInt32(arr[i, 0]);
                pts[i].Name = arr[i, 1];
                pts[i].Unit = arr[i, 2];
                pts[i].Cost = (float)Convert.ToDouble(arr[i, 3]);
                pts[i].Created = db.StrToDate(arr[i, 4]);
                pts[i].Updated = db.StrToDate(arr[i, 5]);
                pts[i].Deleted = db.StrToDate(arr[i, 6]);
            }
            return pts;
        }
        public ProductType Add(string name = null, string unit = null, float cost = 0f)
        {
            int id = db.Insert("product_types", new string[] { "ptname", "ptunit", "ptcost" }, new string[] { name, unit, cost.ToString() });
            ProductType[] arr = Where(id: id);
            if (arr.Length > 0)
                return arr[0];
            else
                return null;
        }
        public bool Save()
        {
            if (ID > 0)
            {
                return db.Update("product_types", new Dictionary<string, string> {
                    { "ptname",Name },
                    { "ptunit",Unit },
                    { "ptcost",Cost.ToString() }
                }, new Dictionary<string, string> {
                    { "ptid", ID.ToString() }
                });
            }
            else
            {
                ProductType pt = Add(name: Name, unit: Unit, cost: Cost);
                if (pt.ID > 0)
                {
                    ID = pt.ID;
                    Name = pt.Name;
                    Unit = pt.Unit;
                    Cost = pt.Cost;
                    Created = pt.Created;
                    Updated = pt.Updated;
                    Deleted = pt.Deleted;
                }
                return ID>0;
            }
        }
        public bool Delete()
        {
            return db.Delete("product_types", new Dictionary<string, string> {
                { "ptid", ID.ToString() }
            });
        }
    }
    public class ProductTypeItem
    {
        public ProductTypeItem(ProductType p)
        {
            ID = p.ID;
            Name = p.Name;
            Unit = p.Unit;
            Cost = p.Cost;
            Created = p.Created != DateTime.MinValue ? p.Created.ToLongDateString() : "";
            Updated = p.Updated != DateTime.MinValue ? p.Updated.ToLongDateString() : "";
            Deleted = p.Deleted != DateTime.MinValue ? p.Deleted.ToLongDateString() : "";
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public float Cost { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string Deleted { get; set; }
    }
}

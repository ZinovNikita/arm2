using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace arm2
{
    public class Product
    {
        private DB db = null;
        public int ID = -1;
        public Product Parent;
        public ProductType Type;
        public string SerialNumber = "";
        public bool Reopen = false;
        public DateTime Created = new DateTime();
        public DateTime Updated = new DateTime();
        public DateTime Deleted = new DateTime();
        public Dictionary<string,string> Header = new Dictionary<string, string>() {
            { "ID","ID" },
            { "Parent","Родитель" },
            { "Type","Тип" },
            { "SerialNumber","Серийный номер" },
            { "Reopen","Восстановлен?" },
            { "Created","Создан" },
            { "Updated","Изменени" },
            { "Deleted","Удален" }
        };
        public Product(int id = -1)
        {
            db = new DB();
            if (id > 0)
            {
                Product[] arr = Where(id: id);
                if (arr.GetLength(0) > 0)
                {
                    ID = arr[0].ID;
                    Parent = arr[0].Parent;
                    Type = arr[0].Type;
                    SerialNumber = arr[0].SerialNumber;
                    Reopen = arr[0].Reopen;
                    Created = arr[0].Created;
                    Updated = arr[0].Updated;
                    Deleted = arr[0].Deleted;
                }
            }
        }
        public Product[] Where(
            string sql = null,
            int id = -1,
            int parent = -1,
            int type = -1,
            string snumber = null,
            bool reopen = false,
            DateTime created = new DateTime(),
            DateTime updated = new DateTime(),
            DateTime deleted = new DateTime(),
            bool is_active = false
        ){
            if (sql == null)
            {
                sql = "select * from products ";
                string and = " where ";
                if (id > 0)
                {
                    sql += and + "pid=" + id.ToString();
                    and = " and ";
                }
                if (parent > 0)
                {
                    sql += and + "pparent=" + parent.ToString();
                    and = " and ";
                }
                if (type > 0)
                {
                    sql += and + "pptid=" + type.ToString();
                    and = " and ";
                }
                if (snumber != null)
                {
                    sql += and + "psernum='" + snumber + "'";
                    and = " and ";
                }
                if (reopen)
                {
                    sql += and + "preopen=1";
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
                if (is_active)
                {
                    sql += and + "deleted_at is null";
                    and = " and ";
                }
                else
                {
                    if (deleted != DateTime.MinValue)
                    {
                        sql += and + "deleted_at >= " + db.DateToStr(deleted) + " and deleted_at < date_add(" + db.DateToStr(deleted) + ", interval 1 day)";
                        and = " and ";
                    }
                }
            }
            string[,] arr = db.Select(sql);
            Product[] parr = new Product[arr.GetLength(0)];
            for (int i = 0; i < parr.Length; i++)
            {
                parr[i] = new Product();
                parr[i].ID = Convert.ToInt32(arr[i, 0]);
                parr[i].Parent = new Product(id: Convert.ToInt32(arr[i, 1]));
                parr[i].Type = new ProductType(id: Convert.ToInt32(arr[i, 2]));
                parr[i].SerialNumber = arr[i, 3];
                parr[i].Reopen = arr[i, 4] == "1";
                parr[i].Created = db.StrToDate(arr[i, 5]);
                parr[i].Updated = db.StrToDate(arr[i, 6]);
                parr[i].Deleted = db.StrToDate(arr[i, 7]);
            }
            return parr;

        }
        public Product Add(int parent = -1, int type = -1, string snumber = null)
        {
            int id = db.Insert("products", new string[] { "pparent", "pptid", "psernum" }, new string[] { parent.ToString(), type.ToString(), snumber });
            Product[] arr = Where(id: id);
            if (arr.Length > 0)
                return arr[0];
            else
                return null;
        }
        public bool Save()
        {
            return db.Update("products", new Dictionary<string, string> {
                { "pparent",Parent.ID.ToString() },
                { "pptid",Type.ID.ToString()},
                { "psernum",SerialNumber},
                { "preopen",Reopen?"1":"0"}
            }, new Dictionary<string, string> {
                { "pid", ID.ToString() }
            });
        }
    }
    public class ProductItem
    {
        public ProductItem(Product p)
        {
            ID = p.ID;
            Parent = p.Parent.ID;
            Type = p.Type.Name;
            SerialNumber = p.SerialNumber;
            Reopen = p.Reopen;
            Created = p.Created.ToLongDateString();
            Updated = p.Updated.ToLongDateString();
            Deleted = p.Deleted.ToLongDateString();
        }
        public int ID { get; set; }
        public int Parent { get; set; }
        public string Type { get; set; }
        public string SerialNumber { get; set; }
        public bool Reopen { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string Deleted { get; set; }
    }
}

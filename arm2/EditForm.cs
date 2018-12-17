using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arm2
{
    class EditForm : Form
    {
        private int TopPos = 10;
        private Product editProduct;
        private ProductType editType;
        public EditForm(Product product = null, ProductType type = null)
        {
            this.Text = "Изменение объекта";
            this.Width = 285;
            editProduct = product;
            editType = type;
            if (editType != null)
            {
                Control[] idFld0 = Element("ReadOnly", editType.Header["ID"]);
                this.Controls.Add(idFld0[0]);
                idFld0[1].Text = editType.ID.ToString();
                idFld0[1].Name = "ID";
                this.Controls.Add(idFld0[1]);
                Control[] nmFld = Element("Input", editType.Header["Name"]);
                this.Controls.Add(nmFld[0]);
                nmFld[1].Text = editType.Name;
                nmFld[1].Name = "Name";
                this.Controls.Add(nmFld[1]);
                Control[] utFld = Element("Input", editType.Header["Unit"]);
                this.Controls.Add(utFld[0]);
                utFld[1].Text = editType.Unit;
                utFld[1].Name = "Unit";
                this.Controls.Add(utFld[1]);
                Control[] ctFld = Element("Number", editType.Header["Cost"]);
                this.Controls.Add(ctFld[0]);
                NumericUpDown ctud = (NumericUpDown)ctFld[1];
                ctud.Text = editType.Cost.ToString();
                ctud.Maximum = Int32.MaxValue;
                ctud.Minimum = 0;
                ctud.Name = "Cost";
                this.Controls.Add(ctud);
                Control[] save = Element("Button");
                save[1].Text = "Сохранить";
                save[1].Click += new EventHandler(SaveClick);
                this.Controls.Add(save[1]);
                if (editType.ID > 0) {
                    Control[] del = Element("Button");
                    del[1].Text = " Удалить";
                    del[1].Click += new EventHandler(DelClick);
                    this.Controls.Add(del[1]);
                }
            }
            else
            {
                Control[] idFld = Element("ReadOnly", editProduct.Header["ID"]);
                this.Controls.Add(idFld[0]);
                idFld[1].Text = editProduct.ID.ToString();
                idFld[1].Name = "ID";
                this.Controls.Add(idFld[1]);
                Control[] parentFld = Element("Select", editProduct.Header["Parent"]);
                this.Controls.Add(parentFld[0]);
                ComboBox parent = (ComboBox)parentFld[1];
                Product[] arr = new Product().Where(sql: "select * from products where deleted_at is null and pid<>"+ editProduct.ID.ToString());
                foreach (Product a in arr)
                {
                    parent.Items.Add(new KeyValuePair<string, int>(a.SerialNumber, a.ID));
                    if (editProduct.Parent != null && a.ID == editProduct.Parent.ID)
                        parent.SelectedIndex = parent.Items.IndexOf(new KeyValuePair<string, int>(a.SerialNumber, a.ID));
                }
                parent.DisplayMember = "key";
                parent.ValueMember = "value";
                parent.Name = "Parent";
                this.Controls.Add(parent);
                Control[] typeFld = Element("Select", editProduct.Header["Type"]);
                this.Controls.Add(typeFld[0]);
                ComboBox ptype = (ComboBox)typeFld[1];
                ProductType[] arr2 = new ProductType().Where(sql: "select * from product_types where deleted_at is null");
                foreach (ProductType a in arr2)
                {
                    ptype.Items.Add(new KeyValuePair<string, int>(a.Name, a.ID));
                    if (editProduct.Type != null && a.ID == editProduct.Type.ID)
                        ptype.SelectedIndex = ptype.Items.IndexOf(new KeyValuePair<string, int>(a.Name, a.ID));
                }
                ptype.DisplayMember = "key";
                ptype.ValueMember = "value";
                ptype.Name = "ProductType";
                this.Controls.Add(ptype);
                Control[] snFld = Element("Input", editProduct.Header["SerialNumber"]);
                this.Controls.Add(snFld[0]);
                snFld[1].Text = editProduct.SerialNumber;
                snFld[1].Name = "SerialNumber";
                this.Controls.Add(snFld[1]);
                Control[] rpFld = Element("Check", editProduct.Header["Reopen"]);
                this.Controls.Add(rpFld[0]);
                CheckBox cbrp = (CheckBox)rpFld[1];
                cbrp.Checked = editProduct.Reopen;
                cbrp.Name = "Reopen";
                this.Controls.Add(cbrp);
                Control[] save = Element("Button");
                save[1].Text = "Сохранить";
                save[1].Click += new EventHandler(SaveClick);
                this.Controls.Add(save[1]);
                if (editProduct.ID > 0)
                {
                    Control[] del = Element("Button");
                    del[1].Text = " Удалить";
                    del[1].Click += new EventHandler(DelClick);
                    this.Controls.Add(del[1]);
                }
            }
            this.Height = TopPos+45;
        }
        private Control[] Element(string type, string title="")
        {
            Control[] ctrl = new Control[2];
            ctrl[0] = new Label();
            ctrl[0].Text = title;
            ctrl[0].Width = 250;
            ctrl[0].Height = 15;
            ctrl[0].Left = 10;
            ctrl[0].Top = TopPos;
            TopPos += 15;
            switch (type)
            {
                case "Input":ctrl[1] = new TextBox();break;
                case "DateTime":ctrl[1] = new DateTimePicker();break;
                case "Check":ctrl[1] = new CheckBox();break;
                case "Select":ctrl[1] = new ComboBox();break;
                case "Number":ctrl[1] = new NumericUpDown();break;
                case "ReadOnly":ctrl[1] = new Label();break;
                case "Button":ctrl[1] = new Button();break;
            }
            ctrl[1].Width = 250;
            ctrl[1].Height = 25;
            ctrl[1].Left = 10;
            ctrl[1].Top = TopPos;
            TopPos += 30;
            return ctrl;
        }
        public void SaveClick(object sender, EventArgs e)
        {
            bool success = false;
            if (editType != null)
            {
                editType.Name = this.Controls.Find("Name", true)[0].Text;
                editType.Unit = this.Controls.Find("Unit", true)[0].Text;
                float.TryParse(this.Controls.Find("Cost", true)[0].Text, out editType.Cost);
                success = editType.Save();
            }
            else
            {
                editProduct.Parent = new Product(id: ComboBoxValue(this.Controls.Find("Parent", true)[0]));
                editProduct.Type = new ProductType(id: ComboBoxValue(this.Controls.Find("ProductType", true)[0]));
                editProduct.SerialNumber = this.Controls.Find("SerialNumber", true)[0].Text;
                editProduct.Reopen = ((CheckBox)this.Controls.Find("Reopen", true)[0]).Checked;
                success = editProduct.Save();
            }
            if (success)
            {
                MessageBox.Show("Изменения успешно сохранены");
                this.Close();
            }
            else
                MessageBox.Show("Не удалось сохранить");
        }
        public void DelClick(object sender, EventArgs e)
        {
            bool success = false;
            if (editType != null)
                success = editType.Delete();
            else
                success = editProduct.Delete();
            if (success)
            {
                MessageBox.Show("Запись успешно удалена");
                this.Close();
            }
            else
                MessageBox.Show("Не удалось удалить");
        }
        private int ComboBoxValue(Control ctrl)
        {
            ComboBox cb = (ComboBox)ctrl;
            if (cb.SelectedValue != null)
                MessageBox.Show((string)cb.SelectedValue);
            if (cb.SelectedItem != null)
            {
                KeyValuePair<string, int> pkv = (KeyValuePair<string, int>)cb.SelectedItem;
                return pkv.Value;
            }
            return -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace arm2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User CurrentUser;
        public MainWindow()
        {
            InitializeComponent();
            Window1 loginWindow = new Window1();
            loginWindow.CheckLogin(this);
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
        }
        public void CanAccess(int uid)
        {
            IsEnabled = true;
            CurrentUser = new User(uid);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Items.Clear();
            dataGrid.Columns.Clear();
            ProductType list = new ProductType();
            foreach (var h in list.Header)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = h.Value;
                textColumn.Binding = new Binding(h.Key);
                dataGrid.Columns.Add(textColumn);
            }
            dataGrid.Tag = "ProductType";
            ProductType[] list2 = list.Where(sql: "select * from product_types where deleted_at is null");
            foreach (ProductType p in list2)
            {
                dataGrid.Items.Add(new ProductTypeItem(p));
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            dataGrid.Items.Clear();
            dataGrid.Columns.Clear();
            Product list = new Product();
            foreach(var h in list.Header)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = h.Value;
                textColumn.Binding = new Binding(h.Key);
                dataGrid.Columns.Add(textColumn);
            }
            dataGrid.Tag = "Product";
            Product[] list2 = list.Where(sql: "select * from products where deleted_at is null");
            foreach(Product p in list2)
            {
                dataGrid.Items.Add(new ProductItem(p));
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (dataGrid.Tag)
            {
                case "Product":
                    ProductItem pi = (ProductItem)dataGrid.SelectedItem;
                    Product p = new Product(id: pi.ID);
                    EditForm ef = new EditForm(product: p);
                    ef.FormClosed += DataGrid_Refresh;
                    ef.Show();
                    break;
                case "ProductType":
                    ProductTypeItem pti = (ProductTypeItem)dataGrid.SelectedItem;
                    ProductType pt = new ProductType(id: pti.ID);
                    EditForm eft = new EditForm(type: pt);
                    eft.FormClosed += DataGrid_Refresh;
                    eft.Show();
                    break;
            }
        }

        private void DataGrid_Refresh(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            switch (dataGrid.Tag)
            {
                case "Product":
                    MenuItem_Click_1(sender, null);
                    break;
                case "ProductType":
                    MenuItem_Click(sender, null);
                    break;
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            EditForm eft = new EditForm(type: new ProductType());
            dataGrid.Tag = "ProductType";
            eft.FormClosed += DataGrid_Refresh;
            eft.Show();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            EditForm ef = new EditForm(product: new Product());
            dataGrid.Tag = "Product";
            ef.FormClosed += DataGrid_Refresh;
            ef.Show();
        }
    }
}

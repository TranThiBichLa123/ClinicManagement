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
using DTO;
using BLL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Collections.ObjectModel;
namespace ClinicManagement.SidebarItems
{
   
    public partial class Setting : UserControl
    {
        SettingBLL settingBLL = new SettingBLL();
        SettingBLL accountBLL = new SettingBLL();
        SettingBLL staffBLL = new SettingBLL();

        private ObservableCollection<DTO.Setting> roleList;
        private ObservableCollection<DTO.Account> accountList;
        private ObservableCollection<DTO.Staff> staffList;

        public Setting()
        {
            InitializeComponent();
            roleList = new ObservableCollection<DTO.Setting>();
            accountList = new ObservableCollection<DTO.Account>();
            staffList = new ObservableCollection<DTO.Staff>();

            roleDataGrid.ItemsSource = roleList;
            accountDataGrid.ItemsSource = accountList;
            staffDataGrid.ItemsSource = staffList;

            LoadRoleInfo();
            LoadAccountInfo();
            LoadStaffInfo();

        }
        private void LoadRoleInfo()
        {
            var roles = settingBLL.GetRoleList();
            roleList.Clear();
            foreach (var role in roles)
            {
                roleList.Add(role);
            }
        }
        private void LoadAccountInfo()
        {
            var accounts = accountBLL.GetAccountList();
            accountList.Clear();
            foreach (var acc in accounts)
            {
                accountList.Add(acc);
            }
        }
        private void LoadStaffInfo()
        {
            var staff = staffBLL.GetStaffList();
            staffList.Clear();
            foreach (var s in staff)
            {
                staffList.Add(s);
            }
        }

        private void btnEditRole_Click(object sender, RoutedEventArgs e)
        {
            EditRole editRole = new EditRole();
            editRole.ShowDialog();
        }
    }
}

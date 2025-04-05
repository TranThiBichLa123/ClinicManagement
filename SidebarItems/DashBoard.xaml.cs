using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : UserControl
    {
        public DashBoard()
        {
            InitializeComponent();
        }

        private void prf_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = prf;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
        }

        private void prf_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window.GetWindow(this)?.DragMove();
            }
        }
    }
}

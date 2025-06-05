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
using BLL;
using DTO;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for ReceiptList.xaml
    /// </summary>
    public partial class ReceiptList : UserControl
    {
       
        
        public ReceiptList()
        {
            InitializeComponent();
            
        }
        private void InvoiceList_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
          
        }


        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            

        }

      
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void AddClick(object sender, RoutedEventArgs e)
        {
            
            NewDrug newDrug = new NewDrug();
            newDrug.ShowDialog();
        
    }
    }
}

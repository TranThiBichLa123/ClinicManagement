using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace ClinicManagement
{
    

    public class ChucNangPhanQuyenVM : INotifyPropertyChanged
    {
        private bool _duocCapQuyen;

        public int ID_ChucNang { get; set; }

        public string TenChucNang { get; set; }

        public string CapDoPhanQuyen { get; set; }

        public bool DuocCapQuyen
        {
            get => _duocCapQuyen;
            set
            {
                if (_duocCapQuyen != value)
                {
                    _duocCapQuyen = value;
                    OnPropertyChanged(nameof(DuocCapQuyen));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

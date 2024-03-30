using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagementPrism.Model
{
    public class ControlModel:BindableBase
    {
        private int _id;

        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _device;

        public string Device
        {
            get { return _device; }
            set { SetProperty(ref _device, value); }
        }

        private string _point;

        public string Point
        {
            get { return _point; }
            set { SetProperty(ref _point, value); }
        }

        private string _condition;

        public string Condition
        {
            get { return _condition; }
            set { SetProperty(ref _condition, value); }
        }

        private string _threshold;

        public string Threshold
        {
            get { return _threshold; }
            set { SetProperty(ref _threshold, value); }
        }


        private string _do;

        public string Do
        {
            get { return _do; }
            set { SetProperty(ref _do, value); }
        }

        private string _openclose;

        public string OpenClose
        {
            get { return _openclose; }
            set { SetProperty(ref _openclose, value); }
        }

        private int _status;

        public int Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
    }
}

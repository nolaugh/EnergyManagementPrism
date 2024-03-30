using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagementPrism.Model
{
    public class HistoryDataModel:BindableBase
    {
        private int _id;

        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _point;

        public string Point
        {
            get { return _point; }
            set { SetProperty(ref _point, value); }
        }

        private decimal _usingpower;

        public decimal UsingPower
        {
            get { return _usingpower; }
            set { SetProperty(ref _usingpower, value); }
        }

        private decimal _data;

        public decimal Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        private DateTime _time;

        public DateTime Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        private string _loglevel;

        public string LogLevel
        {
            get { return _loglevel; }
            set { SetProperty(ref _loglevel, value); }
        }

    }
}

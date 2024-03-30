using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagementPrism.Model
{
    public class Device : BindableBase
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

        private decimal _volotage;

        public decimal Volotage
        {
            get { return _volotage; }
            set { SetProperty(ref _volotage, value); }
        }

        private decimal _electricit;

        public decimal Electricit
        {
            get { return _electricit; }
            set { SetProperty(ref _electricit, value); }
        }

        private decimal _power;

        public decimal Power
        {
            get { return _power; }
            set { SetProperty(ref _power, value); }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }
    }

}

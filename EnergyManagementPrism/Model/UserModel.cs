using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagementPrism.Model
{
    public class UserModel:BindableBase
    {
        private int _id;

        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _account;

        public string Account
        {
            get { return _account; }
            set { SetProperty(ref _account, value); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private int _permissions;

        public int Permissions
        {
            get { return _permissions; }
            set { SetProperty(ref _permissions, value); }
        }
    }
}

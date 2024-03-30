using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagementPrism.Model
{
   public  class LoginModel
    {
        //账户
        public string Account { get; set; }

        //密码
        public string Password { get; set; }

        //权限
        public  int Permissions { get; set; }

    }
}

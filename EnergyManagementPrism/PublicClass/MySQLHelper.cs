using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using Mysqlx.Crud;
using System.Threading;

namespace EnergyManagementPrism.PublicClass
{
    class MySQLHelper
    {
        private static MySqlConnection connection = null;

        private static string connectionString = "server=localhost;database=db_energymanagement;user id=root;password=123456;Port = 3306;CharacterSet = utf8";

        //连接数据库
        public static MySqlConnection Connection()
        {

            if (connection != null)
            {
                return connection;
            }
            else
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            return connection;
        }

        //关闭数据库
        public static void Disconnection()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            try
            {
                DataTable dataTable = new DataTable();
                MySqlCommand command = Connection().CreateCommand();
                command.CommandText = query;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
                return dataTable;
            }
            catch
            {
                throw;
            }         
        }

        public void InsertDevice(string name, decimal volotage, decimal electricit, decimal power, string address)
        {
            string query = "INSERT INTO tb_device (Name, Volotage, Electricit, Power, Address) VALUES (@Name, @Volotage, @Electricit, @Power, @Address)";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Name", name),
                new MySqlParameter("@Volotage", volotage),
                new MySqlParameter("@Electricit", electricit),
                new MySqlParameter("@Power", power),
                new MySqlParameter("@Address", address)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void InsertUser(string account, string password, int permissions)
        {
            string query = "INSERT INTO tb_login (Account, Password, Permissions) VALUES ( @Account, @Password, @Permissions)";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Account", account),
                new MySqlParameter("@Password", password),
                new MySqlParameter("@Permissions", permissions),
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void InsertControl(string device, string point,string condition,string threshold,string ocdo,string openclose,int status)
        {
            string query = "INSERT INTO tb_control (Device, Point, `Condition`, Threshold, `Do`, OpenClose, Status) VALUES (@Device, @Point, @Condition, @Threshold, @Do, @OpenClose, @Status)";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Device", device),
                new MySqlParameter("@Point", point),
                new MySqlParameter("@Condition", condition),
                new MySqlParameter("@Threshold", threshold),
                new MySqlParameter("@Do", ocdo),
                new MySqlParameter("@OpenClose", openclose),
                new MySqlParameter("@Status", status)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void UpdateDevice(int id, string name, decimal volotage, decimal electricit, decimal power, string address)
        {
            string query = "UPDATE tb_device SET Name = @Name, Volotage = @Volotage, Electricit = @Electricit, Power = @Power, Address = @Address WHERE ID = @ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Name", name),
                new MySqlParameter("@Volotage", volotage),
                new MySqlParameter("@Electricit", electricit),
                new MySqlParameter("@Power", power),
                new MySqlParameter("@Address", address),
                new MySqlParameter("@ID", id)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void UpdateUser(int id, string account, string password, int permissions)
        {
            string query = "UPDATE tb_login SET  Account = @Account, Password = @Password, Permissions = @Permissions WHERE ID = @ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Account", account),
                new MySqlParameter("@Password", password),
                new MySqlParameter("@Permissions", permissions),
                new MySqlParameter("@ID", id)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void DeleteDevice(int id)
        {
            string query = "DELETE FROM tb_device WHERE ID = @ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@ID", id)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void UpdateControl(int id,string device, string point, string condition, string threshold, string ocdo, string openclose, int status)
        {
            string query = "UPDATE tb_control SET Device = @Device, Point = @Point, `Condition` = @Condition, " +
                "Threshold = @Threshold, `Do` = @Do, OpenClose = @OpenClose, Status = @Status WHERE ID = @ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
               new MySqlParameter("@ID", id),
               new MySqlParameter("@Device", device),
               new MySqlParameter("@Point", point),
               new MySqlParameter("@Condition", condition),
               new MySqlParameter("@Threshold", threshold),
               new MySqlParameter("@Do", ocdo),
               new MySqlParameter("@OpenClose", openclose),
               new MySqlParameter("@Status", status)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void UpdateControl(int id,int status)
        {
            string query = "UPDATE tb_control SET Status = @Status WHERE ID = @ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
               new MySqlParameter("@ID", id),
               new MySqlParameter("@Status", status)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void DeleteUser(int id)
        {
            string query = "DELETE FROM tb_login WHERE ID = @ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@ID", id)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void DeleteControl(int id)
        {
            string query = "DELETE FROM tb_control WHERE ID = @ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@ID", id)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void UpdateDeviceBind(int bindid,int id)
        {
            string query = "UPDATE tb_devicebind SET BindId = @BindId where ID=@ID";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                
                new MySqlParameter("@BindId", bindid),
                new MySqlParameter("@ID", id)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public DataTable GetAllConnectData()
        {
            string query = "SELECT * FROM tb_connect";
            return ExecuteQuery(query);
        }

        public void UpdateConnectData(string ip, string port, string com, int baudRate, int dataBits, int stopBits, string parity)
        {
            string query = "UPDATE tb_connect SET IP = @IP, Port = @Port, Com = @Com, BaudRate = @BaudRate, DataBits = @DataBits, StopBits = @StopBits, Parity = @Parity WHERE ID = 1";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@IP", ip),
                new MySqlParameter("@Port", port),
                new MySqlParameter("@Com", com),
                new MySqlParameter("@BaudRate", baudRate),
                new MySqlParameter("@DataBits", dataBits),
                new MySqlParameter("@StopBits", stopBits),
                new MySqlParameter("@Parity", parity)
            };

            ExecuteQuery(query, parameters.ToArray());
        }

        public void InsertHistory(string name, string point, decimal usingpower, decimal data, DateTime dateTime,string loglevel)
        {
            string query = "INSERT INTO tb_historydata (Name, Point, UsingPower, Data, Time, LogLevel) VALUES " +
                "(@Name, @Point, @UsingPower, @Data, @Time,@LogLevel)";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Name", name),
                new MySqlParameter("@Point", point),
                new MySqlParameter("@UsingPower", usingpower),
                new MySqlParameter("@Data", data),
                new MySqlParameter("@Time", dateTime),
                new MySqlParameter("@LogLevel", loglevel),
            };

            ExecuteQuery(query, parameters.ToArray());
        }

       
    }
}

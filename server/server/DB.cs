using ContentPackage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class DB
    {
        static string connectString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\LTM\project\LTM\server\server\LTM.mdf;Integrated Security=True";

        public static Account GetAccountByUserName(string UserName)
        {
            DataTable dt = new DataTable();
            using(var conn = new SqlConnection(connectString))
            {
                string query = "select * from Account where UName='" + UserName + "'";
                SqlDataAdapter adapter =new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(query, conn);
                adapter.Fill(dt);

            }
            if(dt.Rows.Count > 0)
            {
                return new Account() { username = dt.Rows[0]["UName"].ToString(), password = dt.Rows[0]["PWord"].ToString() };
            }
            return null;
        }
    }
}

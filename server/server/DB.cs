using ContentPackage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class DB
    {
        static string connectString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\sieus\Desktop\LTM-develop\LTM-develop\server\server\LTM.mdf;Integrated Security=True";

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

        public static Boolean UserSignUp(Account acc)
        {
            Boolean flag = true;
            SqlConnection connection = new SqlConnection(connectString);
            string query = $"INSERT INTO ACCOUNT (UName, PWord, FName) VALUES ({acc.username}, {acc.password}, 'FName')";
            Console.WriteLine(query);
            try
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.InsertCommand = new SqlCommand(query, connection);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                flag = false;
            }
            return flag;
        }
    }
}

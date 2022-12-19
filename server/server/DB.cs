using ContentPackage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace server
{
    internal class DB
    {
        static string connectString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\LTM\project\LTM\server\server\LTM.mdf;Integrated Security=True";

        public static Account GetAccountByUserName(string UserName)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(connectString))
                {
                    string query = "select * from Account where UName='" + UserName + "'";
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, conn);
                    adapter.Fill(dt);

                }
                if (dt.Rows.Count > 0)
                {
                    return new Account() { username = dt.Rows[0]["UName"].ToString(), password = dt.Rows[0]["PWord"].ToString(), fullname = dt.Rows[0]["FName"].ToString() };
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }

        public static List<Account> GetFriends(string userName)
        {
            List<Account> friends = new List<Account>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "select FrUName, FName from friend as f left join account as a on f.FrUName=a.UName where f.UName=@UName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar, 20).Value = userName;
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Account friend = new Account() { username = reader.GetString("FrUName"), fullname = reader.GetString("FName"), password = "", online = false };
                            friends.Add(friend);
                        }
                    }
                    conn.Close();
                }
                return friends;
            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }

        public static bool CreateGroupChat(InGroup inGroup)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "insert into GroupChat(GName, Admin) values(@GName, @Admin); select cast(scope_identity() as int)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@GName", SqlDbType.NVarChar, 20).Value = inGroup.GChat.GName;
                    cmd.Parameters.Add("@Admin", SqlDbType.VarChar, 20).Value = inGroup.GChat.Admin;
                    conn.Open();
                    var id = cmd.ExecuteScalar();
                    inGroup.GChat.Id = Convert.ToInt32(id);
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar);
                    foreach (string member in inGroup.Members)
                    {
                        cmd.CommandText = "insert into InGroup values(@Id, @UName);";
                        cmd.Parameters["@Id"].Value = id;
                        cmd.Parameters["@UName"].Value = member;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
            return false;
        }

        public static List<string> getUsersInGroup(int id, string username)
        {
            List<string> users = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "select UName from InGroup where Id=@Id and UName<>@UName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = username;
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            users.Add(reader.GetString("UName"));
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return users;
        }

        public static List<GroupChat> getGroupChats(string username)
        {
            List<GroupChat> groupChats = new List<GroupChat>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "select gc.Id, GName, Admin from GroupChat as gc left join InGroup as ig on gc.Id=ig.Id where UName=@UName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = username;
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int Id = reader.GetInt32("Id");
                            string GName = reader.GetString("GName");
                            string Admin = reader.GetString("Admin");
                            groupChats.Add(new GroupChat() { Id = Id, GName = GName, Admin = Admin, online = false });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return groupChats;
        }

        public static bool checkAccountExisted(string username)
        {
            bool result = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "select UName from Account where UName=@UName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = username;
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        result = true;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public static bool addAccount(Account account)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "insert into Account(FName, UName, PWord) values(@FName, @UName, @PWord)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@FName", SqlDbType.NVarChar).Value = account.fullname;
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = account.username;
                    cmd.Parameters.Add("@PWord", SqlDbType.VarChar).Value = account.password;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static List<Account> searchUsers(Search search)
        {
            List<Account> result = new List<Account>();
            if (!string.IsNullOrEmpty(search.search))
            {
                search.search = search.search + "%";
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectString))
                    {
                        //string query = "select UName, FName from account where (UName like @Search or FName like @Search) and (UName<>@UName and UName not in (select (case @UName when Sender then Receiver when Receiver then Sender end) from FriendRequest where Sender=@UName or Receiver=@UName) and UName not in (select FrUName from Friend where UName=@UName));";
                        string query = "select a.UName, a.FName, (case when a.UName in (select Receiver from FriendRequest as fr where fr.Sender=@UName) then 1 when a.UName in (select FrUName from Friend where UName=@UName) then 2 else 0 end) as FrOrRe from Account as a where (UName like @Search or FName like @Search) and UName<>@UName and UName not in (select Sender from FriendRequest where Receiver=@UName);";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.Add("@Search", SqlDbType.NVarChar).Value = search.search;
                        cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = search.username;
                        conn.Open();
                        using var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Account user = new Account() { username = reader.GetString("UName"), password = "", fullname = reader.GetString("FName"), FrOrRe = reader.GetInt32("FrOrRe") };
                                result.Add(user);
                            }
                        }
                        conn.Close();
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        public static Account addFriendRequest(Message message)
        {
            Account result = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "insert into FriendRequest(Sender, Receiver) values(@Sender, @Receiver)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Sender", SqlDbType.VarChar).Value = message.sender;
                    cmd.Parameters.Add("@Receiver", SqlDbType.VarChar).Value = message.receiver;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "select UName, FName from Account where UName=@UName";
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = message.sender;
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.username = reader.GetString("UName");
                            result.fullname = reader.GetString("FName");
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception)
            {

                return null;
            }

            return result;
        }

        public static List<Account> getFriendRequests(string username)
        {
            List<Account> result = new List<Account>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "select UName, FName from FriendRequest as fr left join Account as a on fr.Sender=a.UName where Receiver=@UName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = username;
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Account user = new Account() { username = reader.GetString("UName"), password = "", fullname = reader.GetString("FName") };
                            result.Add(user);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

        public static Account acceptFriendRequest(Message message)
        {
            Account result = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "insert into Friend values(@UName, @FrUName), (@FrUName, @UName);";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = message.sender;
                    cmd.Parameters.Add("@FrUName", SqlDbType.VarChar).Value = message.receiver;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    query = "delete from FriendRequest where Sender=@FrUName and Receiver=@UName";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select UName, FName from Account where UName=@FrUName";
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.username = reader.GetString("UName");
                            result.password = "";
                            result.fullname = reader.GetString("FName");
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }

        public static List<Message> getWaitingMessage(string username)
        {
            List<Message> result = new List<Message>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "select * from WaitingMessage where Receiver=@Receiver";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Receiver", SqlDbType.VarChar).Value = username;
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Message message = new Message() { sender = reader.GetString("Sender"), content = Encoding.UTF8.GetString(reader.GetSqlBytes(1).Buffer), receiver = reader.GetString("Receiver") };
                            result.Add(message);
                        }
                    }
                    reader.Close();
                    cmd.CommandText = "delete from WaitingMessage where Receiver=@Receiver";
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

        public static void denyFriendRequest(ContentPackage.Message message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "delete from FriendRequest where Sender=@Sender and Receiver=@Receiver";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Sender", SqlDbType.VarChar).Value = message.receiver;
                    cmd.Parameters.Add("@Receiver", SqlDbType.VarChar).Value = message.sender;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }
        }
        public static void addWaitingMessage(Message message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "insert into WaitingMessage(Sender, Content, Receiver) values(@Sender, @Content, @Receiver)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Sender", SqlDbType.VarChar).Value = message.sender;
                    cmd.Parameters.Add("@Content", SqlDbType.Image).Value = Encoding.UTF8.GetBytes(message.content);
                    cmd.Parameters.Add("@Receiver", SqlDbType.VarChar).Value = message.receiver;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        public static void cancelRequest(Message message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "delete from FriendRequest where Sender=@Sender and Receiver=@Receiver";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Sender", SqlDbType.VarChar).Value = message.sender;
                    cmd.Parameters.Add("@Receiver", SqlDbType.VarChar).Value = message.receiver;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        public static void cancelFriend(Message message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectString))
                {
                    string query = "delete from Friend where (UName=@UName and FrUName=@FrUName) or (UName=@FrUName and FrUName=@UName)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@UName", SqlDbType.VarChar).Value = message.sender;
                    cmd.Parameters.Add("@FrUName", SqlDbType.VarChar).Value = message.receiver;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}

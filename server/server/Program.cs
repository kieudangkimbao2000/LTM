using ContentPackage;
using server;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Schema;

Console.InputEncoding = Encoding.UTF8;
IPEndPoint iep = new IPEndPoint(IPAddress.Parse(getLocalIP()), 2000);
TcpListener server = new TcpListener(iep);
server.Start();
Console.WriteLine("Cho client ket noi ...");
bool active = true;
Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
Thread thread = new Thread(() => {
    string s = Console.ReadLine();
    if(s.Equals("false"))
    {
        active = Convert.ToBoolean(s);
    }
});
thread.IsBackground = true;
thread.Start();
while(active)
{
    TcpClient client = server.AcceptTcpClient();
    Console.WriteLine("Co client ket noi.");
    Thread thread1 = new Thread(() => { clientRecieve(client); });
    thread1.IsBackground = true;
    thread1.Start();
}

void clientRecieve(TcpClient client)
{
    string username = "";
    StreamReader sr = new StreamReader(client.GetStream());
    Package? package = new Package();
    try
    {
        while (true)
        {
            string receiveStr = sr.ReadLine();
            package = JsonSerializer.Deserialize<Package>(receiveStr);
            if (package != null)
            {
                if (package.content != null)
                {
                    List<Account> friends;
                    List<GroupChat> groupChats;
                    switch (package.kind)
                    {
                        #region 201
                        case 201:
                            Account regisAcc = JsonSerializer.Deserialize<Account>(package.content);
                            StreamWriter sw = new StreamWriter(client.GetStream());
                            string sendedContent = "";
                            if (DB.checkAccountExisted(regisAcc.username))
                            {
                                package.kind = 102;
                                package.content = "";
                                sendedContent = JsonSerializer.Serialize<ContentPackage.Package>(package);
                                sw.WriteLine(sendedContent);
                                sw.Flush();
                            }
                            if (DB.addAccount(regisAcc))
                            {
                                LoginSuccess loginSuccess = new LoginSuccess() { AccountLogin = regisAcc, Friends = new List<Account>(), GroupChats = new List<GroupChat>() };
                                package.kind = 101;
                                package.content = JsonSerializer.Serialize<LoginSuccess>(loginSuccess);
                                sendedContent = JsonSerializer.Serialize<ContentPackage.Package>(package);
                                sw.WriteLine(sendedContent);
                                sw.Flush();
                            }
                            else
                            {
                                package.kind = 102;
                                package.content = "";
                                sendedContent = JsonSerializer.Serialize<ContentPackage.Package>(package);
                                sw.WriteLine(sendedContent);
                                sw.Flush();
                            }
                            break;
                        #endregion
                        #region 202
                        case 202:
                            Account account = JsonSerializer.Deserialize<Account>(package.content);
                            Account accountCheck = DB.GetAccountByUserName(account.username);
                            sw = new StreamWriter(client.GetStream());
                            if (accountCheck != null)
                            {
                                if (accountCheck.password == account.password)
                                {
                                    friends = DB.GetFriends(account.username);
                                    foreach (Account friendOnline in friends)
                                    {
                                        if (clients.Keys.Contains(friendOnline.username))
                                        {
                                            friendOnline.online = true;
                                            StreamWriter swFriend = new StreamWriter(clients[friendOnline.username].GetStream());
                                            Online online = new Online() { index = accountCheck.username, online = true };
                                            ContentPackage.Package onlinePackage = new ContentPackage.Package() { kind = 107, content = JsonSerializer.Serialize<Online>(online) };
                                            swFriend.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(onlinePackage));
                                            swFriend.Flush();
                                        }
                                    }
                                    groupChats = DB.getGroupChats(account.username);
                                    foreach (GroupChat groupChat in groupChats)
                                    {
                                        checkGroupOnline(groupChat, accountCheck.username);
                                    }
                                    List<Account> friendRequests = DB.getFriendRequests(account.username);
                                    List<Message> waitingMessage = DB.getWaitingMessage(account.username);
                                    LoginSuccess loginSuccess = new LoginSuccess() { AccountLogin = accountCheck, Friends = friends, GroupChats = groupChats, FriendRequests = friendRequests, WaitingMessages = waitingMessage};
                                    package.kind = 101;
                                    package.content = JsonSerializer.Serialize<LoginSuccess>(loginSuccess);
                                    string sendStr = JsonSerializer.Serialize<Package>(package);
                                    sw.WriteLine(sendStr);
                                    sw.Flush();
                                    if (clients.Keys.Contains(account.username))
                                    {
                                        clients.Remove(account.username);
                                    }
                                    clients.Add(account.username, client);
                                    username = account.username;
                                }
                                else
                                {
                                    package.kind = 100;
                                    package.content = "Tên đăng nhập hoặc mật khẩu không đúng!";
                                    string sendStr = JsonSerializer.Serialize<Package>(package);
                                    sw.WriteLine(sendStr);
                                    sw.Flush();
                                }
                            }
                            else
                            {
                                package.kind = 100;
                                package.content = "Tên đăng nhập hoặc mật khẩu không đúng!";
                                string sendStr = JsonSerializer.Serialize<Package>(package);
                                sw.WriteLine(sendStr);
                                sw.Flush();
                            }
                            break;
                        #endregion
                        #region 203
                        case 203:
                            username = package.content;
                            clients.Remove(username);
                            friends = DB.GetFriends(username);
                            foreach (Account friendOnline in friends)
                            {
                                if (clients.Keys.Contains(friendOnline.username))
                                {
                                    friendOnline.online = true;
                                    StreamWriter swFriend = new StreamWriter(clients[friendOnline.username].GetStream());
                                    Online online = new Online() { index = username, online = false };
                                    ContentPackage.Package onlinePackage = new ContentPackage.Package() { kind = 107, content = JsonSerializer.Serialize<Online>(online) };
                                    swFriend.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(onlinePackage));
                                    swFriend.Flush();
                                }
                            }
                            groupChats = DB.getGroupChats(username);
                            foreach (GroupChat groupChat in groupChats)
                            {
                                checkGroupOffline(groupChat, username);
                            }
                            break;
                        #endregion
                        #region 204
                        case 204:
                            Message message = JsonSerializer.Deserialize<Message>(package.content);
                            Regex regex = new Regex(@"^[0-9]+");
                            Match match = regex.Match(message.receiver);
                            if (match.Success)
                            {
                                foreach (string user in DB.getUsersInGroup(Convert.ToInt32(message.receiver), message.sender))
                                {
                                    if (clients.Keys.Contains(user))
                                    {
                                        Message resendMessage = new Message() { sender = message.receiver, content = message.content, receiver = user };
                                        package.content = JsonSerializer.Serialize<Message>(resendMessage);
                                        TcpClient receiver = clients[user];
                                        sw = new StreamWriter(receiver.GetStream());
                                        sw.WriteLine(JsonSerializer.Serialize<Package>(package));
                                        sw.Flush();
                                    }
                                    else
                                    {
                                        Message resendMessage = new Message() { sender = message.receiver, content = message.content, receiver = user };
                                        DB.addWaitingMessage(resendMessage);
                                    }
                                }
                            }
                            else
                            {
                                if (clients.Keys.Contains(message.receiver))
                                {
                                    TcpClient receiver = clients[message.receiver];
                                    sw = new StreamWriter(receiver.GetStream());
                                    sw.WriteLine(receiveStr);
                                    sw.Flush();
                                }
                                else
                                {
                                    DB.addWaitingMessage(message);
                                }
                            }
                            break;
                        #endregion
                        #region 205
                        case 205:
                            InGroup inGroup = JsonSerializer.Deserialize<InGroup>(package.content);
                            sw = new StreamWriter(client.GetStream());
                            if(DB.CreateGroupChat(inGroup))
                            {
                                int count = 0;
                                foreach (string member in inGroup.Members)
                                {
                                    if (clients.Keys.Contains(member))
                                    {
                                        count++;
                                        if (count == 2)
                                        {
                                            inGroup.GChat.online = true;
                                            break;
                                        }
                                    }
                                }
                                package.kind = 104;
                                package.content = JsonSerializer.Serialize<GroupChat>(inGroup.GChat);
                                foreach (string member in inGroup.Members)
                                {
                                    if (clients.Keys.Contains(member))
                                    {
                                        sw = new StreamWriter(clients[member].GetStream());
                                        sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                                        sw.Flush();
                                    }
                                }
                            }
                            else
                            {
                                package.kind = 103;
                                package.content = "Tạo nhóm không thành công!";
                                sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                                sw.Flush();
                            }
                            break;
                        #endregion
                        #region 206
                        case 206:
                            Search search = JsonSerializer.Deserialize<Search>(package.content);
                            List<Account> users = DB.searchUsers(search);
                            sw = new StreamWriter(client.GetStream());
                            package.kind = 105;
                            package.content = JsonSerializer.Serialize<List<Account>>(users);
                            sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                            sw.Flush();
                            break;
                        #endregion
                        #region 207
                        case 207:
                            message = JsonSerializer.Deserialize<Message>(package.content);
                            Account sender = DB.addFriendRequest(message);
                            if (sender != null)
                            {
                                if (clients.Keys.Contains(message.receiver))
                                {
                                    package.content = JsonSerializer.Serialize<Account>(sender);
                                    sw = new StreamWriter(clients[message.receiver].GetStream());
                                    sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                                    sw.Flush();
                                }
                            }
                            break;
                        #endregion
                        #region 208
                        case 208:
                            message = JsonSerializer.Deserialize<Message>(package.content);
                            Account? friend = DB.acceptFriendRequest(message);
                            if (friend != null)
                            {
                                if (clients.Keys.Contains(friend.username))
                                {
                                    friend.online = true;
                                }
                                List<Account> friendRequests = DB.getFriendRequests(message.sender);
                                Accepted accepted = new Accepted() { Friend = friend, FriendRequests = friendRequests};
                                package.kind = 106;
                                package.content = JsonSerializer.Serialize<Accepted>(accepted);
                                sw = new StreamWriter(client.GetStream());
                                sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                                sw.Flush();
                                if (clients.Keys.Contains(message.receiver))
                                {
                                    account = DB.GetAccountByUserName(message.sender);
                                    account.online = true;
                                    sw = new StreamWriter(clients[message.receiver].GetStream());
                                    package.kind = 108;
                                    package.content = JsonSerializer.Serialize<Account>(account);
                                    sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                                    sw.Flush();
                                }
                            }
                            break;
                        #endregion
                        #region 209
                        case 209:
                            message = message = JsonSerializer.Deserialize<Message>(package.content);
                            DB.denyFriendRequest(message);
                            List<Account> friendRequestsDeny = DB.getFriendRequests(message.sender);
                            Accepted acceptedDeny = new Accepted() { Friend = null, FriendRequests = friendRequestsDeny };
                            package.kind = 106;
                            package.content = JsonSerializer.Serialize<Accepted>(acceptedDeny);
                            sw = new StreamWriter(client.GetStream());
                            sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                            sw.Flush();
                            break;
                        #endregion
                        #region 210
                        case 210:
                            message = JsonSerializer.Deserialize<Message>(package.content);
                            DB.cancelRequest(message);
                            if (clients.Keys.Contains(message.receiver))
                            {
                                List<Account> friendRequests = DB.getFriendRequests(message.receiver);
                                package.content = JsonSerializer.Serialize<List<Account>>(friendRequests);
                                sw = new StreamWriter(clients[message.receiver].GetStream());
                                sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                                sw.Flush();
                            }
                            break;
                        #endregion
                        #region 211
                        case 211:
                            message = JsonSerializer.Deserialize<Message>(package.content);
                            DB.cancelFriend(message);
                            friends = DB.GetFriends(message.sender);
                            groupChats = DB.getGroupChats(message.sender);
                            ContentPackage.CancelFriend cancelFriend = new ContentPackage.CancelFriend() { Friends = friends, GroupChats = groupChats };
                            package.content = JsonSerializer.Serialize<ContentPackage.CancelFriend>(cancelFriend);
                            sw = new StreamWriter(client.GetStream());
                            sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                            sw.Flush();
                            if (clients.Keys.Contains(message.receiver))
                            {
                                friends = DB.GetFriends(message.receiver);
                                groupChats = DB.getGroupChats(message.receiver);
                                cancelFriend = new ContentPackage.CancelFriend() { Friends = friends, GroupChats = groupChats };
                                package.content = JsonSerializer.Serialize<ContentPackage.CancelFriend>(cancelFriend);
                                sw = new StreamWriter(clients[message.receiver].GetStream());
                                sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                                sw.Flush();
                            }
                            break;
                            #endregion
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {

    }
    clients.Remove(username);
    sr.Close();
    client.Close();
}

string getLocalIP()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach(IPAddress ip in host.AddressList)
    {
        if(ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return ip.ToString();
        }
    }
    throw new Exception("Không tìm thấy địa chỉ IP!");
}

byte[] cleanByteArray(byte[] data)
{
    int c = 0;
    foreach(var b in data)
    {
        if(b != 0)
        {
            c++;
        }
    }
    byte[] temp = new byte[c];
    for(int i = 0; i < c; i++)
    {
        temp[i] = data[i];  
    }
    return temp;
}

void checkGroupOnline(GroupChat groupChat, string username)
{
    foreach (string userInGroup in DB.getUsersInGroup(groupChat.Id, username))
    {
        if (clients.Keys.Contains(userInGroup))
        {
            groupChat.online = true;
            Online online = new Online() { index = groupChat.Id.ToString(), online = true };
            ContentPackage.Package package = new ContentPackage.Package() { kind = 107, content=JsonSerializer.Serialize<Online>(online)};
            StreamWriter sw = new StreamWriter(clients[userInGroup].GetStream());
            sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
            sw.Flush();
        }
    }
}

void checkGroupOffline(GroupChat groupChat, string username)
{
    int dem = 0;
    string target = "";
    foreach (string userInGroup in DB.getUsersInGroup(groupChat.Id, username))
    {
        if (clients.Keys.Contains(userInGroup))
        {
            dem++;
            target = userInGroup;
        }
    }
    if (dem == 1)
    {
        Online online = new Online() { index = groupChat.Id.ToString(), online = false };
        ContentPackage.Package package = new ContentPackage.Package() { kind = 107, content = JsonSerializer.Serialize<Online>(online) };
        StreamWriter sw = new StreamWriter(clients[target].GetStream());
        sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
        sw.Flush();
    }
}
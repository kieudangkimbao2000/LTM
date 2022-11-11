using ContentPackage;
using server;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Schema;

Console.InputEncoding = Encoding.UTF8;
IPEndPoint iep = new IPEndPoint(IPAddress.Parse(getLocalIP()), 2000);
TcpListener server = new TcpListener(iep);
server.Start();
Console.WriteLine("Waiting client to connect ...");
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
    Console.WriteLine("A client is connected.");
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
                    switch (package.kind)
                    {
                        #region 202
                        case 202:
                            Account account = JsonSerializer.Deserialize<Account>(package.content);
                            Account accountCheck = DB.GetAccountByUserName(account.username);
                            StreamWriter sw = new StreamWriter(client.GetStream());
                            if (accountCheck != null)
                            {
                                if (accountCheck.password == account.password)
                                {
                                    package.kind = 101;
                                    package.content = @"{\rtf1 Đăng nhập thành công. \par}";
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
                                    package.content = @"{\rtf1 Tên đăng nhập hoặc mật khẩu không đúng! \par}";
                                    string sendStr = JsonSerializer.Serialize<Package>(package);
                                    sw.WriteLine(sendStr);
                                    sw.Flush();
                                }
                            }
                            else
                            {
                                package.kind = 100;
                                package.content = @"{\rtf1 Tên đăng nhập hoặc mật khẩu không đúng! \par}";
                                string sendStr = JsonSerializer.Serialize<Package>(package);
                                sw.WriteLine(sendStr);
                                sw.Flush();
                            }
                            break;
                        #endregion
                        #region 203
                        case 203:
                            Account acc = JsonSerializer.Deserialize<Account>(package.content);
                            sw = new StreamWriter(client.GetStream());
                            Boolean insertResult = DB.UserSignUp(acc);
                            Console.WriteLine(insertResult);
                            if (insertResult)
                            {
                                package.kind = 102;
                                package.content = "Sign up successfully !";
                                string sendStr = JsonSerializer.Serialize<Package>(package);
                                sw.WriteLine(sendStr);
                                sw.Flush();
                            } 
                            else
                            {
                                package.kind = 103;
                                package.content = "Sign up unsuccessful !";
                                string sendStr = JsonSerializer.Serialize<Package>(package);
                                sw.WriteLine(sendStr);
                                sw.Flush();
                            }
                            break;
                        #endregion
                        #region 204
                        case 204:
                            Message message = JsonSerializer.Deserialize<Message>(package.content);
                            if (clients.Keys.Contains(message.receiver))
                            {
                                TcpClient receiver = clients[message.receiver];
                                sw = new StreamWriter(receiver.GetStream());
                                sw.WriteLine(receiveStr);
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
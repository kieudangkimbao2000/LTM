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
Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
server.Bind(iep);
server.Listen(10);
Console.WriteLine("Chờ client kết nối ...");
bool active = true;
Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
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
    Socket client = server.Accept();
    Console.WriteLine("Có client kết nối.");
    Thread thread1 = new Thread(() => { clientRecieve(client); });
    thread1.IsBackground = true;
    thread1.Start();
}
server.Close();

void clientRecieve(Socket client)
{
    byte[] data = new byte[1024];
    int recv = client.Receive(data);
    data = cleanByteArray(data);
    Package? package = JsonSerializer.Deserialize<Package>(data);
    bool flag = true;
    if(package != null)
    {
        if(package.content != null)
        {
            switch(package.kind)
            {
                case 202:
                    Account account = JsonSerializer.Deserialize<Account>(package.content);
                    Account accountCheck = DB.GetAccountByUserName(account.username);
                    if (accountCheck != null)
                    {
                        if (accountCheck.password == account.password)
                        {
                            data = new byte[1024];
                            package.kind = 101;
                            package.content = "Đăng nhập thành công.";
                            data = JsonSerializer.SerializeToUtf8Bytes<Package>(package);
                            client.Send(data);
                            clients.Add(account.username, client);
                        }
                        else
                        {
                            data = new byte[1024];
                            package.kind = 100;
                            package.content = "Tên đăng nhập hoặc mật khẩu không đúng!!";
                            data = JsonSerializer.SerializeToUtf8Bytes<Package>(package);
                            client.Send(data);
                            flag = false;
                        }
                    }
                    else
                    {
                        data = new byte[1024];
                        package.kind = 100;
                        package.content = "Tên đăng nhập hoặc mật khẩu không đúng!";
                        data = JsonSerializer.SerializeToUtf8Bytes<Package>(package);
                        client.Send(data);
                        flag = false;
                    }
                    break;
            }
        }
    }
    while(flag)
    {
        try
        {
            data = new byte[1024];
            recv = client.Receive(data);
            if (recv == 0) break;
            data = cleanByteArray(data);
            package = JsonSerializer.Deserialize<Package>(data);
            if (package != null)
            {
                if (package.content != null)
                {
                    switch (package.kind)
                    {
                        case 204:
                            
                            break;
                    }
                }
            }
        }catch(Exception ex)
        {
            break;
        }
    }
    client.Shutdown(SocketShutdown.Both);
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
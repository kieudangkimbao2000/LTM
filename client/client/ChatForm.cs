using ContentPackage;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text.Json;

namespace client
{
    public partial class ChatForm : Form
    {
        IPEndPoint ipe;
        Socket client;
        public ChatForm()
        {
            InitializeComponent();
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            ipe = new IPEndPoint(IPAddress.Parse(getLocalIP()), 2000);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ipe);
            Account account = new Account { username = "client1", password="123"};
            string jsString = JsonSerializer.Serialize<Account>(account);
            Package package = new Package { kind=0, content=jsString};
            client.Send(JsonSerializer.SerializeToUtf8Bytes<Package>(package));

        }

        string getLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Khong tim thay IP!");
        }
    }
}
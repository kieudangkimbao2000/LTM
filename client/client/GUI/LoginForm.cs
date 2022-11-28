using ContentPackage;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace client.GUI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {

            InitializeComponent();
            Thread thread = new Thread(() => { clientRecive(); });
            thread.IsBackground = true;
            thread.Start();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            TcpClient client = new TcpClient();
            client.Connect(iep);
            Account account = new Account();
            account.username = tbUser.Text;
            account.password = tbPwd.Text;
            Package package = new Package() { kind = 202, content = JsonSerializer.Serialize<Account>(account) };
            string sendStr = JsonSerializer.Serialize<Package>(package);
            Program.sw.WriteLine(sendStr);
            Program.sw.Flush();

        }
        private void clientRecive()
        {
            Package? package = new Package();
            while (true)
            {
                string recvStr = Program.sr.ReadLine();
                package = JsonSerializer.Deserialize<Package>(recvStr);
                if (package != null)
                {
                    if (package.content != null)
                    {
                        switch (package.kind)
                        {
                            #region 100
                            case 100:
                                MessageBox.Show("Đăng nhập thất bại");
                                break;
                            #endregion
                            #region 101
                            case 101:
                                MessageBox.Show("Đăng nhập thành công");
                                ChatForm chat = new ChatForm();
                                chat.username = tbUser;
                                chat.ShowDialog();
                                break;
                            #endregion
                        }
                    }
                }
            }
        }
    }
}

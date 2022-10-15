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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace client.GUI
{
    public partial class ChatForm : Form
    {
        delegate void setContentCallback(string content);
        string username = "";
        public ChatForm()
        {
            InitializeComponent();
            this.dgvContent.Columns.Add("Reciver", "Reciver");
            this.dgvContent.Columns.Add("You", "You");
            this.dgvContent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvContent.RowHeadersVisible = false;
            this.dgvContent.AllowUserToAddRows = false;
            this.dgvContent.ColumnHeadersVisible = false;
            Thread thread = new Thread(() => { clientRecive(); });
            thread.IsBackground = true;
            thread.Start();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.username = tbUsername.Text;
            account.password = tbPassword.Text;
            Package package = new Package() { kind = 202, content = JsonSerializer.Serialize<Account>(account) };
            byte[] data = new byte[1024];
            data = JsonSerializer.SerializeToUtf8Bytes<Package>(package);
            Program.client.Send(data);
        }

        private void clientRecive()
        {
            byte[] data = new byte[1024];
            Package? package = new Package();
            while (true)
            {
                data = new byte[1024];
                int recv = Program.client.Receive(data);
                if (recv == 0) break;
                data = Program.cleanByteArray(data);
                package = JsonSerializer.Deserialize<Package>(data);
                if(package != null)
                {
                    if(package.content != null)
                    {
                        switch (package.kind)
                        {
                            #region 100
                            case 100:
                                setContent(package.content);
                                break;
                            #endregion
                            #region 101
                            case 101:
                                setContent(package.content);
                                username = tbUsername.Text;
                                break;
                            #endregion
                            #region 204
                            case 204:
                                ContentPackage.Message message = JsonSerializer.Deserialize<ContentPackage.Message>(package.content);
                                setContent(message.content);
                                break;
                            #endregion
                        }
                    }
                }
            }
        }

        private void setContent(string content)
        {
            if (this.dgvContent.InvokeRequired)
            {
                setContentCallback d = new setContentCallback(setContent);
                this.Invoke(d, new object[] { content });
            }
            else
            {
                this.dgvContent.Rows.Add(content, "");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (username != "")
            {
                if (string.IsNullOrEmpty(this.tbReciver.Text))
                {
                    MessageBox.Show("Chưa có người nhận!");
                }
                else
                {
                    string reciver = this.tbReciver.Text;
                    string content = this.tbSend.Text;
                    ContentPackage.Message message = new ContentPackage.Message() { sender=username, content=content, receiver=reciver};
                    string jsonStr = JsonSerializer.Serialize<ContentPackage.Message>(message);
                    Package package = new Package() { kind=204, content=jsonStr };
                    byte[] data = new byte[1024];
                    data = JsonSerializer.SerializeToUtf8Bytes<Package>(package);
                    Program.client.Send(data);
                    this.dgvContent.Rows.Add("", content);
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
            }
        }
    }
}

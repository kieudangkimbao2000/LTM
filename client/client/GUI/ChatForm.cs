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

namespace client.GUI
{
    public partial class ChatForm : Form
    {
        delegate void setContentCallback(string content);
        string username = "";
        int zIndexLvIcon = 0;
        public ChatForm()
        {
            InitializeComponent();
            zIndexLvIcon = this.Controls.GetChildIndex(this.dgvIcon);
            this.dgvIcon.ColumnHeadersVisible = false;
            this.dgvIcon.RowHeadersVisible = false;
            for (int i = 0; i < 5; i++)
            {
                DataGridViewImageColumn col = new DataGridViewImageColumn();
                col.Width = 30;
                this.dgvIcon.Columns.Add(col);
            }
            this.dgvIcon.Rows[0].Height = 30;
            foreach (DataGridViewImageCell cell in this.dgvIcon.Rows[0].Cells)
            {
                cell.Value = null;

            }
            InitialListIcon();
            Thread thread = new Thread(() => { clientRecive(); });
            thread.IsBackground = true;
            thread.Start();
        }

        private void clientRecive()
        {
            Package? package = new Package();
            while (true)
            { 
                string recvStr = Program.sr.ReadLine();
                package = JsonSerializer.Deserialize<Package>(recvStr);
                if(package != null)
                {
                    if(package.content != null)
                    {
                        switch (package.kind)
                        {
                            #region 100
                            case 100:
                                setReceivedContent(package.content);
                                break;
                            #endregion
                            #region 101
                            case 101:
                                setReceivedContent(package.content);
                                username = tbUsername.Text;
                                break;
                            #endregion
                            #region 204
                            case 204:
                                ContentPackage.Message message = JsonSerializer.Deserialize<ContentPackage.Message>(package.content);
                                setReceivedContent(message.content);
                                break;
                            #endregion
                        }
                    }
                }
            }
        }

        private void setSendedContent(string content)
        {
            this.rtbContent.Select(this.rtbContent.Text.Length, 0);
            content = content.Replace(@"\pard", @"\pard\qr");
            this.rtbContent.SelectedRtf = content;

        }

        private void setReceivedContent(string content)
        {
            if (this.rtbContent.InvokeRequired)
            {
                setContentCallback d = new setContentCallback(setReceivedContent);
                this.Invoke(d, new object[] {content});
            }
            else
            {
                this.rtbContent.SelectedRtf = content;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (username != "")
            {
                if (string.IsNullOrEmpty(this.tbReceiver.Text))
                {
                    MessageBox.Show("Chưa có người nhận!");
                }
                else
                {
                    string reciver = this.tbReceiver.Text;
                    string content = this.rtbSend.Rtf;
                    this.rtbSend.Rtf = null;
                    ContentPackage.Message message = new ContentPackage.Message() { sender=username, content=content, receiver=reciver};
                    string jsonStr = JsonSerializer.Serialize<ContentPackage.Message>(message);
                    Package package = new Package() { kind=204, content=jsonStr };
                    string sendStr = JsonSerializer.Serialize<Package>(package);
                    Program.sw.WriteLine(sendStr);
                    Program.sw.Flush();
                    setSendedContent(content);
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa đăng nhập!");
            }
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.username = tbUsername.Text;
            account.password = tbPassword.Text;
            Package package = new Package() { kind = 202, content = JsonSerializer.Serialize<Account>(account) };
            string sendStr = JsonSerializer.Serialize<Package>(package);
            Program.sw.WriteLine(sendStr);
            Program.sw.Flush();
        }

        private string insertImage(Image image)
        {
            StringBuilder rtf = new StringBuilder();

            rtf.Append(@"{\rtf1");
            rtf.Append(@"{\pict\pngblip\picw400\pich400\picwgoal200\pichgoal200 ");

            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bytes = ms.ToArray();
            }
            for (int i = 0; i < bytes.Length; i++)
            {
                rtf.Append(String.Format("{0:X2}", bytes[i]));
            }

            rtf.Append(@"}");
            rtf.Append(@"}");

            return rtf.ToString();
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            this.dgvIcon.BringToFront();
        }

        private void btnIcon_Leave(object sender, EventArgs e)
        {
            this.Controls.SetChildIndex(this.dgvIcon, zIndexLvIcon);
        }

        private void InitialListIcon()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            string iconFolder = di.Parent.Parent.Parent.FullName + @"\icon";
            int i = 0;
            foreach (var file in Directory.EnumerateFiles(iconFolder, "*.png"))
            {
                DataGridViewRow row = this.dgvIcon.Rows[0];
                if (row.Cells[4].Value != null)
                {
                    row = new DataGridViewRow();
                    row.Height = 30;
                    this.dgvIcon.Rows.Add(row);
                }
                foreach (DataGridViewImageCell cell in this.dgvIcon.Rows[0].Cells)
                {
                    if (cell.Value == null)
                    {
                        Image imgFromFile = Image.FromFile(file);
                        cell.Value = new Bitmap(cell.Size.Width, cell.Size.Height);
                        using (Graphics gp = Graphics.FromImage((Bitmap)cell.Value))
                        {
                            gp.DrawImage(imgFromFile, new Rectangle(Point.Empty, cell.Size));
                        }
                        break;
                    }
                }
                i++;
            }
        }

        private void dgvIcon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Image img = (Image)this.dgvIcon.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            this.rtbSend.SelectedRtf = insertImage(img);
        }

        private void rtbContent_Enter(object sender, EventArgs e)
        {

        }

        private void rtbContent_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}

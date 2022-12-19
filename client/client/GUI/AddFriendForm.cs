using ContentPackage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client.GUI
{
    public partial class AddFriendForm : Form
    {
        public Account account;
        public AddFriendForm()
        {
            InitializeComponent();
        }

        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            Search search = new Search() { username = account.username, search = this.tbSearch.Text };
            ContentPackage.Package package = new ContentPackage.Package() { kind = 206, content = JsonSerializer.Serialize<Search>(search) };
            string sendedStr = JsonSerializer.Serialize<ContentPackage.Package>(package);
            Program.sw.WriteLine(sendedStr);
            Program.sw.Flush();
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                DataGridViewRow row = this.dgvUsers.Rows[e.RowIndex];
                string username = row.Tag.ToString();
                ContentPackage.Message mess = new ContentPackage.Message() { sender = this.account.username, content = "", receiver = username };
                if (Convert.ToInt32(row.Cells[e.ColumnIndex].Tag) == 0)
                {
                    ContentPackage.Package package = new ContentPackage.Package() { kind = 207, content = JsonSerializer.Serialize<ContentPackage.Message>(mess) };
                    Program.sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                    Program.sw.Flush();
                    row.Cells[e.ColumnIndex].Value = "Cancel requested";
                    row.Cells[e.ColumnIndex].Tag = 1;
                }
                else if (Convert.ToInt32(row.Cells[e.ColumnIndex].Tag) == 1)
                {
                    ContentPackage.Package package = new ContentPackage.Package() { kind = 210, content = JsonSerializer.Serialize<ContentPackage.Message>(mess) };
                    Program.sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                    Program.sw.Flush();
                    row.Cells[e.ColumnIndex].Value = "Request";
                    row.Cells[e.ColumnIndex].Tag = 0;
                }
                else if (Convert.ToInt32(row.Cells[e.ColumnIndex].Tag) == 2)
                {
                    ContentPackage.Package package = new ContentPackage.Package() { kind = 211, content = JsonSerializer.Serialize<ContentPackage.Message>(mess) };
                    Program.sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                    Program.sw.Flush();
                    row.Cells[e.ColumnIndex].Value = "Request";
                    row.Cells[e.ColumnIndex].Tag = 0;
                }
            }
        }

        private void dgvFrReq_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                string receiver = this.dgvFrReq.Rows[e.RowIndex].Tag.ToString();
                ContentPackage.Message message = new ContentPackage.Message() { sender = this.account.username, content = "", receiver = receiver };
                ContentPackage.Package package = new ContentPackage.Package() { kind = 208, content = JsonSerializer.Serialize<ContentPackage.Message>(message)};
                Program.sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                Program.sw.Flush();
                this.dgvFrReq.Rows.RemoveAt(e.RowIndex);
            }
            else if (e.ColumnIndex == 2)
            {
                string receiver = this.dgvFrReq.Rows[e.RowIndex].Tag.ToString();
                ContentPackage.Message message = new ContentPackage.Message() { sender = this.account.username, content = "", receiver = receiver };
                ContentPackage.Package package = new ContentPackage.Package() { kind = 209, content = JsonSerializer.Serialize<ContentPackage.Message>(message) };
                Program.sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                Program.sw.Flush();
                this.dgvFrReq.Rows.RemoveAt(e.RowIndex);
            }
        }
    }
}

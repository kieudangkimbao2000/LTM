using ContentPackage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client.GUI
{
    public partial class GroupForm : Form
    {
        public string username;
        public GroupForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.tbSearch.Text != "")
            {
                foreach (DataGridViewRow row in this.dgvFriends.Rows)
                {
                    row.Visible = false;
                    if (row.Tag.ToString().Contains(this.tbSearch.Text) || row.Cells[0].Value.ToString().Contains(this.tbSearch.Text))
                    {
                        row.Visible = true;
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow row in this.dgvFriends.Rows)
                {
                    row.Visible = true;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.tbGroupName.Text == "")
            {
                MessageBox.Show("Chưa có tên nhóm!");
                return;
            }
            GroupChat groupChat = new GroupChat() {
                Id = 0,
                GName = this.tbGroupName.Text,
                Admin = this.username
            };
            List<string> members = new List<string>();
            members.Add(this.username);
            foreach (DataGridViewRow row in this.dgvFriends.Rows)
            {
                if (Convert.ToBoolean(row.Cells[1].Value))
                {
                    members.Add(row.Tag.ToString());
                }
            }
            InGroup inGroup = new InGroup(){
                GChat = groupChat,
                Members = members
            };
            ContentPackage.Package package = new ContentPackage.Package() {
                kind = 205,
                content = JsonSerializer.Serialize<InGroup>(inGroup)
            };
            string sendStr = JsonSerializer.Serialize<ContentPackage.Package>(package);
            Program.sw.WriteLine(sendStr);
            Program.sw.Flush();
        }
    }
}

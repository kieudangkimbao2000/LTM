using ContentPackage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace client.GUI
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
            Thread thread = new Thread(() => { clientRecive(); });
            thread.IsBackground = true;
            thread.Start();
        }

        private void btn_signUp_Click(object sender, EventArgs e)
        {
            Account acc = new Account();
            if(tb_confirmPassword.Text.Equals(tb_password.Text))
            {
                acc.username = tb_userName.Text;
                acc.password = tb_password.Text;
                Package package = new Package() { kind = 203, content = JsonSerializer.Serialize<Account>(acc) };
                string sendStr = JsonSerializer.Serialize<Package>(package);
                Program.sw.WriteLine(sendStr);
                Program.sw.Flush();
            }
            else
            {
                richTB_alert.Text = "Two passwords must be the same";
                richTB_alert.ForeColor = System.Drawing.Color.Red;
            }
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
                            #region 102
                            case 102:
                                flag = true;
                                message = package.content;
                                richTB_alert.Text = message;
                                richTB_alert.ForeColor = System.Drawing.Color.LimeGreen;
                                break;
                            #endregion
                            #region 103
                            case 103:
                                flag = false;
                                message = package.content;
                                richTB_alert.ForeColor = System.Drawing.Color.Red;
                                break;
                            #endregion
                        }
                    }
                }
            }
        }
    }
}

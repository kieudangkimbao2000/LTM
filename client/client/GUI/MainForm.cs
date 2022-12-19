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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Xml;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace client.GUI
{
    public partial class MainForm : Form
    {
        AddFriendForm addFriendForm;
        GroupForm groupForm;
        Account? account;
        List<string> fileNames = new List<string>();
        List<Account> friendRequests = new List<Account>();
        List<Account> friends = new List<Account>();
        string receiver = "";
        int zIndexLvIcon = 0;
        int zIndexMenu = 0;

        int trleft = 7640;
        int widthCell = 11640;
        string currentPanel = "LOGIN";
        
        string contentReceiverPattern = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\fswiss\fprq2\fcharset0 Calibri;}{\f1\froman\fprq2\fcharset0 Times New Roman;}{\f2\fnil Segoe UI;}}{\colortbl ;\red231\green230\blue230;\red0\green255\blue255;\red180\green198\blue231;}{\*\generator Riched20 10.0.22621}\viewkind4\uc1 \pard\widctlpar\sa160\sl252\slmult1\f0\fs22 NameRTF\par\trowd\trgaph108\trleft500\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3\clcbpat3\cellx5000 \pard\intbl\widctlpar\fs26 ContentRTF\par\cf1\fs16 TimeRTF\cf0\fs22\cell\row \pard\f2\fs18\par}";
        string contentSenderPattern = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\froman\fprq2\fcharset0 Times New Roman;}{\f1\fswiss\fprq2\fcharset0 Calibri;}{\f2\fnil Segoe UI;}}{\colortbl ;\red231\green230\blue230;\red0\green255\blue255;\red180\green198\blue231;}{\*\generator Riched20 10.0.22621}\viewkind4\uc1\trowd\trgaph108\trleft<trleftsize>\trpaddl108\trpaddr108\trpaddfl3\trpaddfr3\clcbpat3\cellx<cellsize> \pard\intbl\widctlpar\qr\f1\fs26 ContentRTF\par\cf1\fs16 TimeRTF\cell\row \pard\cf0\f2\fs18\par}";

        string patternFirstWord = "^[A-Za-z]";
        string patternSpecialWords = "[~!@#$%^&*\\(\\)_+{}\\|:\"<>?`\\-=\\[\\]\\\\;',./]";

        delegate void changePanelCallback(string panelName);
        delegate void setContentCallback(string content);
        delegate void setNoticeCallback(string username);
        delegate void addChattedObjToListCallback(List<Account> friends, List<GroupChat> groupChats);
        delegate void setNameAccountCallback();
        delegate void setRegisterFailedCallback();
        delegate void setLoginFailedCallback();
        delegate void setListUsersCallback(List<Account> users);
        delegate void addFriendNoticeCallback(bool notice);
        delegate void addNewFriendToListCallback(Account friend);
        delegate void setOnlineCallback(Online online);
        delegate void removeObjectFromListChatCallback(string index);
        delegate void removeFriendRequestCallback(string index);
        delegate void setFriendRequestsCallback();
        delegate void setNewGroupCallback(GroupChat groupChat);
        delegate void closeGroupFormCallback();

        public MainForm()
        {
            InitializeComponent();
            this.widthCell = this.rtbContent.Width * 15 - 500;
            this.trleft = this.widthCell - 4000;
            zIndexLvIcon = this.pnlChat.Controls.GetChildIndex(this.dgvIcon);
            zIndexMenu = this.pnlLeftChat.Controls.GetChildIndex(this.lstbxMenu);
            this.dgvIcon.ColumnHeadersVisible = false;
            this.dgvIcon.RowHeadersVisible = false;
            this.dgvIcon.AllowUserToResizeRows = false;
            this.dgvIcon.AllowUserToResizeColumns = false;
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
            this.dgvFriends.RowHeadersVisible = false;
            this.dgvFriends.ColumnHeadersVisible = false;
            this.dgvFriends.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            this.dgvFriends.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            Thread thread = new Thread(() => { clientReceive(); });
            thread.IsBackground = true;
            thread.Start();
        }

        private void clientReceive()
        {
            ContentPackage.Package? package = new ContentPackage.Package();
            while (true)
            {
                string recvStr = Program.sr.ReadLine();
                package = JsonSerializer.Deserialize<ContentPackage.Package>(recvStr);
                if (package != null)
                {
                    if (package.content != null)
                    {
                        switch (package.kind)
                        {
                            #region 100
                            case 100:
                                setLoginFailed();
                                break;
                            #endregion
                            #region 101
                            case 101:
                                changePanel("CHAT");
                                LoginSuccess loginSuccess = JsonSerializer.Deserialize<LoginSuccess>(package.content);
                                this.account = loginSuccess.AccountLogin;
                                this.friends = loginSuccess.Friends;
                                this.friendRequests = loginSuccess.FriendRequests;
                                if (this.friendRequests.Count > 0)
                                {
                                    addFriendNotice(true);
                                }
                                addChattedObjToList(loginSuccess.Friends, loginSuccess.GroupChats);
                                if (loginSuccess.WaitingMessages != null)
                                {
                                    foreach (ContentPackage.Message waitingMessage in loginSuccess.WaitingMessages)
                                    {
                                        setReceivedMessage(waitingMessage);
                                        setNotice(waitingMessage.sender);
                                    }
                                }
                                setNameAccount();
                                break;
                            #endregion
                            #region 102
                            case 102:
                                setRegisterFailed();
                                break;
                            #endregion
                            #region 103
                            case 103:
                                MessageBox.Show(package.content);
                                break;
                            #endregion
                            #region 104
                            case 104:
                                GroupChat? groupChat = JsonSerializer.Deserialize<GroupChat>(package.content);
                                if (groupChat != null)
                                {
                                    setNewGroup(groupChat);
                                }
                                closeGroupForm();
                                break;
                            #endregion
                            #region 105
                            case 105:
                                List<Account> users = JsonSerializer.Deserialize<List<Account>>(package.content);
                                setListUsers(users);
                                break;
                            #endregion
                            #region 106
                            case 106:
                                Accepted accepted = JsonSerializer.Deserialize<Accepted>(package.content);
                                this.friendRequests = accepted.FriendRequests;
                                if (this.addFriendForm != null)
                                {
                                    setFriendRequests();
                                }
                                if (accepted.Friend != null)
                                {
                                    this.friends.Add(accepted.Friend);
                                    addNewFriendToList(accepted.Friend);
                                }
                                if (this.friendRequests.Count <= 0)
                                {
                                    addFriendNotice(false);
                                }
                                break;
                            #endregion
                            #region 107
                            case 107:
                                Online online = JsonSerializer.Deserialize<Online>(package.content);
                                setOnline(online);
                                break;
                            #endregion
                            #region 108
                            case 108:
                                Account friend = JsonSerializer.Deserialize<Account>(package.content);
                                this.friends.Add(friend);
                                addNewFriendToList(friend);
                                break;
                            #endregion

                            #region 204
                            case 204:
                                ContentPackage.Message message = JsonSerializer.Deserialize<ContentPackage.Message>(package.content);
                                setReceivedMessage(message);
                                break;
                            #endregion
                            #region 207
                            case 207:
                                Account user = JsonSerializer.Deserialize<Account>(package.content);
                                if (user != null)
                                {
                                    this.friendRequests.Add(user);
                                    if (this.addFriendForm != null)
                                    {
                                        setFriendRequests();
                                    }
                                    addFriendNotice(true);
                                }
                                break;
                            #endregion
                            #region 210
                            case 210:
                                this.friendRequests = JsonSerializer.Deserialize<List<Account>>(package.content);
                                if (this.addFriendForm != null)
                                {
                                    setFriendRequests();
                                }
                                if (this.friendRequests.Count <= 0)
                                {
                                    addFriendNotice(false);
                                }
                                break;
                            #endregion
                            #region 211
                            case 211:
                                ContentPackage.CancelFriend cancelFriend = JsonSerializer.Deserialize<ContentPackage.CancelFriend>(package.content);
                                this.friends = cancelFriend.Friends;
                                addChattedObjToList(cancelFriend.Friends, cancelFriend.GroupChats);
                                break;
                            #endregion
                        }
                    }
                }
            }
        }

        private string createImageRTF(Image image, bool icon = false)
        {
            int w = image.Width * 8;
            int h = image.Height * 8;
            
            StringBuilder rtf = new StringBuilder();
            if (icon)
            {
                w = 300;
                h = 300;
                rtf.Append(@"{\rtf1");
            }
            rtf.Append(@"{\pict\pngblip\picw" + w + @"\pich" + h + @"\picwgoal" + w + @"\pichgoal" + h + @" ");

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
            if (icon)
            {
                rtf.Append(@"}");
            }

            return rtf.ToString();
        }

        private void changePanel(string paneName)
        {
            if (this.InvokeRequired)
            {
                changePanelCallback d = new changePanelCallback(changePanel);
                this.Invoke(d, new object[] { paneName });
            }
            else
            {
                this.Controls.Clear();
                switch (paneName.ToUpper())
                {
                    case "LOGIN":
                        this.Text = "Login";
                        this.ClientSize = new System.Drawing.Size(355, 331);
                        this.Controls.Add(this.pnlLogin);
                        toCenter();
                        this.FormBorderStyle = FormBorderStyle.FixedSingle;
                        this.MaximizeBox = false;
                        this.MinimizeBox = false;
                        this.account = null;
                        this.fileNames.Clear();
                        this.receiver = "";
                        this.trleft = 7640;
                        this.widthCell = 11640;
                        this.dgvFriends.Rows.Clear();
                        this.rtbContent.Rtf = "";
                        this.rtbSend.Rtf = "";
                        this.tbSearch.Text = "";
                        this.lbReceiver.Text = "Receiver";
                        this.lbFileName.Text = "";
                        this.tbNameReg.Text = "";
                        this.lbNameReg.Text = "";
                        this.tbAccReg.Text = "";
                        this.lbAccReg.Text = "";
                        this.tbPassReg.Text = "";
                        this.lbPassReg.Text = "";
                        this.tbRePassReg.Text = "";
                        this.lbRePassReg.Text = "";
                        this.friends.Clear();
                        this.friendRequests.Clear();
                        this.account = null;
                        break;
                    case "CHAT":
                        this.Text = "Chat";
                        this.ClientSize = new Size(990, 543);
                        this.Controls.Add(this.pnlChat);
                        toCenter();
                        this.widthCell = this.rtbContent.Width * 15 - 500;
                        this.trleft = this.widthCell - 4000;
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                        this.MaximizeBox = true;
                        this.MinimizeBox = true;
                        this.tbUser.Text = "";
                        this.tbPwd.Text = "";
                        break;
                    case "REGISTER":
                        this.Text = "Register";
                        this.ClientSize = new System.Drawing.Size(355, 429);
                        this.Controls.Add(this.pnlRegister);
                        toCenter();
                        this.tbUser.Text = "";
                        this.tbPwd.Text = "";
                        break;
                }
                this.currentPanel = paneName;
            }
        }

        private void toCenter()
        {
            int x = Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2;
            int y = Screen.PrimaryScreen.Bounds.Height / 2 - this.Height / 2;
            this.Location = new Point(x, y);
        }

        private void setSendedContent(string content)
        {
            this.rtbContent.Select(this.rtbContent.TextLength, 0);
            string receiveContent = this.contentSenderPattern;
            receiveContent = receiveContent.Replace("ContentRTF", content);
            receiveContent = receiveContent.Replace("TimeRTF", DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt"));
            string saveFolder = Directory.GetCurrentDirectory() + "\\save\\" + this.account.username;
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            string chattedFile = saveFolder + "\\" + this.receiver + ".xml";
            if (File.Exists(chattedFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(chattedFile);
                XmlNode node = xmlDoc.GetElementsByTagName("Messages")[0].LastChild.Clone();
                node.InnerText = receiveContent;
                xmlDoc.GetElementsByTagName("Messages")[0].AppendChild(node);
                xmlDoc.Save(chattedFile);
            }
            else
            {
                using (XmlWriter writer = XmlWriter.Create(chattedFile))
                {
                    writer.WriteStartElement("Messages");
                    writer.WriteElementString("Message", receiveContent);
                    writer.WriteEndElement();
                }
            }
            //receiveContent = receiveContent.Replace(@"<trleftsize>", this.trleft.ToString()).Replace(@"<cell1size>", this.widthCell1.ToString()).Replace(@"<cell2size>", this.widthCell2.ToString());
            receiveContent = receiveContent.Replace(@"<trleftsize>", this.trleft.ToString()).Replace(@"<cellsize>", this.widthCell.ToString());
            this.rtbContent.SelectedRtf = receiveContent;
        }

        private void setReceivedContent(string content)
        {
            if (this.rtbContent.InvokeRequired)
            {
                setContentCallback d = new setContentCallback(setReceivedContent);
                this.Invoke(d, new object[] { content });
            }
            else
            {
                this.rtbContent.Select(this.rtbContent.TextLength, 0);
                this.rtbContent.SelectedRtf = content;
            }
        }

        private void setNotice(string username)
        {
            if (this.dgvFriends.InvokeRequired)
            {
                setNoticeCallback d = new setNoticeCallback(setNotice);
                this.Invoke(d, new object[] { username });
            }
            else
            {
                foreach (DataGridViewRow row in this.dgvFriends.Rows)
                {
                    if (row.Tag.ToString().Equals(username))
                    {
                        string imageFilePath = Directory.GetCurrentDirectory() + "\\pic\\notice.png";
                        row.Cells[3].Value = Image.FromFile(imageFilePath);
                        break;
                    }
                }
            }
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

        private void addChattedObjToList(List<Account> friends, List<GroupChat> groupChats)
        {
            if (this.dgvFriends.InvokeRequired)
            {
                addChattedObjToListCallback d = new addChattedObjToListCallback(addChattedObjToList);
                this.Invoke(d, new object[] { friends, groupChats });
            }
            else
            {
                this.dgvFriends.Rows.Clear();
                foreach (Account friend in friends)
                {
                    this.dgvFriends.Rows.Add();
                    int count = this.dgvFriends.Rows.Count;
                    DataGridViewRow row = this.dgvFriends.Rows[count - 1];
                    row.Tag = friend.username;
                    row.Cells[0].Value = friend.fullname;
                    row.Cells[1].Value = "U";
                    if (friend.online)
                    {
                        row.Cells[2].Value = "Online";
                        row.Cells[2].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        row.Cells[2].Value = "Offline";
                        row.Cells[2].Style.ForeColor = Color.Gray;
                    }
                    //row.DefaultCellStyle.BackColor = Color.FromArgb(212, 243, 255);
                }
                foreach (GroupChat groupChat in groupChats)
                {
                    this.dgvFriends.Rows.Add();
                    int count = this.dgvFriends.Rows.Count;
                    DataGridViewRow row = this.dgvFriends.Rows[count -1];
                    row.Tag = groupChat.Id;
                    row.Cells[0].Value = groupChat.GName;
                    row.Cells[1].Value = "G";
                    if (groupChat.online)
                    {
                        row.Cells[2].Value = "Online";
                        row.Cells[2].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        row.Cells[2].Value = "Offline";
                        row.Cells[2].Style.ForeColor = Color.Gray;
                    }
                    //row.DefaultCellStyle.BackColor = Color.FromArgb(212, 243, 255);
                }
                this.dgvFriends.ClearSelection();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = this.tbUser.Text;
            string password = this.tbPwd.Text;
            Regex regexFirstWord = new Regex(patternFirstWord);
            Regex regexSpecialWords = new Regex(patternSpecialWords);
            bool flag = false;
            #region username
            if (string.IsNullOrEmpty(username))
            {
                this.lbLogin.Text = "Chưa có tên tài khoản!";
                flag = true;
            }
            if (!regexFirstWord.IsMatch(username))
            {
                this.lbLogin.Text = "Ký tự đầu tên tài khoản phải là chữ cái!";
                flag = true;
            }
            if (regexSpecialWords.IsMatch(username))
            {
                this.lbLogin.Text = "Không chứa các ký tự ~!@#$%^&*()_+{}|:\"<>?`-=[]\\;',./";
                flag = true;
            }
            #endregion
            #region password
            if (string.IsNullOrEmpty(password))
            {
                this.lbLogin.Text = "Chưa có mật khẩu!";
                flag = true;
            }
            if (regexSpecialWords.IsMatch(password))
            {
                this.lbLogin.Text = "Không chứa các ký tự ~!@#$%^&*()_+{}|:\"<>?`-=[]\\;',./";
                flag = true;
            }
            #endregion
            if (flag)
            {
                return;
            }

            Account account = new Account();
            account.username = tbUser.Text;
            account.password = tbPwd.Text;
            Package package = new Package() { kind = 202, content = JsonSerializer.Serialize<Account>(account) };
            string sendStr = JsonSerializer.Serialize<Package>(package);
            Program.sw.WriteLine(sendStr);
            Program.sw.Flush();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.receiver))
            {
                MessageBox.Show("Chưa có người nhận!");
            }
            else
            {
                foreach (string fileName in fileNames)
                {
                    Image img = Image.FromFile(fileName);
                    string imgRTF = createImageRTF(img);
                    setSendedContent(imgRTF);
                    string sendImage = this.contentReceiverPattern;
                    sendImage = sendImage.Replace("NameRTF", this.account.fullname);
                    sendImage = sendImage.Replace("ContentRTF", imgRTF);
                    sendImage = sendImage.Replace("TimeRTF", DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt"));
                    ContentPackage.Message messageIamge = new ContentPackage.Message() { sender = this.account.username, content = sendImage, receiver = this.receiver };
                    string imageJsonStr = JsonSerializer.Serialize<ContentPackage.Message>(messageIamge);
                    ContentPackage.Package imagePackage = new ContentPackage.Package() { kind = 204, content = imageJsonStr };
                    string sendImageStr = JsonSerializer.Serialize<ContentPackage.Package>(imagePackage);
                    Program.sw.WriteLine(sendImageStr);
                    Program.sw.Flush();
                }
                fileNames.Clear();
                this.lbFileName.Text = "";
                if (this.rtbSend.Text != "")
                {
                    string content = this.rtbSend.Rtf.Substring(this.rtbSend.Rtf.IndexOf(@"\pard"));
                    content = content.Substring(0, content.LastIndexOf("}")).Replace(@"\pard", "").Replace(@"\par", "").Replace(@"\fs18", "");
                    this.rtbSend.Rtf = null;
                    setSendedContent(content);
                    string sendContent = this.contentReceiverPattern;
                    sendContent = sendContent.Replace("NameRTF", this.account.fullname);
                    sendContent = sendContent.Replace("ContentRTF", content);
                    sendContent = sendContent.Replace("TimeRTF", DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt"));
                    ContentPackage.Message message = new ContentPackage.Message() { sender = this.account.username, content = sendContent, receiver = this.receiver };
                    string jsonStr = JsonSerializer.Serialize<ContentPackage.Message>(message);
                    ContentPackage.Package package = new ContentPackage.Package() { kind = 204, content = jsonStr };
                    string sendStr = JsonSerializer.Serialize<ContentPackage.Package>(package);
                    Program.sw.WriteLine(sendStr);
                    Program.sw.Flush();
                }
            }
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            this.dgvIcon.BringToFront();
        }

        private void btnCreateGr_Click(object sender, EventArgs e)
        {
            GroupForm groupForm = new GroupForm();
            groupForm.username = this.account.username;
            foreach (DataGridViewRow row in this.dgvFriends.Rows)
            {
                groupForm.dgvFriends.Rows.Add(row.Cells[0].Value);
                groupForm.dgvFriends.Rows[groupForm.dgvFriends.Rows.Count - 1].Tag = row.Tag;

            }
            groupForm.ShowDialog();
        }

        private void dgvFriends_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string imageFilePath = Directory.GetCurrentDirectory() + "\\pic\\selected.png";
            this.rtbContent.Rtf = "";
            DataGridViewRow row = this.dgvFriends.Rows[e.RowIndex];
            row.Selected = true;
            row.Cells[3].Value = Image.FromFile(imageFilePath);
            this.receiver = row.Tag.ToString();
            this.lbReceiver.Text = row.Cells[0].Value.ToString();
            string saveFolder = Directory.GetCurrentDirectory() + "\\save\\" + this.account.username;
            if (Directory.Exists(saveFolder))
            {
                string filePath = saveFolder + "\\" + this.receiver + ".xml";
                if (File.Exists(filePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);
                    foreach (XmlNode node in doc.GetElementsByTagName("Messages")[0].ChildNodes)
                    {
                        this.rtbContent.Select(this.rtbContent.TextLength, 0);
                        string content = node.InnerText.Replace(@"<trleftsize>", this.trleft.ToString()).Replace(@"<cellsize>", this.widthCell.ToString());
                        this.rtbContent.SelectedRtf = content;
                    }
                }
            }
        }

        private void dgvIcon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Image img = (Image)this.dgvIcon.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            this.rtbSend.SelectedRtf = createImageRTF(img, true);
        }

        private void btnIcon_Leave(object sender, EventArgs e)
        {
            this.pnlChat.Controls.SetChildIndex(this.dgvIcon, zIndexLvIcon);
        }

        private void btnCreateGr_Click_1(object sender, EventArgs e)
        {
            this.groupForm = new GroupForm();
            this.groupForm.username = this.account.username;
            foreach (Account account in this.friends)
            {
                this.groupForm.dgvFriends.Rows.Add(account.fullname);
                this.groupForm.dgvFriends.Rows[this.groupForm.dgvFriends.Rows.Count - 1].Tag = account.username;

            }
            this.groupForm.ShowDialog();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.currentPanel.Equals("CHAT"))
            {
                this.lstbxMenu.Location = new Point(this.btnSetting.Location.X, this.pnlLeftBottomChat.Location.Y - this.lstbxMenu.Height);
                this.dgvIcon.Location = new Point(this.btnIcon.Location.X + this.pnlLeftChat.Width - (this.dgvIcon.Width / 2), this.panelBottom.Location.Y - this.dgvIcon.Height);
                this.rtbSend.Width = this.panelBottom.Width - this.btnSend.Width - this.btnIcon.Width - this.btnImage.Width - 10*3;//10*3 khoảng margin giữa các control
                this.widthCell = this.rtbContent.Width * 15 - 500;
                this.trleft = this.widthCell - 4000;
                if (this.receiver != "")
                {
                    string saveFolder = Directory.GetCurrentDirectory() + "\\save\\" + this.account.username.ToString();
                    if (Directory.Exists(saveFolder))
                    {
                        string filePath = saveFolder + "\\" + this.receiver + ".xml";
                        if (File.Exists(filePath))
                        {
                            this.rtbContent.Rtf = "";
                            XmlDocument doc = new XmlDocument();
                            doc.Load(filePath);
                            foreach (XmlNode node in doc.GetElementsByTagName("Messages")[0].ChildNodes)
                            {
                                this.rtbContent.Select(this.rtbContent.TextLength, 0);
                                string content = node.InnerText.Replace(@"<trleftsize>", this.trleft.ToString()).Replace(@"<cellsize>", this.widthCell.ToString());
                                this.rtbContent.SelectedRtf = content;
                            }
                        }
                    }
                }
            }
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

        private void btnSetting_Click(object sender, EventArgs e)
        {
            this.lstbxMenu.BringToFront();
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())    
            {
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Image|*.png; *.jpg; *.jpeg";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        this.fileNames.Add(filePath);
                        string fileName = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                        if (fileName.Length > 10)
                        {
                            fileName = fileName.Substring(0,10) + "...";
                        }
                        builder.Append(fileName + ", ");
                    }
                    if (builder.Length > 30)
                    {
                        this.lbFileName.Text = builder.ToString().Substring(0, 30) + " ....";
                    }
                    else 
                    {
                        this.lbFileName.Text = builder.ToString();
                    }
                }
            }
        }

        private void btnSetting_Leave(object sender, EventArgs e)
        {
            this.pnlLeftChat.Controls.SetChildIndex(this.lstbxMenu, zIndexMenu);
        }

        private void lstbxMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstbxMenu.SelectedIndex > -1)
            {
                string value = this.lstbxMenu.SelectedItem.ToString();
                switch (value.ToUpper())
                {
                    case "LOG OUT":
                        ContentPackage.Package package = new ContentPackage.Package() { kind = 203, content = this.account.username };
                        Program.sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                        Program.sw.Flush();
                        changePanel("LOGIN");
                        break;
                    default:

                        break;
                }
            }
            this.lstbxMenu.SelectedIndex = -1;
        }
        private void setNameAccount()
        {
            if (this.lstbxMenu.InvokeRequired)
            {
                setNameAccountCallback d = new setNameAccountCallback(setNameAccount);
                this.lstbxMenu.Invoke(d);
            }
            else
            {
                this.lstbxMenu.Items[0] = this.account.fullname;
            }    
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            changePanel("REGISTER");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            changePanel("LOGIN");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            bool flag = false;
            this.lbNameReg.Text = "";
            this.lbAccReg.Text = "";
            this.lbPassReg.Text = "";
            this.lbRePassReg.Text = "";
            string name = this.tbNameReg.Text;
            string username = this.tbAccReg.Text;
            string password = this.tbPassReg.Text;
            string repassword = this.tbRePassReg.Text;
            Regex regexFirstWord = new Regex(this.patternFirstWord);
            Regex regexSpecialWords = new Regex(this.patternSpecialWords);
            #region name
            if (string.IsNullOrEmpty(name))
            {
                this.lbNameReg.Text = "Chưa nhập tên người dùng!";
                flag = true;
            }
            if (name.Length > 50)
            {
                this.lbNameReg.Text = "Có tối đa 50 ký tự!";
                flag = true;
            }
            if (!regexFirstWord.IsMatch(name))
            {
                this.lbNameReg.Text = "Ký tự đầu phải là chữ!";
                flag = true;
            }
            if (regexSpecialWords.IsMatch(name))
            {
                this.lbNameReg.Text = "Không chứa các ký tự ~!@#$%^&*()_+{}|:\"<>?`-=[]\\;',./";
                flag = true;
            }
            #endregion
            #region username
            if (string.IsNullOrEmpty(username))
            {
                this.lbAccReg.Text = "Chưa nhập tên tài khoản!";
                flag = true;
            }
            if (username.Length < 6 || username.Length > 20)
            {
                this.lbAccReg.Text = "Phải có tối thiểu 6 ký tự và tối đa 20 ký tự!";
                flag = true;
            }
            if (!regexFirstWord.IsMatch(username))
            {
                this.lbAccReg.Text = "Ký tự đầu phải là chữ!";
                flag = true;
            }
            if (regexSpecialWords.IsMatch(username))
            {
                this.lbAccReg.Text = "Không chứa các ký tự ~!@#$%^&*()_+{}|:\"<>?`-=[]\\;',./";
                flag = true;
            }
            #endregion
            #region password
            if (string.IsNullOrEmpty(password))
            {
                this.lbPassReg.Text = "Chưa nhập mật khẩu!";
                flag = true;
            }
            if (password.Length < 8 || password.Length > 20)
            {
                this.lbPassReg.Text = "Có tối thiểu 8 ký tự và tối đa 20 ký tự!";
                flag = true;
            }
            if (regexSpecialWords.IsMatch(password))
            {
                this.lbPassReg.Text = "Không chứa các ký tự ~!@#$%^&*()_+{}|:\"<>?`-=[]\\;',./";
                flag = true;
            }
            #endregion
            #region repassword
            if (!password.Equals(repassword))
            {
                this.lbRePassReg.Text = "Mật khẩu không trùng khớp!";
                flag = true;
            }
            #endregion
            if (flag)
            {
                return;
            }
            Account regisAcc = new Account() { fullname = name, username = username, password = password };
            ContentPackage.Package package = new ContentPackage.Package() { kind = 201, content = JsonSerializer.Serialize<Account>(regisAcc) };
            string sendedContent = JsonSerializer.Serialize<ContentPackage.Package>(package);
            Program.sw.WriteLine(sendedContent);
            Program.sw.Flush();
        }

        private void setRegisterFailed()
        {
            if (this.lbAccReg.InvokeRequired)
            {
                setRegisterFailedCallback d = new setRegisterFailedCallback(setRegisterFailed);
                this.lbAccReg.Invoke(d);
            }
            else
            {
                this.lbAccReg.Text = "Tài khoản đã tồn tại!";
            }
        }

        private void setLoginFailed()
        {
            if (this.lbLogin.InvokeRequired)
            {
                setLoginFailedCallback d = new setLoginFailedCallback(setLoginFailed);
                this.lbLogin.Invoke(d);
            }
            else
            {
                this.lbLogin.Text = "Tài khoản hoặc mật khẩu không chính xác!";
            }
        }

        private void btnAddFriend_Click(object sender, EventArgs e)
        {
            this.addFriendForm = new AddFriendForm();
            addFriendForm.account = this.account;
            foreach (Account user in this.friendRequests)
            {
                this.addFriendForm.dgvFrReq.Rows.Add();
                int count = this.addFriendForm.dgvFrReq.Rows.Count;
                DataGridViewRow row = this.addFriendForm.dgvFrReq.Rows[count - 1];
                row.Tag = user.username;
                row.Cells[0].Value = user.fullname;
                row.Cells[1].Value = "Accept";
                row.Cells[2].Value = "Deny";
            }
            this.addFriendForm.ShowDialog();
        }

        private void setListUsers(List<Account> users)
        {
            if (this.addFriendForm.InvokeRequired)
            {
                setListUsersCallback d = new setListUsersCallback(setListUsers);
                this.addFriendForm.Invoke(d, new object[] { users });
            }
            else
            {
                this.addFriendForm.dgvUsers.Rows.Clear();
                foreach (Account user in users)
                {
                    this.addFriendForm.dgvUsers.Rows.Add();
                    int count = this.addFriendForm.dgvUsers.Rows.Count;
                    DataGridViewRow row = this.addFriendForm.dgvUsers.Rows[count - 1];
                    row.Tag = user.username;
                    row.Cells[0].Value = user.fullname;
                    switch (user.FrOrRe)
                    {
                        case 2:
                            row.Cells[1].Value = "Cancel friend";
                            row.Cells[1].Tag = 2;
                            break;
                        case 1:
                            row.Cells[1].Value = "Cancel request";
                            row.Cells[1].Tag = 1;
                            break;
                        default:
                            row.Cells[1].Value = "Request";
                            row.Cells[1].Tag = 0;
                            break;
                    }
                }
            }
        }

        private void addFriendNotice(bool notice)
        {
            if (this.btnAddFriend.InvokeRequired)
            {
                addFriendNoticeCallback d = new addFriendNoticeCallback(addFriendNotice);
                this.btnAddFriend.Invoke(d, new object[] { notice });
            }
            else
            {
                if (notice)
                {
                    string filepath = Directory.GetCurrentDirectory() + "\\pic\\add_friend_notice.png";
                    this.btnAddFriend.Image = Image.FromFile(filepath);
                }
                else
                {
                    string filepath = Directory.GetCurrentDirectory() + "\\pic\\add_friend.png";
                    this.btnAddFriend.Image = Image.FromFile(filepath);
                }
            }
        }

        private void setReceivedMessage(ContentPackage.Message message)
        {
            string saveFolder = Directory.GetCurrentDirectory() + "\\save\\" + this.account.username;
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            string chattedFile = saveFolder + "\\" + message.sender + ".xml";
            if (File.Exists(chattedFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(chattedFile);
                XmlNode node = xmlDoc.GetElementsByTagName("Messages")[0].LastChild.Clone();
                node.InnerText = message.content;
                xmlDoc.GetElementsByTagName("Messages")[0].AppendChild(node);
                xmlDoc.Save(chattedFile);
            }
            else
            {
                using (XmlWriter writer = XmlWriter.Create(chattedFile))
                {
                    writer.WriteStartElement("Messages");

                    writer.WriteElementString("Message", message.content);
                    writer.WriteEndElement();
                }
            }
            if (message.sender.Equals(this.receiver))
            {
                setReceivedContent(message.content);
            }
            else
            {
                setNotice(message.sender);
            }
        }

        private void addNewFriendToList(Account friend)
        {
            if (this.dgvFriends.InvokeRequired)
            {
                addNewFriendToListCallback d = new addNewFriendToListCallback(addNewFriendToList);
                this.dgvFriends.Invoke(d, new object[] { friend });
            }
            else
            {
                this.dgvFriends.Rows.Add();
                int count = this.dgvFriends.Rows.Count;
                DataGridViewRow row = this.dgvFriends.Rows[count - 1];
                row.Tag = friend.username;
                row.Cells[0].Value = friend.fullname;
                row.Cells[1].Value = "U";
                if (friend.online)
                {
                    row.Cells[2].Value = "Online";
                    row.Cells[2].Style.ForeColor = Color.Green;
                }
                else
                {
                    row.Cells[2].Value = "Offline";
                    row.Cells[2].Style.ForeColor = Color.Gray;
                }
                //row.DefaultCellStyle.BackColor = Color.FromArgb(212, 243, 255);
            }
        }

        private void setOnline(Online online)
        {
            if (this.dgvFriends.InvokeRequired)
            {
                setOnlineCallback d = new setOnlineCallback(setOnline);
                this.dgvFriends.Invoke(d, new object[] {online});
            }
            else
            {
                foreach (DataGridViewRow row in this.dgvFriends.Rows)
                {
                    if (row.Tag.ToString().Equals(online.index))
                    {
                        if (online.online)
                        {

                            row.Cells[2].Value = "Online";
                            row.Cells[2].Style.ForeColor = Color.Green;
                        }
                        else
                        {
                            row.Cells[2].Value = "Offline";
                            row.Cells[2].Style.ForeColor = Color.Gray;
                        }
                    }
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.account != null)
            {
                ContentPackage.Package package = new ContentPackage.Package() { kind = 203, content = this.account.username};
                Program.sw.WriteLine(JsonSerializer.Serialize<ContentPackage.Package>(package));
                Program.sw.Flush();
            }
        }
        private void setFriendRequests()
        {
            if (this.addFriendForm.InvokeRequired)
            {
                setFriendRequestsCallback d = new setFriendRequestsCallback(setFriendRequests);
                this.addFriendForm.Invoke(d);
            }
            else
            {
                this.addFriendForm.dgvFrReq.Rows.Clear();
                foreach (Account account in this.friendRequests)
                {
                    this.addFriendForm.dgvFrReq.Rows.Add();
                    int count = this.addFriendForm.dgvFrReq.Rows.Count;
                    DataGridViewRow row = this.addFriendForm.dgvFrReq.Rows[count - 1];
                    row.Tag = account.username;
                    row.Cells[0].Value = account.fullname;
                    row.Cells[1].Value = "Accept";
                    row.Cells[2].Value = "Deny";
                }
            }
        }

        private void setNewGroup(GroupChat groupChat)
        {
            if (this.dgvFriends.InvokeRequired)
            {
                setNewGroupCallback d = new setNewGroupCallback(setNewGroup);
                this.dgvFriends.Invoke(d, new object[] { groupChat });
            }
            else
            {
                this.dgvFriends.Rows.Add();
                int count = this.dgvFriends.Rows.Count;
                DataGridViewRow row = this.dgvFriends.Rows[count -1];
                row.Tag = groupChat.Id;
                row.Cells[0].Value = groupChat.GName;
                row.Cells[1].Value = "G";
                if (groupChat.online)
                {

                    row.Cells[2].Value = "Online";
                    row.Cells[2].Style.ForeColor = Color.Green;
                }
                else
                {
                    row.Cells[2].Value = "Offline";
                    row.Cells[2].Style.ForeColor = Color.Gray;
                }
            }
        }

        private void closeGroupForm()
        {
            if (this.groupForm.InvokeRequired)
            {
                closeGroupFormCallback d = new closeGroupFormCallback(closeGroupForm);
                this.groupForm.Invoke(d);
            }
            else
            {
                this.groupForm.Close();
            }
        }

        private void dgvFriends_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.dgvFriends.Rows[e.RowIndex];
            string imageFilePath = Directory.GetCurrentDirectory() + "\\pic\\nothing.png";
            row.Cells[3].Value = Image.FromFile(imageFilePath);
        }
    }
}

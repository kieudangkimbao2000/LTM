using System.Globalization;

namespace ContentPackage
{
    public class Package
    {
        public int kind { get; set; }
        public string content { get; set; }
    }

    public class Account
    {
        public string username { get; set; }
        public string password { get; set; }
        public string fullname { get; set; }
        public bool online { get; set; }
        public int FrOrRe { get; set; }
    }

    public class Message
    {
        public string sender { get; set; }
        public string content { get; set; }
        public string receiver { get; set; }
    }

    public class GroupChat
    {
        public int Id { get; set; }
        public string GName { get; set; }
        public string Admin { get; set; }

        public bool online { get; set; }
    }
    public class InGroup
    {
        public GroupChat GChat { get; set; }
        public List<string> Members { get; set; }
    }

    public class LoginSuccess
    {
        public Account AccountLogin { get; set; }
        public List<Account> Friends { get; set; }
        public List<GroupChat> GroupChats { get; set; }
        public List<Account> FriendRequests { get; set; }
        public List<Message> WaitingMessages { get; set; }
    }

    public class Search
    {
        public string username { get; set; }
        public string search { get; set; }
    }

    public class Accepted
    {
        public List<Account> FriendRequests { get; set; }
        public Account? Friend { get; set; }
    }

    public class Online
    {
        public string index { get; set; }
        public bool online { get; set; }
    }

    public class CancelFriend
    {
        public List<Account> Friends { get; set; }
        public List<GroupChat> GroupChats { get; set; }
    }
}
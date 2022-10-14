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
    }

    public class Message
    {
        public string sender { get; set; }
        public string content { get; set; }
        public string receiver { get; set; }
    }
}
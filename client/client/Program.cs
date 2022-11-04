using client.GUI;
using System.Net;
using System.Net.Sockets;

namespace client
{
    internal static class Program
    {
        public static TcpClient client;
        public static StreamReader sr;
        public static StreamWriter sw;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(getLocalIP()), 2000);
            client = new TcpClient();
            client.Connect(iep);
            sr = new StreamReader(client.GetStream());
            sw = new StreamWriter(client.GetStream());

            ApplicationConfiguration.Initialize();
            Application.Run(new ChatForm());
        }

        static string getLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Khong tim thay dia chi IP!");
        }



        public static byte[] cleanByteArray(byte[] data)
        {
            int c = 0;
            foreach (var b in data)
            {
                if (b != 0)
                {
                    c++;
                }
            }
            byte[] temp = new byte[c];
            for (int i = 0; i < c; i++)
            {
                temp[i] = data[i];
            }
            return temp;
        }
    }
}

namespace Email_Sender
{
    public class SmtpModel
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }

        public SmtpModel() { }
        public SmtpModel(string host, int port, bool enableSll)
        {
            Host = host;
            Port = port;
            EnableSsl = enableSll;
        }
        public SmtpModel(string host, int port, bool enableSll, string userNamme, string displayName,string password) {
            Host = host;
            Port = port;
            EnableSsl = enableSll;
            UserName = userNamme;
            DisplayName = displayName;
            Password = password;
        }


    }
}

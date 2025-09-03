
namespace Email_Sender
{
    public class EmailForm
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        /// <summary>
        /// First file is receiver list and it should be excel or csv
        /// </summary>
        public List<string> ReceiverAndAttachments { get; set; }
    }
}


using System.Text.Json;

namespace Email_Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args!= null && args.Length > 0)
            {
                var helper = new Helper();

                if (args.Length == 1)
                    helper.LogFileName = $"{Environment.CurrentDirectory}\\log.txt";
                else
                    helper.LogFileName = args[1];

                try
                {
                    string jsonData = File.ReadAllText(args[0]);
                    EmailForm emailForm = JsonSerializer.Deserialize<EmailForm>(jsonData);

                    if (emailForm != null && !string.IsNullOrWhiteSpace(emailForm.Subject) && !string.IsNullOrWhiteSpace(emailForm.Body) && emailForm.ReceiverAndAttachments.Count >= 1)
                    {
                        var receiver = helper.GetEmailReceiverList(emailForm.ReceiverAndAttachments[0]);
                        var attachments = helper.GetAttachments(emailForm.ReceiverAndAttachments.Skip(1));
                        new EmailService(helper).SendEmail(receiver, emailForm.Subject, emailForm.Body, attachments, emailForm.DisplayName, emailForm.UserName, emailForm.Password);

                        File.Delete(args[0]);
                    }
                }
                catch (Exception ex)
                {
                    helper.WriteErrorLogMessage(ex.Message.ToString());
                }
            }            
        }
    }
}
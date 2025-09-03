using System.Net.Mail;
using System.Net;

namespace Email_Sender
{
    public class EmailService
    {
        Helper _helper = null;
        public EmailService(Helper helper) {
            _helper = helper;
        }

        public void SendEmail(List<ReceiverListAndEmailTypo> toMailAddresses, string subject, string body, List<Attachment> attachments, string displayName, string userName, string password)
        {
            for(int i = 0;i < toMailAddresses.Count; i++)
            {
                if(SendEmail(toMailAddresses[i], subject, body, attachments, displayName, userName, password))
                {
                    Console.WriteLine($"Sent: {i + 1}/{toMailAddresses.Count}");
                }
                else
                {
                    Console.WriteLine($"Fail to sent: {i + 1}/{toMailAddresses.Count}");
                }               
            }
        }
        public bool SendEmail(ReceiverListAndEmailTypo toMailAddress, string subject, string body, List<Attachment> attachments, string displayName, string userName, string password)
        {

            bool mailSent = false;
            try
            {
                //Replace Word
                if (toMailAddress.Typo != null)
                {
                    foreach (var keyValue in toMailAddress.Typo)
                    {
                        body = body.Replace($"@{keyValue.Key}", keyValue.Value);
                    }
                }
                string senderEmail = new string[] { userName, _helper.SMTP.UserName}.FirstOrDefault(s => !string.IsNullOrEmpty(s)) ?? "";
                string senderDisplayName = new string[] { displayName, _helper.SMTP.DisplayName }.FirstOrDefault(s => !string.IsNullOrEmpty(s)) ?? "";
                string senderPassword = new string[] { password, _helper.SMTP.Password }.FirstOrDefault(s => !string.IsNullOrEmpty(s)) ?? "";
   
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(senderEmail, senderDisplayName);
                message.To.Add(toMailAddress.MailAddress);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                if (attachments != null && attachments.Count > 0)
                {
                    attachments.ForEach(attachment =>
                    {
                        message.Attachments.Add(attachment);
                    });
                }
                smtp.Host = _helper.SMTP.Host;
                smtp.Port = _helper.SMTP.Port;
                smtp.EnableSsl = _helper.SMTP.EnableSsl;
                smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.SendCompleted += new SendCompletedEventHandler(_helper.SendCompletedCallback);
                smtp.Send(message);
                mailSent = true;
            }
            catch (Exception ex)
            {
                mailSent = false;
                _helper.WriteErrorLogMessage(ex.Message.ToString());
            }

            return mailSent;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using OfficeOpenXml;
using System.ComponentModel;
using System.Data;
using System.Net.Mail;
using System.Text.Json;

namespace Email_Sender
{
    public class Helper
    {
        public string LogFileName { get; set; }

        private IConfiguration _configuration = null;
        public Helper()
        {
            string appSetting = $"{Environment.CurrentDirectory}\\appsettings.json";
            if (File.Exists(appSetting))
            {
                var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile(appSetting, optional: false).Build();

                _configuration = builder;
            }
           
        }

        public SmtpModel SMTP
        {
            get
            {
                var smtp = _configuration?.GetSection("SMTP");
                
                if (smtp == null || string.IsNullOrWhiteSpace(smtp["Host"]))
                    return new SmtpModel("smtp.office365.com", 587,true);
                else
                    return new SmtpModel(smtp["Host"], Convert.ToInt32(smtp["Port"]), Convert.ToBoolean(smtp["EnableSsl"]), smtp["UserName"], smtp["DisplayName"], smtp["Password"]);
            }
        }

        public DataTable ConvertCSVToDataTable(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return ConvertCSVtoDataTable(new TextFieldParser(new StringReader(text)));
        }

        public DataTable ConvertExcelToDataTable(string filePath)
        {
            var dt = new DataTable();
            var fi = new FileInfo(filePath);
            // Check if the file exists
            if (!fi.Exists)
                throw new Exception("File " + filePath + " Does Not Exists");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var xlPackage = new ExcelPackage(fi);

            // get the first worksheet in the workbook
            var worksheet = xlPackage.Workbook.Worksheets[0];

            dt = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].ToDataTable(c => {
                c.FirstRowIsColumnNames = true;
                c.ColumnNameParsingStrategy = OfficeOpenXml.Export.ToDataTable.NameParsingStrategy.RemoveSpace;
            });

            return dt;
        }

        private DataTable ConvertCSVtoDataTable(TextFieldParser textFieldParser)
        {
            DataTable dt = new DataTable();
            using (var parser = textFieldParser)
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] headers = parser.ReadFields();
                foreach (string header in headers)
                {
                    dt.Columns.Add(header.Replace(" ", "").Trim());
                }

                while (!parser.EndOfData)
                {
                    string[] rows = parser.ReadFields();
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        public void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                WriteErrorLogMessage(e.Error.Message);
            }
        }

        public void WriteErrorLogMessage(string errorMessage)
        {
            if(!string.IsNullOrWhiteSpace(LogFileName) && !string.IsNullOrWhiteSpace(errorMessage) )
            {
                string logFileFolder = Path.GetDirectoryName(LogFileName)??"";
                if (!string.IsNullOrWhiteSpace(logFileFolder) && !Directory.Exists(Path.GetDirectoryName(LogFileName)))
                {
                    Directory.CreateDirectory(logFileFolder);
                }

                if (File.Exists(LogFileName))
                {
                    FileInfo fs = new FileInfo(LogFileName);
                    long fileSizeibMbs = fs.Length / (1024 * 1024);
                    if(fileSizeibMbs > 2){
                        File.WriteAllText(LogFileName, "");
                    }
                }

                if (!File.Exists(LogFileName))
                {
                    var logFile = File.Create(LogFileName);
                    logFile.Close();
                }
                using StreamWriter sw = File.AppendText(LogFileName);
                sw.WriteLine(JsonSerializer.Serialize(new
                {
                    time = DateTime.Now,
                    message = errorMessage
                }));
                sw.WriteLine(" ");
                sw.Close();
            }            
        }

        public List<Attachment> GetAttachments(IEnumerable<string> uploadedFiles)
        {
            List<Attachment> attachments = new List<Attachment>();
            foreach (var fileName in uploadedFiles)
            {
                if (File.Exists(fileName))
                {
                    attachments.Add(new Attachment(fileName));
                }                
            }
            return attachments;
        }


        public List<ReceiverListAndEmailTypo> GetEmailReceiverList(string fileName)
        {
            HashSet<string> emailsKey = new HashSet<string>();
            List<ReceiverListAndEmailTypo> mailAddresses = new List<ReceiverListAndEmailTypo>();
            var dataTable = new DataTable();

            if (fileName.EndsWith(".csv") && File.Exists(fileName))
                dataTable = ConvertCSVToDataTable(fileName);

            else if (fileName.EndsWith(".xlsx") && File.Exists(fileName))
                dataTable = ConvertExcelToDataTable(fileName);

            if (!dataTable.Columns.Contains("Name") && !dataTable.Columns.Contains("EmailAddress"))
            {
                Console.WriteLine($"The \"Name\" and \"EmailAddress\" columns are missing from the file: {fileName}");
                throw new Exception($"The \"Name\" and \"EmailAddress\" columns are missing from the file: {fileName}");
            }
            else if (!dataTable.Columns.Contains("EmailAddress"))
            {
                Console.WriteLine($"The Column \"EmailAddress\" is missing from the file: {fileName}");
                throw new Exception($"The Column \"EmailAddress\" is missing from the file: {fileName}");
            }
            else if (!dataTable.Columns.Contains("Name"))
            {   
                Console.WriteLine($"The Column \"Name\" is missing from the file: {fileName}");
                throw new Exception($"The Column \"Name\" is missing from the file: {fileName}");
            }

            if(dataTable.Rows.Count==0) {
                Console.WriteLine($"There are no entries for \"Name\" and \"EmailAddress\" in the file: {fileName}");
                throw new Exception($"There are no entries for \"Name\" and \"EmailAddress\" in the file: {fileName}");
            }

            foreach (DataRow row in dataTable.Rows)
            {
                ReceiverListAndEmailTypo receiverListAndEmailTypo = new ReceiverListAndEmailTypo();
                if (!string.IsNullOrWhiteSpace(row["EmailAddress"].ToString()) && !emailsKey.Contains(row["EmailAddress"].ToString()))
                {
                    emailsKey.Add(row["EmailAddress"].ToString());
                    receiverListAndEmailTypo.MailAddress = new MailAddress(row["EmailAddress"].ToString(), row["Name"].ToString());
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        var key = column.ColumnName.Replace(" ", "");
                        if (!keyValuePairs.ContainsKey(key))
                            keyValuePairs.Add(key, row[column.ColumnName].ToString());
                    }
                    receiverListAndEmailTypo.Typo = keyValuePairs;
                    mailAddresses.Add(receiverListAndEmailTypo);
                }
            }

            return mailAddresses;
        }
    }
}

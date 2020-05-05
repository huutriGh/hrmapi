using HRM.Models;
using Microsoft.AspNet.Cryptography.KeyDerivation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HRM.Services.ServiceImp
{
    public class HelperIml : IHelper
    {
        private const int Keysize = 256;
        private readonly IApplication application;
        public HelperIml(IApplication application)
        {
            this.application = application;

        }
        public HelperIml()
        {
            

        }
       
        public static DateTime DateAddMonths(DateTime startDate, int monthCount, bool sticky)
        {
            DateTime result = startDate.AddMonths(monthCount);
            if (sticky)
            {
                if (DateTime.DaysInMonth(startDate.Year, startDate.Month) == startDate.Day)
                {
                    int resultYear = result.Year;
                    int resultMonth = result.Month;
                    result = new DateTime(resultYear, resultMonth,
                                          DateTime.DaysInMonth(resultYear, resultMonth));
                }
            }
            return result;
        }

        public static void DateDiffMonths(DateTime date1, DateTime date2, bool sticky,
                                    out int months, out int days)
        {

            // ensure that date1 <= date2
            if (date2 < date1)
            {
                DateTime t = date2;
                date2 = date1;
                date1 = t;
            }

            // make a guess at the answer; using 31 means that we'll be close but won't exceed
            int monthCount = ((date2 - date1).Days / 31);

            // find the maximum number of months that's less than or equal to the second date
            DateTime testDate = DateAddMonths(date1, monthCount, sticky);
            while (testDate < date2)
            {
                testDate = DateAddMonths(date1, ++monthCount, sticky);
            }

            // if we've hit the exact date, return the number of months and zero days
            if (testDate == date2)
            {
                months = monthCount;
                days = 0;
                return;
            }

            // otherwise we exceeded the second date, back up and return the correct values
            testDate = DateAddMonths(date1, --monthCount, sticky);
            months = monthCount;
            days = (date2 - testDate).Days;
        }

        private const int DerivationIterations = 1000;
        public void SendEmail(string subject, List<EmployeeLeave> mainContent, string mailTo, Int16 node)
        {
            try
            {
                var leaveType = application.GetContext().LeaveType.ToList();
                var content = application.GetContext().EmployeeLeave.ToList().Where(el => mainContent.Select(m => m.LeaveId).Contains(el.LeaveId)).ToList();
                StringBuilder listLeave = new StringBuilder();
                var EmployeeRequest = application.GetContext().Database.SqlQuery<Employee>("select Gender,  FullName from Employee where BusinessEntityID = @BusinessEntityID", new SqlParameter("@businessEntityID", content.First().BusinessEntityID)).FirstOrDefault();
                var mailConfig = application.GetContext().SystemConfig.Where(c => c.Id.Equals(1)).SingleOrDefault();
                //string action = hubject.Equals("Applied", StringComparison.OrdinalIgnoreCase) && node == 5 ? "Verify" : ((subject.Equals("verified", StringComparison.OrdinalIgnoreCase) || subject.Equals("verified", StringComparison.OrdinalIgnoreCase)) ? "approve" : "completed");
                var action = content.FirstOrDefault().Status;
                StringBuilder sb = new StringBuilder();
                SqlParameter paramter = new SqlParameter("@businessEntityID", content.First().BusinessEntityID);
                if (action == 1 || action == 2)
                {

                    if (action == 1 && !string.IsNullOrEmpty(content.First().AssigneeVer))
                    {

                        sb.AppendLine("select Email, Gender, FirstName from Employee where BusinessEntityID = @businessEntityID");
                        paramter = new SqlParameter("@businessEntityID", content.First().AssigneeVer);
                    }
                    else if (action == 2 && !string.IsNullOrEmpty(content.First().AssigneeApp))
                    {

                        sb.AppendLine("select Email, Gender, FirstName from Employee where BusinessEntityID = @businessEntityID");
                        paramter = new SqlParameter("@businessEntityID", content.First().AssigneeApp);
                    }

                    //else if ((action == 2 && string.IsNullOrEmpty(content.First().AssigneeApp)) || (action == 1 && string.IsNullOrEmpty(content.First().AssigneeVer)))
                    else
                    {
                        sb.AppendLine("DECLARE @Manager hierarchyid");
                        sb.AppendLine("Select @Manager = OrganizationNode from  Employee where BusinessEntityID = @businessEntityID");
                        sb.AppendLine("select Email, Gender, FirstName  from Employee where @Manager.IsDescendantOf(OrganizationNode)=1");
                        sb.AppendLine("and BusinessEntityID <> @businessEntityID order by OrganizationLevel desc");
                        if (action == 2)
                        {
                            paramter = new SqlParameter("@businessEntityID", content.First().PersonVerified);

                        }

                    }
                }
                else
                {

                    sb.AppendLine("select Email, Gender, FirstName from Employee where BusinessEntityID = @businessEntityID");
                }


                var emailTo = application.GetContext().Database.SqlQuery<Employee>(sb.ToString(), paramter).FirstOrDefault();
                string body = "";
                foreach (var item in content)
                {

                    listLeave.AppendLine("<tr>");
                    listLeave.AppendLine("<td>" + item.BusinessEntityID + "</td>");
                    listLeave.AppendLine("<td>" + EmployeeRequest.FullName + "</td>");
                    listLeave.AppendLine("<td>" + leaveType.Where(l => l.LeaveTypeId.Equals(item.LeaveTypeId)).Select(l => l.Description).FirstOrDefault() + "</td>");
                    listLeave.AppendLine("<td>" + item.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                    listLeave.AppendLine("<td>" + item.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "</td>");
                    listLeave.AppendLine("<td>" + (item.Residence == "residence" ? "Place of residence" : "Travel out of residence") + " </td>");
                    listLeave.AppendLine("<td>" + item.ToLocation + "</td>");
                    listLeave.AppendLine("<td>" + item.Contact + "</td>");
                    listLeave.AppendLine("<td>" + item.PersonToCover + "</td>");
                    listLeave.AppendLine("</tr>");

                }
                if (action < 3)
                {
                    body = mailConfig.EmailTemplate.Replace("[Name]", (emailTo.Gender.Equals("F", StringComparison.OrdinalIgnoreCase) ? "Ms. " : "Mr. ") + emailTo.FirstName).Replace("[Employee]", (EmployeeRequest.Gender.Equals("F", StringComparison.OrdinalIgnoreCase) ? "Ms. " : "Mr. ") + EmployeeRequest.FullName).Replace("[Action]", action == 1 ? "verify" : "approve").Replace("[body]", listLeave.ToString());
                }
                else
                {
                    body = mailConfig.EmailTemplateAprroved.Replace("[Name]", (EmployeeRequest.Gender.Equals("F", StringComparison.OrdinalIgnoreCase) ? "Ms. " : "Mr. ") + EmployeeRequest.FullName).Replace("[body]", listLeave.ToString());
                }




                if (mailConfig != null)
                {
                    //SmtpClient mailcl = new SmtpClient(mailConfig.MailServer);
                    //MailAddress from = new MailAddress(mailConfig.Email);  
                    SmtpClient mailcl = new SmtpClient("smtp.live.com");
                    MailAddress from = new MailAddress("huutriqt13@hotmail.com");
                    mailcl.UseDefaultCredentials = false;
                    // mailcl.Credentials = new NetworkCredential(mailConfig.EmailUser, Decrypt(mailConfig.PasswordEmail, mailConfig.MailServer));
                    mailcl.Credentials = new NetworkCredential("huutriqt13@hotmail.com", "tri@hotmail.com");
                    mailcl.Port = 25;
                    mailcl.EnableSsl = true;
                    MailMessage Msg = new MailMessage();
                    Msg.To.Add(emailTo.Email);

                    Msg.From = from;
                    Msg.Subject = "Leave Request";
                    Msg.Body = body;
                    Msg.IsBodyHtml = true;
                    Msg.BodyEncoding = System.Text.Encoding.UTF8;
                    mailcl.Send(Msg);
                }
                else
                {
                    throw new Exception("Email is not configed !!!");
                }




            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public  string Encrypt(string input)
        {

            byte[] salt = new byte[16] { 10, 12, 13, 15, 20, 25, 29, 20, 10, 12, 13, 15, 20, 25, 29, 20 };
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: input,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }


        public  bool validPassword(string password, string hashedPassword)
        {
            var dbPasswordHashed = Convert.FromBase64String(hashedPassword);
            var inputPasswordHash = Convert.FromBase64String(Encrypt(password));
            return StructuralComparisons.StructuralEqualityComparer.Equals(dbPasswordHashed, inputPasswordHash);

        }
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        public static string Encrypt(string plainText, string passPhrase)
        {
           
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
           
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
        
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
    }
}
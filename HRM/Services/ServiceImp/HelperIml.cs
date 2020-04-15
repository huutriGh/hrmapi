﻿using Microsoft.AspNet.Cryptography.KeyDerivation;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly IApplication application;
        public HelperIml(IApplication application)
        {
            this.application = application;

        }
        public HelperIml()
        {
            

        }
        private const int Keysize = 256;

       
        private const int DerivationIterations = 1000;
        public void SendEmail(string subject, string body, string mailTo, string cc)
        {
            try
            {
                var mailConfig = application.GetContext().SystemConfig.Where(c => c.Id.Equals(1)).SingleOrDefault();
                if(mailConfig != null)
                {
                    SmtpClient mailcl = new SmtpClient(mailConfig.MailServer);
                    MailAddress from = new MailAddress(mailConfig.Email);
                    mailcl.UseDefaultCredentials = false;
                    mailcl.Credentials = new NetworkCredential(mailConfig.EmailUser, Decrypt(mailConfig.PasswordEmail, mailConfig.MailServer));

                    MailMessage Msg = new MailMessage();
                    Msg.To.Add(mailTo);
                    Msg.From = from;
                    Msg.Subject = subject;
                    Msg.Body = body;
                    Msg.IsBodyHtml = false;
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
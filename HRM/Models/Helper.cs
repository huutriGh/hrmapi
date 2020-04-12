using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Cryptography.KeyDerivation;
using Microsoft.Owin;
using Owin;



namespace HRM.Models
{
    public class Helper
    {
        public static string Encrypt(string input)
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

        
        public static bool validPassword(string password, string hashedPassword)
        {
            var dbPasswordHashed = Convert.FromBase64String(hashedPassword);
            var inputPasswordHash = Convert.FromBase64String(Encrypt(password));
            return StructuralComparisons.StructuralEqualityComparer.Equals(dbPasswordHashed, inputPasswordHash);
            
        }


    }
}

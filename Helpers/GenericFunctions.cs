using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MasterPass.Asmx.Helpers
{
    public class GenericFunctions
    {
        public static string Encrypt(string encryptString)
        {
            string EncryptionKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string GenerateRandomNumbers(int length = 10)
        {
            //Initiate objects & vars  
            byte[] seed = Guid.NewGuid().ToByteArray();
            Random random = new Random(BitConverter.ToInt32(seed, 0));
            int randNumber = 0;
            //Loop ‘length’ times to generate a random number or character
            String randomNumber = "";
            for (int i = 0; i < length; i++)
            {
                randNumber = random.Next(48, 58);
                randomNumber = randomNumber + (char)randNumber;
                //append random char or digit to randomNumber string

            }
            return randomNumber;
        }

        public static string[] SplitStringMiniStatement(string St)
        {
            string[] result = new string[10];
            string valueString = "";
            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                {
                    valueString = St.Substring(0, 87);
                }
                else if (i == 1)
                {
                    valueString = St.Substring(87, 87);
                }
                else if (i == 2)
                {
                    valueString = St.Substring(174, 87);
                }
                else if (i == 3)
                {
                    valueString = St.Substring(261, 87);
                }
                else if (i == 4)
                {
                    valueString = St.Substring(348, 87);
                }
                else if (i == 5)
                {
                    valueString = St.Substring(435, 87);
                }
                else if (i == 6)
                {
                    valueString = St.Substring(522, 87);
                }
                else if (i == 7)
                {
                    valueString = St.Substring(609, 87);
                }
                else if (i == 8)
                {
                    valueString = St.Substring(696, 87);
                }
                else if (i == 9)
                {
                    valueString = St.Substring(783, 87);
                }

                result[i] = valueString;
            }
            return result;
        }

        public static string[] SplitStringMiniStatementDet(string St)
        {
            string[] result = new string[4];
            string valueString = "";
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    valueString = St.Substring(0, 8);
                }
                else if (i == 1)
                {
                    valueString = St.Substring(8, 61).Trim();
                }
                else if (i == 2)
                {
                    valueString = St.Substring(69, 1);
                }
                else if (i == 3)
                {
                    valueString = St.Substring(70, 17).Trim();
                }

                result[i] = valueString;
            }
            return result;
        }
    }
}
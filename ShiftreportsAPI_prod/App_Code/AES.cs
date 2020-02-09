using shiftreportapp.data;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ShiftReportApi.App_Code
{
	public class AES
    {



        private static string GenerateKey(int iKeySize)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = iKeySize;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.GenerateIV();
            string ivStr = Convert.ToBase64String(aesEncryption.IV);
            aesEncryption.GenerateKey();
            string keyStr = Convert.ToBase64String(aesEncryption.Key);
            string completeKey = ivStr + "," + keyStr;

            return Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(completeKey));
        }

        private static string Encrypt(string clearText)
        {
            string EncryptionKey = "xM5z01923C#^^;;5";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private static string Decrypt(string cipherText)
        {
            string EncryptionKey = "xM5z01923C#^^;;5";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
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

        public static bool CheckPassword(string plaintext, string hashed)
        {
            return StringComparer.Ordinal.Compare(hashed,Encrypt(plaintext)) == 0;
        }

      

        public static void EncryptManagerPw(int managerID, string password)
        {
            AppModel Context = new shiftreportapp.data.AppModel();
            var db = Context.Database;
            string hash = Encrypt(password);
            db.ExecuteSqlCommand("update managers_mst set manager_pw='" + hash + "' where id=" + managerID);
        }

        public static string DecryptData(string hash)
        {
            return Decrypt(hash);
        }

        public static void EncryptCard(int cardID, string number,string csc)
        {
            AppModel Context = new shiftreportapp.data.AppModel();
            var db = Context.Database;
            string numberHash = Encrypt(number);
            string cscHash = Encrypt(csc);
            db.ExecuteSqlCommand("update customer_payment_mst set card_number='" + numberHash + "', card_csc='" + cscHash + "' where id=" + cardID);
        }

    

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static void EncryptCashierPw(int cashierID, string password)
        {
            AppModel Context = new shiftreportapp.data.AppModel();
            var db = Context.Database;
            string hash = Encrypt(password);
            db.ExecuteSqlCommand("update cashier_mst set cashier_pw='" + hash + "' where id=" + cashierID);
        }

        public static void EncryptCard(string cardID, string number, string csc)
        {
            //AppModel Context = new shiftreportapp.data.AppModel();
            //var db = Context.Database;
            //string encryptedCard = number + "xM5~z_^^";
            //string hash = BCrypt.HashPassword(encryptedCard, BCrypt.GenerateSalt());

            //string encryptedCSC = csc + "xM5~z_^^";
            //string cscHash = BCrypt.HashPassword(encryptedCSC, BCrypt.GenerateSalt());
            //db.ExecuteSqlCommand("update customer_payment_mst set card_number='" + hash + "', csc='" + cscHash + "' where id=" + cardID);
        }



		// Decrypt a byte array into a byte array using a key and an IV 
		public static String Decrypt(String cipherData, String Key, String IV)
		{
			byte[] decryptedData;
			//string plaintext = null;
			//MemoryStream ms = new MemoryStream(cipherData);

			RijndaelManaged alg = new RijndaelManaged();
			
			alg.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(Key);
			alg.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
			alg.Mode = CipherMode.CBC;
			


			Byte[] cipherBin = System.Text.ASCIIEncoding.ASCII.GetBytes(cipherData);
			//Array.Copy(Key, 0, IV, 0, IV.Length);

			ICryptoTransform decryptor = alg.CreateDecryptor(alg.Key, alg.IV);

			using (MemoryStream ms = new MemoryStream(cipherBin))
			{
				using (CryptoStream csDecrypt = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
				{
					using (StreamReader sw = new StreamReader(csDecrypt))
					{
						sw.ReadToEnd();
						sw.Close();
					}

					csDecrypt.Close();
					decryptedData = ms.ToArray();
				}
			}

			//byte[] decryptedData = System.Text.Encoding.Unicode.GetBytes(plaintext);
			return System.Text.ASCIIEncoding.ASCII.GetString(decryptedData);
		}



	}
    
}
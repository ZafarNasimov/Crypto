using System.Security.Cryptography;
using System.Text;


    public class WalletUtil
    {
        private byte[] privateKeyBytes {  get; set; }
        private string PrivateKey;

        private byte[] publicKeyBytes { get; set; }
        private string PublicKey;
        public string Address { get; set; }

        private void GenerateECCKeys()
        {
            using (ECDsa ecdsa = ECDsa.Create())
            {
                privateKeyBytes = ecdsa.ExportECPrivateKey();
                publicKeyBytes = ecdsa.ExportSubjectPublicKeyInfo();

                PrivateKey = BitConverter.ToString(privateKeyBytes).Replace("-", String.Empty).ToLower();
                PublicKey = BitConverter.ToString(publicKeyBytes).Replace("-", String.Empty).ToLower();
            }
        }
        
        public string GetPrivateKey()
        {
        	return PrivateKey.Substring(0, 64);
        }
        
        public string GetPublicKey()
        {
        	return PublicKey.Substring(0, 64);
        }

        public string GenerateAddress()
        {
            GenerateECCKeys();
        
            // Step 1: SHA-256 hash
            byte[] sha256Hash = SHA256Hash(publicKeyBytes);

            // Step 2: Base16 encoding
            Address = Base16Encode(sha256Hash);
            return Address;
        }

        private static byte[] SHA256Hash(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }

        private static string Base16Encode(byte[] data)
        {            
            const string Base16Chars = "0123456789ABCDEF";
            StringBuilder result = new StringBuilder(data.Length * 2);

            foreach (byte b in data)
            {
                result.Append(Base16Chars[b >> 4]);
                result.Append(Base16Chars[b & 0xF]);
            }

            return result.ToString();
        }
        
        
    }


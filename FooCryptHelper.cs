using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace FooBlog
{
    public class FooCryptHelper
    {
        // AES256 (GCM) ENCRYPTION + PBKDF2 DERIVATION OPERATIONS.
        private const string TokenPurpose = "Protected String";
        private static readonly SecureRandom Random = new SecureRandom();

        //Preconfigured Encryption Parameters
        public static readonly int NonceBitSize = 128;
        public static readonly int MacBitSize = 128;
        public static readonly int KeyBitSize = 256;

        //Preconfigured Password Key Derivation Parameters
        public static readonly int SaltBitSize = 512;
        public static readonly int Iterations = 10000;
        public static readonly int MinPasswordLength = 12;


        // AES256 GCM ENCRYPTION + HMAC MESSAGE VALIDATION

        public static byte[] NewKey()
        {
            var key = new byte[KeyBitSize/8];
            Random.NextBytes(key);
            return key;
        }

        public static string SimpleEncrypt(string secretMessage, byte[] key, byte[] nonSecretPayload = null)
        {
            if (String.IsNullOrEmpty(secretMessage))
                throw new ArgumentException(@"Secret Message Required!", "secretMessage");

            byte[] plainText = Encoding.UTF8.GetBytes(secretMessage);
            byte[] cipherText = SimpleEncrypt(plainText, key, nonSecretPayload);
            return Convert.ToBase64String(cipherText);
        }

        public static string SimpleDecrypt(string encryptedMessage, byte[] key, int nonSecretPayloadLength = 0)
        {
            if (String.IsNullOrEmpty(encryptedMessage))
                throw new ArgumentException(@"Encrypted Message Required!", "encryptedMessage");

            byte[] cipherText = Convert.FromBase64String(encryptedMessage);
            byte[] plaintext = SimpleDecrypt(cipherText, key, nonSecretPayloadLength);
            return Encoding.UTF8.GetString(plaintext);
        }

        public static string SimpleEncryptWithPassword(string secretMessage, string password,
                                                       byte[] nonSecretPayload = null)
        {
            if (String.IsNullOrEmpty(secretMessage))
                throw new ArgumentException(@"Secret Message Required!", "secretMessage");

            byte[] plainText = Encoding.UTF8.GetBytes(secretMessage);
            byte[] cipherText = SimpleEncryptWithPassword(plainText, password, nonSecretPayload);
            return Convert.ToBase64String(cipherText);
        }

        public static string SimpleDecryptWithPassword(string encryptedMessage, string password,
                                                       int nonSecretPayloadLength = 0)
        {
            if (String.IsNullOrWhiteSpace(encryptedMessage))
                throw new ArgumentException(@"Encrypted Message Required!", "encryptedMessage");

            byte[] cipherText = Convert.FromBase64String(encryptedMessage);
            byte[] plaintext = SimpleDecryptWithPassword(cipherText, password, nonSecretPayloadLength);
            return Encoding.UTF8.GetString(plaintext);
        }

        public static byte[] SimpleEncrypt(byte[] secretMessage, byte[] key, byte[] nonSecretPayload = null)
        {
            //User Error Checks
            if (key == null || key.Length != KeyBitSize/8)
                throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "key");

            if (secretMessage == null || secretMessage.Length == 0)
                throw new ArgumentException(@"Secret Message Required!", "secretMessage");

            //Non-secret Payload Optional
            nonSecretPayload = nonSecretPayload ?? new byte[] {};

            //Using random nonce large enough not to repeat
            var nonce = new byte[NonceBitSize/8];
            Random.NextBytes(nonce, 0, nonce.Length);

            var cipher = new GcmBlockCipher(new AesFastEngine());
            var parameters = new AeadParameters(new KeyParameter(key), MacBitSize, nonce, nonSecretPayload);
            cipher.Init(true, parameters);

            //Generate Cipher Text With Auth Tag
            var cipherText = new byte[cipher.GetOutputSize(secretMessage.Length)];
            int len = cipher.ProcessBytes(secretMessage, 0, secretMessage.Length, cipherText, 0);
            cipher.DoFinal(cipherText, len);

            //Assemble Message
            using (var combinedStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(combinedStream))
                {
                    //Prepend Authenticated Payload
                    binaryWriter.Write(nonSecretPayload);
                    //Prepend Nonce
                    binaryWriter.Write(nonce);
                    //Write Cipher Text
                    binaryWriter.Write(cipherText);
                }
                return combinedStream.ToArray();
            }
        }

        public static byte[] SimpleDecrypt(byte[] encryptedMessage, byte[] key, int nonSecretPayloadLength = 0)
        {
            //User Error Checks
            if (key == null || key.Length != KeyBitSize/8)
                throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "key");

            if (encryptedMessage == null || encryptedMessage.Length == 0)
                throw new ArgumentException(@"Encrypted Message Required!", "encryptedMessage");

            using (var cipherStream = new MemoryStream(encryptedMessage))
            using (var cipherReader = new BinaryReader(cipherStream))
            {
                //Grab Payload
                byte[] nonSecretPayload = cipherReader.ReadBytes(nonSecretPayloadLength);

                //Grab Nonce
                byte[] nonce = cipherReader.ReadBytes(NonceBitSize/8);

                var cipher = new GcmBlockCipher(new AesFastEngine());
                var parameters = new AeadParameters(new KeyParameter(key), MacBitSize, nonce, nonSecretPayload);
                cipher.Init(false, parameters);

                //Decrypt Cipher Text
                byte[] cipherText =
                    cipherReader.ReadBytes(encryptedMessage.Length - nonSecretPayloadLength - nonce.Length);
                var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];

                try
                {
                    int len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
                    cipher.DoFinal(plainText, len);
                }
                catch (InvalidCipherTextException)
                {
                    //Return null if it doesn't authenticate
                    return null;
                }

                return plainText;
            }
        }

        public static byte[] SimpleEncryptWithPassword(byte[] secretMessage, string password,
                                                       byte[] nonSecretPayload = null)
        {
            nonSecretPayload = nonSecretPayload ?? new byte[] {};

            //User Error Checks
            if (String.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
                throw new ArgumentException(
                    String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

            if (secretMessage == null || secretMessage.Length == 0)
                throw new ArgumentException(@"Secret Message Required!", "secretMessage");

            var generator = new Pkcs5S2ParametersGenerator();

            //Use Random Salt to minimize pre-generated weak password attacks.
            var salt = new byte[SaltBitSize/8];
            Random.NextBytes(salt);

            generator.Init(
                PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()),
                salt,
                Iterations);

            //Generate Key
            var key = (KeyParameter) generator.GenerateDerivedMacParameters(KeyBitSize);

            //Create Full Non Secret Payload
            var payload = new byte[salt.Length + nonSecretPayload.Length];
            Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
            Array.Copy(salt, 0, payload, nonSecretPayload.Length, salt.Length);

            return SimpleEncrypt(secretMessage, key.GetKey(), payload);
        }

        public static byte[] SimpleDecryptWithPassword(byte[] encryptedMessage, string password,
                                                       int nonSecretPayloadLength = 0)
        {
            //User Error Checks
            if (String.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
                throw new ArgumentException(
                    String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

            if (encryptedMessage == null || encryptedMessage.Length == 0)
                throw new ArgumentException(@"Encrypted Message Required!", "encryptedMessage");

            var generator = new Pkcs5S2ParametersGenerator();

            //Grab Salt from Payload
            var salt = new byte[SaltBitSize/8];
            Array.Copy(encryptedMessage, nonSecretPayloadLength, salt, 0, salt.Length);

            generator.Init(
                PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()),
                salt,
                Iterations);

            //Generate Key
            var key = (KeyParameter) generator.GenerateDerivedMacParameters(KeyBitSize);

            return SimpleDecrypt(encryptedMessage, key.GetKey(), salt.Length + nonSecretPayloadLength);
        }

        // ENCRYPTION OPERTAIONS.

        public static string Encrypt(string plaintext, string key)
        {
            try
            {
                return SimpleEncryptWithPassword(plaintext, key);
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return null;
            }
        }

        public static string Decrypt(string encrypted, string key)
        {
            try
            {
                byte[] encryptedArray = Convert.FromBase64String(encrypted);
                byte[] decryptedArray = SimpleDecryptWithPassword(encryptedArray, key);
                return Encoding.UTF8.GetString(decryptedArray);
            }
            catch (Exception ex)
            {
                FooLogging.WriteLog(ex.ToString());
                return null;
            }
        }

        // HASHING OPERATIONS.

        public static string CreateShaHash(string input)
        {
            var hasher = new SHA256Managed();
            byte[] passwordAsByte = Encoding.UTF8.GetBytes(input);
            byte[] encryptedBytes = hasher.ComputeHash(passwordAsByte);
            hasher.Clear();
            // Return as hex.
            return BitConverter.ToString(encryptedBytes).Replace("-", string.Empty);
        }

        // MACHINE KEY OPERATIONS.

        public static string MachineEncrypt(string input)
        {
            byte[] unprotectedBytes = Encoding.UTF8.GetBytes(input);
            byte[] protectedBytes = MachineKey.Protect(unprotectedBytes, TokenPurpose);
            string protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static string MachineDecrypt(string protectedText)
        {
            byte[] protectedBytes = Convert.FromBase64String(protectedText);
            byte[] unprotectedBytes = MachineKey.Unprotect(protectedBytes, TokenPurpose);
            if (unprotectedBytes != null)
            {
                string unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
                return unprotectedText;
            }
            return null;
        }
    }
}
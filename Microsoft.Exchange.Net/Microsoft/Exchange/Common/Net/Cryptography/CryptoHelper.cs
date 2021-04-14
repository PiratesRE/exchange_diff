using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public class CryptoHelper
	{
		internal CryptoHelper(string keyContainer, string password)
		{
			this.cspParams = new CspParameters
			{
				KeyContainerName = keyContainer,
				ProviderType = 1,
				KeyNumber = 1,
				Flags = CspProviderFlags.UseDefaultKeyContainer
			};
			this.pdb = new PasswordDeriveBytes(password, null, this.cspParams);
			this.tdes = new TripleDESCryptoServiceProvider
			{
				Key = this.pdb.CryptDeriveKey("TripleDES", "MD5", 0, new byte[8])
			};
		}

		protected CryptoHelper()
		{
		}

		public static CryptoHelper GetInstanceFromGlobalObject(string password)
		{
			return CryptoHelper.GetInstanceFromGlobalObject("CryptoHelper", password);
		}

		public static CryptoHelper GetInstance(string password)
		{
			return CryptoHelper.GetInstance("CryptoHelper", password);
		}

		public static CryptoHelper GetInstance(string keyContainer, string password)
		{
			if (keyContainer == null)
			{
				throw new ArgumentNullException("keyContainer");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			CryptoHelper result;
			lock (CryptoHelper.cryptoHelperTableLock)
			{
				IDictionary<string, CryptoHelper> dictionary;
				if (CryptoHelper.cryptoHelperTable.ContainsKey(keyContainer))
				{
					dictionary = CryptoHelper.cryptoHelperTable[keyContainer];
				}
				else
				{
					dictionary = new Dictionary<string, CryptoHelper>();
					CryptoHelper.cryptoHelperTable.Add(keyContainer, dictionary);
				}
				CryptoHelper cryptoHelper;
				if (dictionary.ContainsKey(password))
				{
					cryptoHelper = dictionary[password];
				}
				else
				{
					cryptoHelper = new CryptoHelper(keyContainer, password);
					dictionary.Add(password, cryptoHelper);
				}
				result = cryptoHelper;
			}
			return result;
		}

		public static CryptoHelper GetInstanceFromGlobalObject(string keyContainer, string password)
		{
			if (keyContainer == null)
			{
				throw new ArgumentNullException("keyContainer");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			IDictionary<string, CryptoHelper> dictionary = null;
			CryptoHelper result = null;
			if (CryptoHelper.cryptoHelperTable.ContainsKey(keyContainer))
			{
				dictionary = CryptoHelper.cryptoHelperTable[keyContainer];
			}
			if (dictionary != null && dictionary.ContainsKey(password))
			{
				result = dictionary[password];
			}
			return result;
		}

		public static void BuildCryptoHelperTable(string password)
		{
			CryptoHelper.BuildCryptoHelperTable("CryptoHelper", password);
		}

		public static void BuildCryptoHelperTable(string keyContainer, string password)
		{
			if (keyContainer == null)
			{
				throw new ArgumentNullException("keyContainer");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			IDictionary<string, CryptoHelper> dictionary;
			if (CryptoHelper.cryptoHelperTable.ContainsKey(keyContainer))
			{
				dictionary = CryptoHelper.cryptoHelperTable[keyContainer];
			}
			else
			{
				dictionary = new Dictionary<string, CryptoHelper>();
				CryptoHelper.cryptoHelperTable.Add(keyContainer, dictionary);
			}
			CryptoHelper value;
			if (dictionary.ContainsKey(password))
			{
				value = dictionary[password];
				return;
			}
			value = new CryptoHelper(keyContainer, password);
			dictionary.Add(password, value);
		}

		public virtual string Encrypt(string inputText)
		{
			byte[] inArray = this.DoEncrypt(inputText);
			return Convert.ToBase64String(inArray);
		}

		public virtual string Decrypt(string inputText)
		{
			if (inputText == null)
			{
				throw new ArgumentNullException("inputText");
			}
			byte[] decodedBytes = Convert.FromBase64String(inputText);
			return this.DoDecrypt(decodedBytes);
		}

		internal string GenerateHint(string hintMaterial)
		{
			if (hintMaterial == null)
			{
				throw new ArgumentNullException("hintMaterial");
			}
			int num = 5381;
			for (int i = 0; i < hintMaterial.Length; i++)
			{
				num = num * 33 + (int)hintMaterial[i];
			}
			byte[] array = new byte[4];
			for (int j = 3; j >= 0; j--)
			{
				array[j] = (byte)(num % 256);
				num >>= 8;
			}
			string text = Convert.ToBase64String(array);
			if (text.Length < 5)
			{
				text += "EEEEE";
			}
			return text.Substring(0, 5);
		}

		internal string EncryptWithHint(string inputText, string hint, int hintOffset)
		{
			if (hint == null)
			{
				throw new ArgumentNullException("hint");
			}
			if (hintOffset < 0)
			{
				throw new ArgumentOutOfRangeException("hintOffset");
			}
			byte[] inArray = this.DoEncrypt(inputText);
			string text = Convert.ToBase64String(inArray);
			if (text.Length < hintOffset)
			{
				throw new InvalidOperationException("hintOffset is too big");
			}
			return text.Substring(0, hintOffset) + hint + text.Substring(hintOffset);
		}

		internal string DecryptWithHint(string inputText, string hint, int hintOffset)
		{
			if (inputText == null)
			{
				throw new ArgumentNullException("inputText");
			}
			if (hint == null)
			{
				throw new ArgumentNullException("hint");
			}
			if (hintOffset < 0)
			{
				throw new ArgumentOutOfRangeException("hintOffset");
			}
			if (inputText.Length < hintOffset + 5)
			{
				throw new ArgumentException("inputText is too short to contain hint.");
			}
			if (!hint.Equals(inputText.Substring(hintOffset, 5), StringComparison.InvariantCulture))
			{
				return null;
			}
			string s = inputText.Substring(0, hintOffset) + inputText.Substring(hintOffset + 5);
			byte[] decodedBytes = Convert.FromBase64String(s);
			return this.DoDecrypt(decodedBytes);
		}

		private byte[] DoEncrypt(string inputText)
		{
			if (inputText == null)
			{
				throw new ArgumentNullException("inputText");
			}
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			ICryptoTransform cryptoTransform = null;
			byte[] result;
			try
			{
				ASCIIEncoding asciiencoding = new ASCIIEncoding();
				memoryStream = new MemoryStream();
				cryptoTransform = this.tdes.CreateEncryptor(this.tdes.Key, new byte[8]);
				cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
				byte[] bytes = asciiencoding.GetBytes(inputText);
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				result = memoryStream.ToArray();
			}
			finally
			{
				if (cryptoStream != null)
				{
					cryptoStream.Close();
				}
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
				if (cryptoTransform != null)
				{
					cryptoTransform.Dispose();
				}
			}
			return result;
		}

		private string DoDecrypt(byte[] decodedBytes)
		{
			if (decodedBytes == null)
			{
				throw new ArgumentNullException("decodedBytes");
			}
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			ICryptoTransform cryptoTransform = null;
			string @string;
			try
			{
				ASCIIEncoding asciiencoding = new ASCIIEncoding();
				memoryStream = new MemoryStream(decodedBytes);
				cryptoTransform = this.tdes.CreateDecryptor(this.tdes.Key, new byte[8]);
				cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
				byte[] array = new byte[decodedBytes.Length];
				int count = cryptoStream.Read(array, 0, array.Length);
				@string = asciiencoding.GetString(array, 0, count);
			}
			finally
			{
				if (cryptoStream != null)
				{
					cryptoStream.Close();
				}
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
				if (cryptoTransform != null)
				{
					cryptoTransform.Dispose();
				}
			}
			return @string;
		}

		public const string DefaultKeyContainerName = "CryptoHelper";

		protected const string EncryptionAlgorithmName = "TripleDES";

		protected const string HashAlgorithmName = "MD5";

		protected const string HintFiller = "EEEEE";

		protected const int HintLength = 5;

		private static IDictionary<string, IDictionary<string, CryptoHelper>> cryptoHelperTable = new Dictionary<string, IDictionary<string, CryptoHelper>>();

		private static object cryptoHelperTableLock = new object();

		private CspParameters cspParams;

		private PasswordDeriveBytes pdb;

		private TripleDESCryptoServiceProvider tdes;
	}
}

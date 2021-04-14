using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Exchange.Security.Cryptography;

namespace Microsoft.Exchange.Management.RightsManagement
{
	internal class PasswordDerivedKey
	{
		public PasswordDerivedKey(SecureString password) : this(PasswordDerivedKey.RmsPasswordDeriveBytes.GetBytes(password, 16))
		{
		}

		public PasswordDerivedKey(SecureString password, bool newHashAlgorithm) : this(PasswordDerivedKey.GetPasswordDeriveBytes(password, newHashAlgorithm))
		{
		}

		private static byte[] GetPasswordDeriveBytes(SecureString password, bool newHashAlgorithm)
		{
			if (newHashAlgorithm)
			{
				return PasswordDerivedKey.RmsPasswordDeriveBytes.GetBytes(password, 16);
			}
			byte[] bytes;
			using (PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(PasswordDerivedKey.ConvertFromSecureString(password), null))
			{
				bytes = passwordDeriveBytes.GetBytes(16);
			}
			return bytes;
		}

		public PasswordDerivedKey(byte[] key) : this(new AesCryptoServiceProvider(), key)
		{
		}

		public PasswordDerivedKey(SymmetricAlgorithm algorithm, byte[] key)
		{
			this.m_symmetricAlgorithm = algorithm;
			this.m_symmetricAlgorithm.Key = key;
			this.m_symmetricAlgorithm.IV = PasswordDerivedKey.IV;
			this.m_symmetricAlgorithm.Padding = PaddingMode.PKCS7;
		}

		private static string ConvertFromSecureString(SecureString secureString)
		{
			if (secureString == null)
			{
				throw new ArgumentNullException("secureString");
			}
			IntPtr intPtr = Marshal.SecureStringToBSTR(secureString);
			string result;
			try
			{
				string text = Marshal.PtrToStringUni(intPtr);
				result = text;
			}
			finally
			{
				Marshal.ZeroFreeBSTR(intPtr);
			}
			return result;
		}

		private static byte[] ConvertFromSecureStringToByteArray(SecureString secureString)
		{
			if (secureString == null)
			{
				throw new ArgumentNullException("secureString");
			}
			IntPtr intPtr = Marshal.SecureStringToCoTaskMemUnicode(secureString);
			byte[] result;
			try
			{
				int num = secureString.Length * Marshal.SizeOf(typeof(char)) * 2;
				byte[] array = new byte[num];
				Marshal.Copy(intPtr, array, 0, num);
				result = array;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeCoTaskMemUnicode(intPtr);
				}
			}
			return result;
		}

		internal void Clear()
		{
			this.m_symmetricAlgorithm.Clear();
		}

		internal byte[] Decrypt(byte[] cipherText)
		{
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			byte[] result;
			try
			{
				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, this.m_symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
				cryptoStream.Write(cipherText, 0, cipherText.Length);
				cryptoStream.FlushFinalBlock();
				result = memoryStream.ToArray();
			}
			finally
			{
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
				if (cryptoStream != null)
				{
					cryptoStream.Close();
				}
			}
			return result;
		}

		private const int KEYSIZE = 128;

		private static byte[] IV = new byte[]
		{
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16
		};

		private SymmetricAlgorithm m_symmetricAlgorithm;

		private class RmsPasswordDeriveBytes
		{
			public static byte[] GetBytes(SecureString password, int numberOfBytes)
			{
				if (password == null)
				{
					throw new ArgumentException("password");
				}
				if (numberOfBytes > 32)
				{
					throw new ArgumentOutOfRangeException("numberOfBytes");
				}
				byte[] array = null;
				using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
				{
					array = sha256CryptoServiceProvider.ComputeHash(PasswordDerivedKey.ConvertFromSecureStringToByteArray(password));
				}
				byte[] array2 = new byte[numberOfBytes];
				for (int i = 0; i < numberOfBytes; i++)
				{
					array2[i] = array[i];
				}
				return array2;
			}

			private const int SHA256HashSize = 32;
		}
	}
}

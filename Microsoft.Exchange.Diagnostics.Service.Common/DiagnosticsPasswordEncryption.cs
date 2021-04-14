using System;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class DiagnosticsPasswordEncryption
	{
		public DiagnosticsPasswordEncryption(string keyString, string initialVectorString)
		{
			this.key = Convert.FromBase64String(keyString);
			this.initialVector = Convert.FromBase64String(initialVectorString);
		}

		public string EncryptString(string password)
		{
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			StreamWriter streamWriter = null;
			AesCryptoServiceProvider aesCryptoServiceProvider = null;
			try
			{
				aesCryptoServiceProvider = new AesCryptoServiceProvider();
				ICryptoTransform transform = aesCryptoServiceProvider.CreateEncryptor(this.key, this.initialVector);
				memoryStream = new MemoryStream();
				cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				streamWriter = new StreamWriter(cryptoStream);
				streamWriter.Write(password);
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
					streamWriter = null;
				}
				if (cryptoStream != null)
				{
					cryptoStream.Close();
					cryptoStream = null;
				}
				if (memoryStream != null)
				{
					memoryStream.Close();
				}
				if (aesCryptoServiceProvider != null)
				{
					aesCryptoServiceProvider.Dispose();
				}
			}
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		public string DecryptString(string encryptedPasswordString)
		{
			byte[] buffer = Convert.FromBase64String(encryptedPasswordString);
			MemoryStream memoryStream = null;
			CryptoStream cryptoStream = null;
			StreamReader streamReader = null;
			AesCryptoServiceProvider aesCryptoServiceProvider = null;
			string result;
			try
			{
				aesCryptoServiceProvider = new AesCryptoServiceProvider();
				ICryptoTransform transform = aesCryptoServiceProvider.CreateDecryptor(this.key, this.initialVector);
				memoryStream = new MemoryStream(buffer);
				cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
				streamReader = new StreamReader(cryptoStream);
				result = streamReader.ReadToEnd();
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
					streamReader = null;
				}
				if (cryptoStream != null)
				{
					cryptoStream.Close();
					cryptoStream = null;
				}
				if (memoryStream != null)
				{
					memoryStream.Close();
					memoryStream = null;
				}
				if (aesCryptoServiceProvider != null)
				{
					aesCryptoServiceProvider.Dispose();
				}
			}
			return result;
		}

		private readonly byte[] key;

		private readonly byte[] initialVector;
	}
}

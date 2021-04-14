using System;
using System.Security;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Cryptography;

namespace Microsoft.Exchange.Migration
{
	internal class CryptoAdapter : ICryptoAdapter
	{
		string ICryptoAdapter.ClearStringToEncryptedString(string clearString)
		{
			if (string.IsNullOrEmpty(clearString))
			{
				return null;
			}
			string result;
			using (SecureString secureString = clearString.ConvertToSecureString())
			{
				result = CryptoTools.Encrypt(secureString, CryptoAdapter.EncryptionKey);
			}
			return result;
		}

		bool ICryptoAdapter.TrySecureStringToEncryptedString(SecureString secureInput, out string encryptedString, out Exception failure)
		{
			bool result = true;
			failure = null;
			try
			{
				encryptedString = CryptoTools.Encrypt(secureInput, CryptoAdapter.EncryptionKey);
			}
			catch (Exception ex)
			{
				encryptedString = null;
				result = false;
				failure = ex;
			}
			return result;
		}

		SecureString ICryptoAdapter.EncryptedStringToSecureString(string encryptedInput)
		{
			return CryptoTools.Decrypt(encryptedInput, CryptoAdapter.EncryptionKey);
		}

		bool ICryptoAdapter.TryEncryptedStringToSecureString(string encryptedInput, out SecureString decryptedOutput, out Exception failure)
		{
			bool result = true;
			failure = null;
			try
			{
				decryptedOutput = CryptoTools.Decrypt(encryptedInput, CryptoAdapter.EncryptionKey);
			}
			catch (Exception ex)
			{
				decryptedOutput = null;
				result = false;
				failure = ex;
			}
			return result;
		}

		private static readonly byte[] EncryptionKey = new Guid("6E1822F4-6997-4A24-870D-D2D671898FD4").ToByteArray();
	}
}

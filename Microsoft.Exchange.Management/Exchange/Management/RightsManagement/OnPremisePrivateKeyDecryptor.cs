using System;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OnPremisePrivateKeyDecryptor : IPrivateKeyDecryptor
	{
		public OnPremisePrivateKeyDecryptor(SecureString password)
		{
			this.password = password;
		}

		public byte[] Decrypt(string encryptedData)
		{
			byte[] result;
			try
			{
				result = OnPremisePrivateKeyDecryptor.DecryptWithPassword(this.password, encryptedData);
			}
			catch (CryptographicException innerException)
			{
				throw new PrivateKeyDecryptionFailedException("Unable to decrypt TPD private key", innerException);
			}
			return result;
		}

		private static byte[] DecryptWithPassword(SecureString password, string toDecrypt)
		{
			PasswordDerivedKey passwordDerivedKey = new PasswordDerivedKey(password);
			byte[] result;
			try
			{
				result = passwordDerivedKey.Decrypt(Convert.FromBase64String(toDecrypt));
			}
			catch (CryptographicException)
			{
				PasswordDerivedKey passwordDerivedKey2 = new PasswordDerivedKey(password, false);
				try
				{
					result = passwordDerivedKey2.Decrypt(Convert.FromBase64String(toDecrypt));
				}
				finally
				{
					passwordDerivedKey2.Clear();
				}
			}
			finally
			{
				passwordDerivedKey.Clear();
			}
			return result;
		}

		private SecureString password;
	}
}

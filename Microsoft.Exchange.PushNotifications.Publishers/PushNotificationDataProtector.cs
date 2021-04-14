using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Exchange.Security.Dkm;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationDataProtector
	{
		public PushNotificationDataProtector(IExchangeGroupKey dkm = null)
		{
			this.Dkm = (dkm ?? new ExchangeGroupKey(null, "Microsoft Exchange DKM"));
		}

		private IExchangeGroupKey Dkm { get; set; }

		public SecureString Decrypt(string encryptedText)
		{
			if (string.IsNullOrEmpty(encryptedText))
			{
				return null;
			}
			SecureString result;
			try
			{
				result = this.Dkm.EncryptedStringToSecureString(encryptedText);
			}
			catch (Exception ex)
			{
				if (this.ShouldWrapException(ex))
				{
					throw new PushNotificationConfigurationException(Strings.DataProtectionDecryptingError(encryptedText, ex.Message), ex);
				}
				throw;
			}
			return result;
		}

		public string Encrypt(string clearText)
		{
			if (string.IsNullOrEmpty(clearText))
			{
				return null;
			}
			string result;
			try
			{
				result = this.Dkm.ClearStringToEncryptedString(clearText);
			}
			catch (Exception ex)
			{
				if (this.ShouldWrapException(ex))
				{
					throw new PushNotificationConfigurationException(Strings.DataProtectionEncryptingError(ex.Message), ex);
				}
				throw;
			}
			return result;
		}

		private bool ShouldWrapException(Exception ex)
		{
			return ex is CryptographicException || ex is InvalidDataException || ex is FormatException || this.Dkm.IsDkmException(ex);
		}

		public static readonly PushNotificationDataProtector Default = new PushNotificationDataProtector(new ExchangeGroupKey(null, "Microsoft Exchange DKM"));
	}
}

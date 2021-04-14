using System;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Dkm;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NullExchangeGroupKey : IExchangeGroupKey
	{
		public string ClearStringToEncryptedString(string clearString)
		{
			return clearString;
		}

		public string SecureStringToEncryptedString(SecureString secureString)
		{
			if (secureString == null || secureString.Length == 0)
			{
				return null;
			}
			return secureString.ConvertToUnsecureString();
		}

		public SecureString EncryptedStringToSecureString(string encryptedString)
		{
			if (string.IsNullOrEmpty(encryptedString))
			{
				return null;
			}
			SecureString secureString = new SecureString();
			SecureString result;
			using (DisposeGuard disposeGuard = secureString.Guard())
			{
				foreach (char c in encryptedString)
				{
					secureString.AppendChar(c);
				}
				disposeGuard.Success();
				result = secureString;
			}
			return result;
		}

		public bool TrySecureStringToEncryptedString(SecureString secureString, out string encryptedString, out Exception exception)
		{
			exception = null;
			encryptedString = this.SecureStringToEncryptedString(secureString);
			return true;
		}

		public bool TryEncryptedStringToSecureString(string encryptedString, out SecureString secureString, out Exception exception)
		{
			exception = null;
			secureString = this.EncryptedStringToSecureString(encryptedString);
			return true;
		}

		public bool IsDkmException(Exception e)
		{
			return false;
		}
	}
}

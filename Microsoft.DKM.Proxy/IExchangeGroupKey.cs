using System;
using System.Security;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Dkm
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExchangeGroupKey
	{
		string ClearStringToEncryptedString(string clearString);

		string SecureStringToEncryptedString(SecureString secureString);

		SecureString EncryptedStringToSecureString(string encryptedString);

		bool TrySecureStringToEncryptedString(SecureString secureString, out string encryptedString, out Exception exception);

		bool TryEncryptedStringToSecureString(string encryptedString, out SecureString secureString, out Exception exception);

		bool IsDkmException(Exception e);
	}
}

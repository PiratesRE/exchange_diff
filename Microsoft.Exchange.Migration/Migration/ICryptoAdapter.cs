using System;
using System.Security;

namespace Microsoft.Exchange.Migration
{
	internal interface ICryptoAdapter
	{
		string ClearStringToEncryptedString(string clearString);

		SecureString EncryptedStringToSecureString(string encryptedString);

		bool TryEncryptedStringToSecureString(string encryptedString, out SecureString secureString, out Exception exception);

		bool TrySecureStringToEncryptedString(SecureString secureString, out string encryptedString, out Exception exception);
	}
}

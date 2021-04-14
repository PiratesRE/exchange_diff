using System;
using System.Security;
using Microsoft.Exchange.Security.Dkm;

namespace Microsoft.Exchange.Migration
{
	internal class DkmAdapter : ICryptoAdapter
	{
		string ICryptoAdapter.ClearStringToEncryptedString(string clearString)
		{
			return new ExchangeGroupKey(null, "Microsoft Exchange DKM").ClearStringToEncryptedString(clearString);
		}

		SecureString ICryptoAdapter.EncryptedStringToSecureString(string encryptedString)
		{
			return new ExchangeGroupKey(null, "Microsoft Exchange DKM").EncryptedStringToSecureString(encryptedString);
		}

		bool ICryptoAdapter.TryEncryptedStringToSecureString(string encryptedString, out SecureString secureString, out Exception exception)
		{
			return new ExchangeGroupKey(null, "Microsoft Exchange DKM").TryEncryptedStringToSecureString(encryptedString, out secureString, out exception);
		}

		bool ICryptoAdapter.TrySecureStringToEncryptedString(SecureString secureString, out string encryptedString, out Exception exception)
		{
			return new ExchangeGroupKey(null, "Microsoft Exchange DKM").TrySecureStringToEncryptedString(secureString, out encryptedString, out exception);
		}
	}
}

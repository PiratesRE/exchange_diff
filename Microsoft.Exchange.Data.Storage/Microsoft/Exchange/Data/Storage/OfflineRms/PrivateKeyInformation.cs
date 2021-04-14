using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PrivateKeyInformation
	{
		public PrivateKeyInformation(string keyId, string keyIdType, string keyContainerName, int keyNumber, string cSPName, int cSPType, string encryptedPrivateKeyBlob, bool isSLCKey)
		{
			this.KeyId = keyId;
			this.KeyIdType = keyIdType;
			this.KeyContainerName = keyContainerName;
			this.CSPName = cSPName;
			this.CSPType = cSPType;
			this.KeyNumber = keyNumber;
			this.Identity = this.KeyId + this.KeyIdType;
			this.EncryptedPrivateKeyBlob = encryptedPrivateKeyBlob;
			this.IsSLCKey = isSLCKey;
		}

		public static string GetIdentity(string keyId, string keyIdType)
		{
			return keyId + keyIdType;
		}

		public readonly int KeyNumber;

		public readonly int CSPType;

		public readonly string CSPName;

		public readonly string KeyContainerName;

		public readonly string KeyId;

		public readonly string KeyIdType;

		public readonly string Identity;

		public readonly string EncryptedPrivateKeyBlob;

		public readonly bool IsSLCKey;
	}
}

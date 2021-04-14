using System;
using System.IO;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	[Serializable]
	public class EdgeSyncCredential
	{
		public static EdgeSyncCredential DeserializeEdgeSyncCredential(byte[] data)
		{
			EdgeSyncCredential result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				EdgeSyncCredential edgeSyncCredential = (EdgeSyncCredential)EdgeSyncCredential.serializer.Deserialize(memoryStream);
				result = edgeSyncCredential;
			}
			return result;
		}

		public static byte[] SerializeEdgeSyncCredential(EdgeSyncCredential credential)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EdgeSyncCredential.serializer.Serialize(memoryStream, credential);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public EdgeSyncCredential Clone()
		{
			return new EdgeSyncCredential
			{
				EdgeServerFQDN = this.EdgeServerFQDN,
				ESRAUsername = this.ESRAUsername,
				EncryptedESRAPassword = this.EncryptedESRAPassword,
				EffectiveDate = this.EffectiveDate,
				Duration = this.Duration,
				IsBootStrapAccount = this.IsBootStrapAccount
			};
		}

		private static EdgeSyncCredentialSerializer serializer = new EdgeSyncCredentialSerializer();

		public string EdgeServerFQDN;

		public string ESRAUsername;

		public byte[] EncryptedESRAPassword;

		public byte[] EdgeEncryptedESRAPassword;

		public long EffectiveDate;

		public long Duration;

		public bool IsBootStrapAccount;
	}
}

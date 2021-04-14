using System;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	[Serializable]
	public struct EdgeSubscriptionData
	{
		public string EdgeServerName;

		public string EdgeServerFQDN;

		public byte[] EdgeCertificateBlob;

		public byte[] PfxKPKCertificateBlob;

		public string ESRAUsername;

		public string ESRAPassword;

		public long EffectiveDate;

		public long Duration;

		public int AdamSslPort;

		public string ServerType;

		public string ProductID;

		public int VersionNumber;

		public string SerialNumber;
	}
}

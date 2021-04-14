using System;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public class CredentialRecord
	{
		public string TargetEdgeServerFQDN;

		public string ESRAUsername;

		public DateTime EffectiveDate;

		public TimeSpan Duration;

		public bool IsBootStrapAccount;
	}
}

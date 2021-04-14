using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class FeatureSetSettings : ItemPropertiesBase
	{
		[DataMember]
		public long MaxDailyMessages { get; set; }

		[DataMember]
		public bool ToolsAccount { get; set; }

		[DataMember]
		public long MaxRecipients { get; set; }

		[DataMember]
		public bool HipChallengeApplicable { get; set; }

		[DataMember]
		public int AccountTrustLevel { get; set; }

		[DataMember]
		public bool HijackDetection { get; set; }
	}
}

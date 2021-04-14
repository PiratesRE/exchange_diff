using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MyAccountStatisticsData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public UnlimitedUnsignedInteger DatabaseIssueWarningQuota { get; set; }

		[DataMember]
		public UnlimitedUnsignedInteger DatabaseProhibitSendQuota { get; set; }

		[DataMember]
		public UnlimitedUnsignedInteger DatabaseProhibitSendReceiveQuota { get; set; }

		[DataMember]
		public NullableStorageLimitStatus StorageLimitStatus { get; set; }

		[DataMember]
		public UnlimitedUnsignedInteger TotalItemSize { get; set; }
	}
}

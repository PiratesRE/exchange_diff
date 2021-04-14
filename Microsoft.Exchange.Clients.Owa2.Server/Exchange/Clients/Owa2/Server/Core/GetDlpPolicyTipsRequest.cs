using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetDlpPolicyTipsRequest
	{
		[DataMember]
		public BaseItemId ItemId { get; set; }

		[DataMember]
		public bool NeedToReclassify { get; set; }

		[DataMember]
		public bool BodyOrSubjectChanged { get; set; }

		[DataMember]
		public bool CustomizedStringsNeeded { get; set; }

		[DataMember]
		public EventTrigger EventTrigger { get; set; }

		[DataMember]
		public EmailAddressWrapper[] Recipients { get; set; }

		[DataMember]
		public bool ClientSupportsScanResultData { get; set; }

		[DataMember]
		public string ScanResultData { get; set; }

		public static GetDlpPolicyTipsRequest Ping = new GetDlpPolicyTipsRequest
		{
			ItemId = new ItemId(Guid.Empty.ToString(), Guid.Empty.ToString()),
			NeedToReclassify = false,
			BodyOrSubjectChanged = false
		};
	}
}

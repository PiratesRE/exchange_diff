using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PolicyTipCustomizedStrings
	{
		[DataMember]
		public string ComplianceURL { get; set; }

		[DataMember]
		public string PolicyTipMessageNotifyString { get; set; }

		[DataMember]
		public string PolicyTipMessageOverrideString { get; set; }

		[DataMember]
		public string PolicyTipMessageBlockString { get; set; }
	}
}

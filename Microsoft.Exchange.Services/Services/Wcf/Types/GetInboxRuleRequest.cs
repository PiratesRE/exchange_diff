using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetInboxRuleRequest : BaseJsonRequest
	{
		[DataMember]
		public string DescriptionTimeFormat { get; set; }

		[DataMember]
		public string DescriptionTimeZone { get; set; }

		public override string ToString()
		{
			return string.Format("GetInboxRuleRequest: DescriptionTimeFormat = {0}, DescriptionTimeZone = {1}", this.DescriptionTimeZone, this.DescriptionTimeFormat);
		}
	}
}

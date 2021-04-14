using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InboxRule : SetInboxRuleData
	{
		[DataMember]
		public RuleDescription Description { get; internal set; }

		[DataMember]
		public string DescriptionTimeFormat { get; internal set; }

		[DataMember]
		public string DescriptionTimeZone { get; internal set; }

		[DataMember]
		public bool Enabled { get; internal set; }

		[DataMember]
		public bool InError { get; internal set; }

		[DataMember]
		public bool SupportedByTask { get; internal set; }

		[DataMember]
		public string[] WarningMessages { get; internal set; }

		public InboxRule()
		{
			this.Enabled = true;
		}
	}
}

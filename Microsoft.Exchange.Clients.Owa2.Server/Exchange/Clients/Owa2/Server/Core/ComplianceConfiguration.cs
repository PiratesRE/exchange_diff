using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ComplianceConfiguration
	{
		[DataMember]
		public RmsTemplateType[] RmsTemplates { get; set; }

		[DataMember]
		public MessageClassificationType[] MessageClassifications { get; set; }
	}
}

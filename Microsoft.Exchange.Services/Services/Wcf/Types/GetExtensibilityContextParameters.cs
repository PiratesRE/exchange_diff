using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetExtensibilityContextParameters
	{
		[DataMember(IsRequired = true, Order = 1)]
		public FormFactor FormFactor { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		public string ClientLanguage { get; set; }
	}
}

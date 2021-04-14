using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class EwsProxyRequestParameters
	{
		[DataMember(IsRequired = true, Order = 1)]
		public string Body { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		public string Token { get; set; }

		[DataMember(IsRequired = true, Order = 3)]
		public string ExtensionId { get; set; }
	}
}

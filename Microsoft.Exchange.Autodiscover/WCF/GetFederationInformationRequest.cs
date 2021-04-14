using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class GetFederationInformationRequest : AutodiscoverRequest
	{
		[DataMember(Name = "Domain", IsRequired = true)]
		public string Domain { get; set; }
	}
}

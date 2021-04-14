using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "GetFederationInformationResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class GetFederationInformationResponse : AutodiscoverResponse
	{
		[DataMember(Name = "Domains", IsRequired = false)]
		public DomainCollection Domains { get; set; }

		[DataMember(Name = "ApplicationUri", IsRequired = false)]
		public Uri ApplicationUri { get; set; }

		[DataMember(Name = "TokenIssuers", IsRequired = false)]
		public TokenIssuerCollection TokenIssuers { get; set; }
	}
}

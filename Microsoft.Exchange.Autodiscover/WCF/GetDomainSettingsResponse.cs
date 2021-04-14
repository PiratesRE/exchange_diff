using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "GetDomainSettingsResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class GetDomainSettingsResponse : AutodiscoverResponse
	{
		public GetDomainSettingsResponse()
		{
			this.DomainResponses = new List<DomainResponse>();
		}

		[DataMember(Name = "DomainResponses")]
		public List<DomainResponse> DomainResponses { get; set; }
	}
}

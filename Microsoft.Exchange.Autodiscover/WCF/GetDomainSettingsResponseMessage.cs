using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetDomainSettingsResponseMessage : AutodiscoverResponseMessage
	{
		public GetDomainSettingsResponseMessage()
		{
			this.Response = new GetDomainSettingsResponse();
		}

		[MessageBodyMember]
		public GetDomainSettingsResponse Response { get; set; }
	}
}

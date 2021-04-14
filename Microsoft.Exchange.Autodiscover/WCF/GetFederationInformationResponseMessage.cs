using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetFederationInformationResponseMessage : AutodiscoverResponseMessage
	{
		public GetFederationInformationResponseMessage()
		{
			this.Response = new GetFederationInformationResponse();
		}

		[MessageBodyMember]
		public GetFederationInformationResponse Response { get; set; }
	}
}

using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetUserSettingsResponseMessage : AutodiscoverResponseMessage
	{
		public GetUserSettingsResponseMessage()
		{
			this.Response = new GetUserSettingsResponse();
		}

		[MessageBodyMember]
		public GetUserSettingsResponse Response { get; set; }
	}
}

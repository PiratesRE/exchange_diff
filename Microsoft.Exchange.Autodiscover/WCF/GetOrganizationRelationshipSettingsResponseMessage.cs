using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetOrganizationRelationshipSettingsResponseMessage : AutodiscoverResponseMessage
	{
		public GetOrganizationRelationshipSettingsResponseMessage()
		{
			this.Response = new GetOrganizationRelationshipSettingsResponse();
		}

		[MessageBodyMember]
		public GetOrganizationRelationshipSettingsResponse Response { get; set; }
	}
}

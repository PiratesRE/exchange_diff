using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class AutodiscoverService : IAutodiscover
	{
		public GetUserSettingsResponseMessage GetUserSettings(GetUserSettingsRequestMessage message)
		{
			return this.Execute<GetUserSettingsResponseMessage>(message);
		}

		public GetDomainSettingsResponseMessage GetDomainSettings(GetDomainSettingsRequestMessage message)
		{
			return this.Execute<GetDomainSettingsResponseMessage>(message);
		}

		public GetFederationInformationResponseMessage GetFederationInformation(GetFederationInformationRequestMessage message)
		{
			return this.Execute<GetFederationInformationResponseMessage>(message);
		}

		public GetOrganizationRelationshipSettingsResponseMessage GetOrganizationRelationshipSettings(GetOrganizationRelationshipSettingsRequestMessage message)
		{
			return this.Execute<GetOrganizationRelationshipSettingsResponseMessage>(message);
		}

		private TResponse Execute<TResponse>(AutodiscoverRequestMessage message) where TResponse : AutodiscoverResponseMessage
		{
			TResponse response = default(TResponse);
			Common.SendWatsonReportOnUnhandledException(delegate
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency(ServiceLatencyMetadata.CoreExecutionLatency, delegate()
				{
					response = (TResponse)((object)message.Execute());
				});
			});
			return response;
		}
	}
}

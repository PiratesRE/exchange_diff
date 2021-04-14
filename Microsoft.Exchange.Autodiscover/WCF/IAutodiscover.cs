using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[ServiceContract(Name = "Autodiscover", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public interface IAutodiscover
	{
		[OperationContract(Action = "http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetUserSettings")]
		GetUserSettingsResponseMessage GetUserSettings(GetUserSettingsRequestMessage message);

		[OperationContract(Action = "http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetDomainSettings")]
		GetDomainSettingsResponseMessage GetDomainSettings(GetDomainSettingsRequestMessage message);

		[OperationContract(Action = "http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetFederationInformation")]
		GetFederationInformationResponseMessage GetFederationInformation(GetFederationInformationRequestMessage message);

		[OperationContract(Action = "http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetOrganizationRelationshipSettings")]
		GetOrganizationRelationshipSettingsResponseMessage GetOrganizationRelationshipSettings(GetOrganizationRelationshipSettingsRequestMessage message);
	}
}

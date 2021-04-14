using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(DirectoryPropertyXmlAppAddress))]
	[XmlInclude(typeof(DirectoryPropertyXmlSharedKeyReference))]
	[XmlInclude(typeof(DirectoryPropertyXmlEncryptedSecretKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlKeyDescription))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementUserKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementUserKeySingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantConfiguration))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantConfigurationSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlAny))]
	[XmlInclude(typeof(DirectoryPropertyXmlAuthorizedParty))]
	[XmlInclude(typeof(DirectoryPropertyXmlThrottleLimit))]
	[XmlInclude(typeof(DirectoryPropertyXmlTaskSetScopeReference))]
	[XmlInclude(typeof(DirectoryPropertyXmlSupportRole))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceInstanceMap))]
	[XmlInclude(typeof(DirectoryPropertyXmlValidationError))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceOriginatedResource))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceInstanceInfo))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceInfo))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceEndpoint))]
	[XmlInclude(typeof(DirectoryPropertyXmlAlternativeSecurityId))]
	[XmlInclude(typeof(DirectoryPropertyXmlProvisionedPlan))]
	[XmlInclude(typeof(DirectoryPropertyXmlPropagationTask))]
	[XmlInclude(typeof(DirectoryPropertyXmlMigrationDetail))]
	[XmlInclude(typeof(DirectoryPropertyXmlLicenseUnitsDetail))]
	[XmlInclude(typeof(DirectoryPropertyXmlLicenseUnitsDetailSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlGeographicLocation))]
	[XmlInclude(typeof(DirectoryPropertyXmlDirSyncStatus))]
	[XmlInclude(typeof(DirectoryPropertyXmlDatacenterRedirection))]
	[XmlInclude(typeof(DirectoryPropertyXmlCredential))]
	[XmlInclude(typeof(DirectoryPropertyXmlContextMoveWatermarks))]
	[XmlInclude(typeof(DirectoryPropertyXmlStrongAuthenticationPolicy))]
	[XmlInclude(typeof(DirectoryPropertyXmlContextMoveStatus))]
	[XmlInclude(typeof(DirectoryPropertyXmlContextMoveStatusSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyVerifiedDomain))]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyPartnership))]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyPartnershipSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyUnverifiedDomain))]
	[XmlInclude(typeof(DirectoryPropertyXmlAssignedPlan))]
	[XmlInclude(typeof(DirectoryPropertyXmlAssignedLicense))]
	[XmlInclude(typeof(DirectoryPropertyXmlContextMoveWatermarksSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlAnySingle))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryPropertyXmlSearchForUsers))]
	[XmlInclude(typeof(DirectoryPropertyXmlAsymmetricKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlStrongAuthenticationMethod))]
	[Serializable]
	public abstract class DirectoryPropertyXml : DirectoryProperty
	{
	}
}

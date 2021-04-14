using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(DirectoryPropertyXmlAsymmetricKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementUserKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementUserKeySingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantConfiguration))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantConfigurationSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlStsAddress))]
	[XmlInclude(typeof(DirectoryPropertyXmlAppAddress))]
	[XmlInclude(typeof(DirectoryPropertyXmlAuthorizedParty))]
	[XmlInclude(typeof(DirectoryPropertyXmlValidationError))]
	[XmlInclude(typeof(DirectoryPropertyXmlSharedKeyReference))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceInfo))]
	[XmlInclude(typeof(DirectoryPropertyXmlAssignedRoleSlice))]
	[XmlInclude(typeof(DirectoryPropertyXmlProvisionedPlan))]
	[XmlInclude(typeof(DirectoryPropertyXmlLicenseUnitsDetail))]
	[XmlInclude(typeof(DirectoryPropertyXmlLicenseUnitsDetailSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlDirSyncStatus))]
	[XmlInclude(typeof(DirectoryPropertyXmlCredential))]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyVerifiedDomain))]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyPartnership))]
	[XmlInclude(typeof(DirectoryPropertyXmlCompanyPartnershipSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlAssignedPlan))]
	[XmlInclude(typeof(DirectoryPropertyXmlAny))]
	[XmlInclude(typeof(DirectoryPropertyXmlAnySingle))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlInclude(typeof(DirectoryPropertyXmlTakeoverAction))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceOriginatedResource))]
	[XmlInclude(typeof(DirectoryPropertyXmlEncryptedSecretKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlKeyDescription))]
	[XmlInclude(typeof(DirectoryPropertyXmlEncryptedExternalSecret))]
	[XmlInclude(typeof(DirectoryPropertyXmlEncryptedExternalSecretSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlAppMetadata))]
	[XmlInclude(typeof(DirectoryPropertyXmlAppMetadataSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceDiscoveryEndpoint))]
	[XmlInclude(typeof(DirectoryPropertyXmlStrongAuthenticationRequirement))]
	[XmlInclude(typeof(DirectoryPropertyXmlInvitationTicket))]
	[XmlInclude(typeof(DirectoryPropertyXmlStringKeyValuePair))]
	[XmlInclude(typeof(DirectoryPropertyXmlStrongAuthenticationMethod))]
	[XmlInclude(typeof(DirectoryPropertyXmlStrongAuthenticationPolicy))]
	[XmlInclude(typeof(DirectoryPropertyXmlAlternativeSecurityId))]
	[Serializable]
	public abstract class DirectoryPropertyXml : DirectoryProperty
	{
	}
}

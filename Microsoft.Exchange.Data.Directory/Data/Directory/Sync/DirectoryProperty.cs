using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To20))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To32000))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0))]
	[XmlInclude(typeof(DirectoryPropertyXml))]
	[XmlInclude(typeof(DirectoryPropertyXmlTakeoverAction))]
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
	[XmlInclude(typeof(DirectoryPropertyXmlSharedKeyReference))]
	[XmlInclude(typeof(DirectoryPropertyXmlAsymmetricKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlEncryptedSecretKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlKeyDescription))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementUserKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementUserKeySingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantKey))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantConfiguration))]
	[XmlInclude(typeof(DirectoryPropertyXmlRightsManagementTenantConfigurationSingle))]
	[XmlInclude(typeof(DirectoryPropertyXmlStsAddress))]
	[XmlInclude(typeof(DirectoryPropertyXmlAppAddress))]
	[XmlInclude(typeof(DirectoryPropertyXmlAuthorizedParty))]
	[XmlInclude(typeof(DirectoryPropertyXmlValidationError))]
	[XmlInclude(typeof(DirectoryPropertyXmlServiceOriginatedResource))]
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
	[XmlInclude(typeof(DirectoryPropertyString))]
	[XmlInclude(typeof(DirectoryPropertyServicePrincipalName))]
	[XmlInclude(typeof(DirectoryPropertyProxyAddresses))]
	[XmlInclude(typeof(DirectoryPropertyStringStsTokenType))]
	[XmlInclude(typeof(DirectoryPropertyStringLength2To500))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To1123))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To1024))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To512))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To256))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To100))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To64))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To40))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To3))]
	[XmlInclude(typeof(DirectoryPropertyStringLength1To2048))]
	[XmlInclude(typeof(DirectoryPropertyStringSingle))]
	[XmlInclude(typeof(DirectoryPropertyTargetAddress))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleServiceInstanceSelectionTagPrefix))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleMailNickname))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleColor))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To2048))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To1123))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To1024))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To512))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To500))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To454))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To256))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To128))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To64))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To40))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To16))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To6))]
	[XmlInclude(typeof(DirectoryPropertyStringSingleLength1To3))]
	[XmlInclude(typeof(DirectoryPropertyReference))]
	[XmlInclude(typeof(DirectoryPropertyReferenceUserAndServicePrincipal))]
	[XmlInclude(typeof(DirectoryPropertyReferenceUserAndServicePrincipalSingle))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAddressList))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAddressListSingle))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAny))]
	[XmlInclude(typeof(DirectoryPropertyReferenceAnySingle))]
	[XmlInclude(typeof(DirectoryPropertyInt64))]
	[XmlInclude(typeof(DirectoryPropertyInt64Single))]
	[XmlInclude(typeof(DirectoryPropertyInt32))]
	[XmlInclude(typeof(DirectoryPropertyInt32Single))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin1Max86400))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max65535))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMax1))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max4))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max3))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max2))]
	[XmlInclude(typeof(DirectoryPropertyInt32SingleMin0Max1))]
	[XmlInclude(typeof(DirectoryPropertyGuid))]
	[XmlInclude(typeof(DirectoryPropertyGuidSingle))]
	[XmlInclude(typeof(DirectoryPropertyDateTime))]
	[XmlInclude(typeof(DirectoryPropertyDateTimeSingle))]
	[XmlInclude(typeof(DirectoryPropertyBoolean))]
	[XmlInclude(typeof(DirectoryPropertyBooleanSingle))]
	[XmlInclude(typeof(DirectoryPropertyBinary))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingle))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To102400))]
	[XmlInclude(typeof(DirectoryPropertyBitmap32))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To12000))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To8000))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To4000))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To195))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength1To128))]
	[XmlInclude(typeof(DirectoryPropertyBinarySingleLength8))]
	[XmlInclude(typeof(DirectoryPropertyBinaryLength1To32768))]
	[XmlInclude(typeof(DirectoryPropertyBinaryLength1To2048))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public abstract class DirectoryProperty
	{
		public abstract IList GetValues();

		public abstract void SetValues(IList values);

		[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01")]
		public DateTime Timestamp { get; set; }

		[XmlIgnore]
		public bool TimestampSpecified { get; set; }

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		internal static readonly IList EmptyValues = new object[0];

		private XmlAttribute[] anyAttrField;
	}
}

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class ServicePrincipal : DirectoryObject
	{
		[XmlElement(Order = 0)]
		public DirectoryPropertyBooleanSingle AccountEnabled
		{
			get
			{
				return this.accountEnabledField;
			}
			set
			{
				this.accountEnabledField = value;
			}
		}

		[XmlElement(Order = 1)]
		public DirectoryPropertyXmlAlternativeSecurityId AlternativeSecurityId
		{
			get
			{
				return this.alternativeSecurityIdField;
			}
			set
			{
				this.alternativeSecurityIdField = value;
			}
		}

		[XmlElement(Order = 2)]
		public DirectoryPropertyXmlAppAddress AppAddress
		{
			get
			{
				return this.appAddressField;
			}
			set
			{
				this.appAddressField = value;
			}
		}

		[XmlElement(Order = 3)]
		public DirectoryPropertyGuidSingle AppPrincipalId
		{
			get
			{
				return this.appPrincipalIdField;
			}
			set
			{
				this.appPrincipalIdField = value;
			}
		}

		[XmlElement(Order = 4)]
		public DirectoryPropertyXmlAsymmetricKey AsymmetricKey
		{
			get
			{
				return this.asymmetricKeyField;
			}
			set
			{
				this.asymmetricKeyField = value;
			}
		}

		[XmlElement(Order = 5)]
		public DirectoryPropertyReferenceUserAndServicePrincipalSingle CreatedOnBehalfOf
		{
			get
			{
				return this.createdOnBehalfOfField;
			}
			set
			{
				this.createdOnBehalfOfField = value;
			}
		}

		[XmlElement(Order = 6)]
		public DirectoryPropertyXmlCredential Credential
		{
			get
			{
				return this.credentialField;
			}
			set
			{
				this.credentialField = value;
			}
		}

		[XmlElement(Order = 7)]
		public DirectoryPropertyStringSingleLength1To1024 Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
			}
		}

		[XmlElement(Order = 8)]
		public DirectoryPropertyStringSingleLength1To256 DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		[XmlElement(Order = 9)]
		public DirectoryPropertyXmlEncryptedSecretKey EncryptedSecretKey
		{
			get
			{
				return this.encryptedSecretKeyField;
			}
			set
			{
				this.encryptedSecretKeyField = value;
			}
		}

		[XmlElement(Order = 10)]
		public DirectoryPropertyBooleanSingle ExternalUserAccountDelegationsAllowed
		{
			get
			{
				return this.externalUserAccountDelegationsAllowedField;
			}
			set
			{
				this.externalUserAccountDelegationsAllowedField = value;
			}
		}

		[XmlElement(Order = 11)]
		public DirectoryPropertyXmlKeyDescription KeyDescription
		{
			get
			{
				return this.keyDescriptionField;
			}
			set
			{
				this.keyDescriptionField = value;
			}
		}

		[XmlElement(Order = 12)]
		public DirectoryPropertyBooleanSingle MicrosoftPolicyGroup
		{
			get
			{
				return this.microsoftPolicyGroupField;
			}
			set
			{
				this.microsoftPolicyGroupField = value;
			}
		}

		[XmlElement(Order = 13)]
		public DirectoryPropertyStringSingleLength1To256 PreferredTokenSigningKeyThumbprint
		{
			get
			{
				return this.preferredTokenSigningKeyThumbprintField;
			}
			set
			{
				this.preferredTokenSigningKeyThumbprintField = value;
			}
		}

		[XmlElement(Order = 14)]
		public DirectoryPropertyServicePrincipalName ServicePrincipalName
		{
			get
			{
				return this.servicePrincipalNameField;
			}
			set
			{
				this.servicePrincipalNameField = value;
			}
		}

		[XmlElement(Order = 15)]
		public DirectoryPropertyXmlSharedKeyReference SharedKeyReference
		{
			get
			{
				return this.sharedKeyReferenceField;
			}
			set
			{
				this.sharedKeyReferenceField = value;
			}
		}

		[XmlElement(Order = 16)]
		public DirectoryPropertyBooleanSingle TrustedForDelegation
		{
			get
			{
				return this.trustedForDelegationField;
			}
			set
			{
				this.trustedForDelegationField = value;
			}
		}

		[XmlElement(Order = 17)]
		public DirectoryPropertyBooleanSingle UseCustomTokenSigningKey
		{
			get
			{
				return this.useCustomTokenSigningKeyField;
			}
			set
			{
				this.useCustomTokenSigningKeyField = value;
			}
		}

		[XmlElement(Order = 18)]
		public DirectoryPropertyStringSingleLength1To2048 WwwHomepage
		{
			get
			{
				return this.wwwHomepageField;
			}
			set
			{
				this.wwwHomepageField = value;
			}
		}

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

		internal override void ForEachProperty(IPropertyProcessor processor)
		{
		}

		private DirectoryPropertyBooleanSingle accountEnabledField;

		private DirectoryPropertyXmlAlternativeSecurityId alternativeSecurityIdField;

		private DirectoryPropertyXmlAppAddress appAddressField;

		private DirectoryPropertyGuidSingle appPrincipalIdField;

		private DirectoryPropertyXmlAsymmetricKey asymmetricKeyField;

		private DirectoryPropertyReferenceUserAndServicePrincipalSingle createdOnBehalfOfField;

		private DirectoryPropertyXmlCredential credentialField;

		private DirectoryPropertyStringSingleLength1To1024 descriptionField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyXmlEncryptedSecretKey encryptedSecretKeyField;

		private DirectoryPropertyBooleanSingle externalUserAccountDelegationsAllowedField;

		private DirectoryPropertyXmlKeyDescription keyDescriptionField;

		private DirectoryPropertyBooleanSingle microsoftPolicyGroupField;

		private DirectoryPropertyStringSingleLength1To256 preferredTokenSigningKeyThumbprintField;

		private DirectoryPropertyServicePrincipalName servicePrincipalNameField;

		private DirectoryPropertyXmlSharedKeyReference sharedKeyReferenceField;

		private DirectoryPropertyBooleanSingle trustedForDelegationField;

		private DirectoryPropertyBooleanSingle useCustomTokenSigningKeyField;

		private DirectoryPropertyStringSingleLength1To2048 wwwHomepageField;

		private XmlAttribute[] anyAttrField;
	}
}

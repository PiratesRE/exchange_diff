using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[Serializable]
	public class ServicePrincipal : DirectoryObject
	{
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

		private DirectoryPropertyBooleanSingle accountEnabledField;

		private DirectoryPropertyXmlAppAddress appAddressField;

		private DirectoryPropertyGuidSingle appPrincipalIdField;

		private DirectoryPropertyXmlAsymmetricKey asymmetricKeyField;

		private DirectoryPropertyXmlCredential credentialField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyXmlEncryptedSecretKey encryptedSecretKeyField;

		private DirectoryPropertyBooleanSingle externalUserAccountDelegationsAllowedField;

		private DirectoryPropertyXmlKeyDescription keyDescriptionField;

		private DirectoryPropertyServicePrincipalName servicePrincipalNameField;

		private DirectoryPropertyXmlSharedKeyReference sharedKeyReferenceField;

		private DirectoryPropertyBooleanSingle trustedForDelegationField;

		private XmlAttribute[] anyAttrField;
	}
}

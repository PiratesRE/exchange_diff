using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class PublicFolder : DirectoryObject
	{
		[XmlElement(Order = 0)]
		public DirectoryPropertyBooleanSingle DirSyncEnabled
		{
			get
			{
				return this.dirSyncEnabledField;
			}
			set
			{
				this.dirSyncEnabledField = value;
			}
		}

		[XmlElement(Order = 1)]
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

		[XmlElement(Order = 2)]
		public DirectoryPropertyStringSingleLength1To256 Mail
		{
			get
			{
				return this.mailField;
			}
			set
			{
				this.mailField = value;
			}
		}

		[XmlElement(Order = 3)]
		public DirectoryPropertyStringSingleMailNickname MailNickname
		{
			get
			{
				return this.mailNicknameField;
			}
			set
			{
				this.mailNicknameField = value;
			}
		}

		[XmlElement(Order = 4)]
		public DirectoryPropertyBooleanSingle MSExchBypassAudit
		{
			get
			{
				return this.mSExchBypassAuditField;
			}
			set
			{
				this.mSExchBypassAuditField = value;
			}
		}

		[XmlElement(Order = 5)]
		public DirectoryPropertyBooleanSingle MSExchMailboxAuditEnable
		{
			get
			{
				return this.mSExchMailboxAuditEnableField;
			}
			set
			{
				this.mSExchMailboxAuditEnableField = value;
			}
		}

		[XmlElement(Order = 6)]
		public DirectoryPropertyInt32Single MSExchMailboxAuditLogAgeLimit
		{
			get
			{
				return this.mSExchMailboxAuditLogAgeLimitField;
			}
			set
			{
				this.mSExchMailboxAuditLogAgeLimitField = value;
			}
		}

		[XmlElement(Order = 7)]
		public DirectoryPropertyInt32Single MSExchModerationFlags
		{
			get
			{
				return this.mSExchModerationFlagsField;
			}
			set
			{
				this.mSExchModerationFlagsField = value;
			}
		}

		[XmlElement(Order = 8)]
		public DirectoryPropertyInt64Single MSExchRecipientTypeDetails
		{
			get
			{
				return this.mSExchRecipientTypeDetailsField;
			}
			set
			{
				this.mSExchRecipientTypeDetailsField = value;
			}
		}

		[XmlElement(Order = 9)]
		public DirectoryPropertyInt32Single MSExchRecipientSoftDeletedStatus
		{
			get
			{
				return this.mSExchRecipientSoftDeletedStatusField;
			}
			set
			{
				this.mSExchRecipientSoftDeletedStatusField = value;
			}
		}

		[XmlElement(Order = 10)]
		public DirectoryPropertyInt32Single MSExchTransportRecipientSettingsFlags
		{
			get
			{
				return this.mSExchTransportRecipientSettingsFlagsField;
			}
			set
			{
				this.mSExchTransportRecipientSettingsFlagsField = value;
			}
		}

		[XmlElement(Order = 11)]
		public DirectoryPropertyProxyAddresses ProxyAddresses
		{
			get
			{
				return this.proxyAddressesField;
			}
			set
			{
				this.proxyAddressesField = value;
			}
		}

		[XmlElement(Order = 12)]
		public DirectoryPropertyStringLength1To1123 ShadowProxyAddresses
		{
			get
			{
				return this.shadowProxyAddressesField;
			}
			set
			{
				this.shadowProxyAddressesField = value;
			}
		}

		[XmlElement(Order = 13)]
		public DirectoryPropertyStringSingleLength1To256 SourceAnchor
		{
			get
			{
				return this.sourceAnchorField;
			}
			set
			{
				this.sourceAnchorField = value;
			}
		}

		[XmlElement(Order = 14)]
		public DirectoryPropertyTargetAddress TargetAddress
		{
			get
			{
				return this.targetAddressField;
			}
			set
			{
				this.targetAddressField = value;
			}
		}

		[XmlArrayItem("AttributeSet", IsNullable = false)]
		[XmlArray(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01", Order = 15)]
		public AttributeSet[] SingleAuthorityMetadata
		{
			get
			{
				return this.singleAuthorityMetadataField;
			}
			set
			{
				this.singleAuthorityMetadataField = value;
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
			throw new NotImplementedException();
		}

		private DirectoryPropertyBooleanSingle dirSyncEnabledField;

		private DirectoryPropertyStringSingleLength1To256 displayNameField;

		private DirectoryPropertyStringSingleLength1To256 mailField;

		private DirectoryPropertyStringSingleMailNickname mailNicknameField;

		private DirectoryPropertyBooleanSingle mSExchBypassAuditField;

		private DirectoryPropertyBooleanSingle mSExchMailboxAuditEnableField;

		private DirectoryPropertyInt32Single mSExchMailboxAuditLogAgeLimitField;

		private DirectoryPropertyInt32Single mSExchModerationFlagsField;

		private DirectoryPropertyInt64Single mSExchRecipientTypeDetailsField;

		private DirectoryPropertyInt32Single mSExchRecipientSoftDeletedStatusField;

		private DirectoryPropertyInt32Single mSExchTransportRecipientSettingsFlagsField;

		private DirectoryPropertyProxyAddresses proxyAddressesField;

		private DirectoryPropertyStringLength1To1123 shadowProxyAddressesField;

		private DirectoryPropertyStringSingleLength1To256 sourceAnchorField;

		private DirectoryPropertyTargetAddress targetAddressField;

		private AttributeSet[] singleAuthorityMetadataField;

		private XmlAttribute[] anyAttrField;
	}
}

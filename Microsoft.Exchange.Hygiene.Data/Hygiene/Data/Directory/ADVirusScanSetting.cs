using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADVirusScanSetting : ADObject
	{
		public ADVirusScanSetting()
		{
		}

		internal ADVirusScanSetting(IConfigurationSession session, string tenantId)
		{
			this.m_Session = session;
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal ADVirusScanSetting(string tenantId)
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public override ObjectId Identity
		{
			get
			{
				return base.Id;
			}
		}

		public ObjectId ConfigurationId
		{
			get
			{
				return (ObjectId)this[ADVirusScanSettingSchema.ConfigurationIdProp];
			}
			set
			{
				this[ADVirusScanSettingSchema.ConfigurationIdProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public VirusScanFlags Flags
		{
			get
			{
				return (VirusScanFlags)this[ADVirusScanSettingSchema.FlagsProp];
			}
			set
			{
				this[ADVirusScanSettingSchema.FlagsProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SenderWarningNotificationId
		{
			get
			{
				return (int)this[ADVirusScanSettingSchema.SenderWarningNotificationIdProp];
			}
			set
			{
				this[ADVirusScanSettingSchema.SenderWarningNotificationIdProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SenderRejectionNotificationId
		{
			get
			{
				return (int)this[ADVirusScanSettingSchema.SenderRejectionNotificationIdProp];
			}
			set
			{
				this[ADVirusScanSettingSchema.SenderRejectionNotificationIdProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int RecipientNotificationId
		{
			get
			{
				return (int)this[ADVirusScanSettingSchema.RecipientNotificationIdProp];
			}
			set
			{
				this[ADVirusScanSettingSchema.RecipientNotificationIdProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AdminNotificationAddress
		{
			get
			{
				return (string)this[ADVirusScanSettingSchema.AdminNotificationAddressProp];
			}
			set
			{
				this[ADVirusScanSettingSchema.AdminNotificationAddressProp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OutboundAdminNotificationAddress
		{
			get
			{
				return (string)this[ADVirusScanSettingSchema.OutboundAdminNotificationAddressProp];
			}
			set
			{
				this[ADVirusScanSettingSchema.OutboundAdminNotificationAddressProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADVirusScanSetting.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADVirusScanSetting.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool ShouldValidatePropertyLinkInSameOrganization(ADPropertyDefinition property)
		{
			return false;
		}

		internal override void InitializeSchema()
		{
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
		}

		private static readonly ADVirusScanSettingSchema schema = ObjectSchema.GetInstance<ADVirusScanSettingSchema>();

		private static string mostDerivedClass = "ADVirusScanSetting";
	}
}

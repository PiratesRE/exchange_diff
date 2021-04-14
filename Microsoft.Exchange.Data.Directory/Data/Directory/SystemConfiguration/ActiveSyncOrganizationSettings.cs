using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ActiveSyncOrganizationSettings : ADContainer
	{
		public ActiveSyncOrganizationSettings()
		{
			this.Name = "Mobile Mailbox Settings";
		}

		[Parameter(Mandatory = false)]
		public DeviceAccessLevel DefaultAccessLevel
		{
			get
			{
				return (DeviceAccessLevel)this[ActiveSyncOrganizationSettingsSchema.AccessLevel];
			}
			set
			{
				this[ActiveSyncOrganizationSettingsSchema.AccessLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UserMailInsert
		{
			get
			{
				return (string)this[ActiveSyncOrganizationSettingsSchema.UserMailInsert];
			}
			set
			{
				this[ActiveSyncOrganizationSettingsSchema.UserMailInsert] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowAccessForUnSupportedPlatform
		{
			get
			{
				return (bool)this[ActiveSyncOrganizationSettingsSchema.AllowAccessForUnSupportedPlatform];
			}
			set
			{
				this[ActiveSyncOrganizationSettingsSchema.AllowAccessForUnSupportedPlatform] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpAddress> AdminMailRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[ActiveSyncOrganizationSettingsSchema.AdminMailRecipients];
			}
			set
			{
				this[ActiveSyncOrganizationSettingsSchema.AdminMailRecipients] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OtaNotificationMailInsert
		{
			get
			{
				return (string)this[ActiveSyncOrganizationSettingsSchema.OtaNotificationMailInsert];
			}
			set
			{
				this[ActiveSyncOrganizationSettingsSchema.OtaNotificationMailInsert] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ActiveSyncDeviceFilterArray DeviceFiltering
		{
			get
			{
				return (ActiveSyncDeviceFilterArray)this[ActiveSyncOrganizationSettingsSchema.DeviceFiltering];
			}
			set
			{
				this[ActiveSyncOrganizationSettingsSchema.DeviceFiltering] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ActiveSyncOrganizationSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMobileMailboxSettings";
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public bool IsIntuneManaged { get; set; }

		public const string ContainerName = "Mobile Mailbox Settings";

		internal const string MostDerivedClassName = "msExchMobileMailboxSettings";

		private static ActiveSyncOrganizationSettingsSchema schema = ObjectSchema.GetInstance<ActiveSyncOrganizationSettingsSchema>();
	}
}

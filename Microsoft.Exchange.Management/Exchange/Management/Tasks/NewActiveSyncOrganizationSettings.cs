using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "ActiveSyncOrganizationSettings", SupportsShouldProcess = true)]
	public sealed class NewActiveSyncOrganizationSettings : NewMultitenancyFixedNameSystemConfigurationObjectTask<ActiveSyncOrganizationSettings>
	{
		[Parameter(Mandatory = false)]
		public DeviceAccessLevel DefaultAccessLevel
		{
			get
			{
				return this.DataObject.DefaultAccessLevel;
			}
			set
			{
				this.DataObject.DefaultAccessLevel = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UserMailInsert
		{
			get
			{
				return this.DataObject.UserMailInsert;
			}
			set
			{
				this.DataObject.UserMailInsert = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpAddress> AdminMailRecipients
		{
			get
			{
				return this.DataObject.AdminMailRecipients;
			}
			set
			{
				this.DataObject.AdminMailRecipients = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OtaNotificationMailInsert
		{
			get
			{
				return this.DataObject.OtaNotificationMailInsert;
			}
			set
			{
				this.DataObject.OtaNotificationMailInsert = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowAccessForUnSupportedPlatform
		{
			get
			{
				return this.DataObject.AllowAccessForUnSupportedPlatform;
			}
			set
			{
				this.DataObject.AllowAccessForUnSupportedPlatform = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ActiveSyncOrganizationSettings activeSyncOrganizationSettings = (ActiveSyncOrganizationSettings)base.PrepareDataObject();
			activeSyncOrganizationSettings.SetId((IConfigurationSession)base.DataSession, this.DataObject.Name);
			return activeSyncOrganizationSettings;
		}
	}
}

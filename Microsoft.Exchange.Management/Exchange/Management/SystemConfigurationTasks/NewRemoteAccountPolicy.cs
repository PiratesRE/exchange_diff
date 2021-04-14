using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "RemoteAccountPolicy", SupportsShouldProcess = true)]
	public sealed class NewRemoteAccountPolicy : NewMultitenancySystemConfigurationObjectTask<RemoteAccountPolicy>
	{
		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan PollingInterval
		{
			get
			{
				return this.DataObject.PollingInterval;
			}
			set
			{
				this.DataObject.PollingInterval = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TimeBeforeInactive
		{
			get
			{
				return this.DataObject.TimeBeforeInactive;
			}
			set
			{
				this.DataObject.TimeBeforeInactive = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TimeBeforeDormant
		{
			get
			{
				return this.DataObject.TimeBeforeDormant;
			}
			set
			{
				this.DataObject.TimeBeforeDormant = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxSyncAccounts
		{
			get
			{
				return this.DataObject.MaxSyncAccounts;
			}
			set
			{
				this.DataObject.MaxSyncAccounts = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SyncNowAllowed
		{
			get
			{
				return this.DataObject.SyncNowAllowed;
			}
			set
			{
				this.DataObject.SyncNowAllowed = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRemoteAccountPolicy(base.Name);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			RemoteAccountPolicy remoteAccountPolicy = (RemoteAccountPolicy)base.PrepareDataObject();
			remoteAccountPolicy.SetId((IConfigurationSession)base.DataSession, base.Name);
			return remoteAccountPolicy;
		}
	}
}

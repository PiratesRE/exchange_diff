using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("New", "SyncServiceInstance", SupportsShouldProcess = true)]
	public sealed class NewSyncServiceInstance : NewADTaskBase<SyncServiceInstance>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public ServiceInstanceId Name
		{
			get
			{
				return (ServiceInstanceId)base.Fields[ADObjectSchema.Name];
			}
			set
			{
				base.Fields[ADObjectSchema.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return (AccountPartitionIdParameter)base.Fields[SyncServiceInstanceSchema.AccountPartition];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.AccountPartition] = value;
			}
		}

		[Parameter]
		public Version MinVersion
		{
			get
			{
				return (Version)base.Fields[SyncServiceInstanceSchema.MinVersion];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.MinVersion] = value;
			}
		}

		[Parameter]
		public Version MaxVersion
		{
			get
			{
				return (Version)base.Fields[SyncServiceInstanceSchema.MaxVersion];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.MaxVersion] = value;
			}
		}

		[Parameter]
		public int ActiveInstanceSleepInterval
		{
			get
			{
				return (int)base.Fields[SyncServiceInstanceSchema.ActiveInstanceSleepInterval];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.ActiveInstanceSleepInterval] = value;
			}
		}

		[Parameter]
		public int PassiveInstanceSleepInterval
		{
			get
			{
				return (int)base.Fields[SyncServiceInstanceSchema.PassiveInstanceSleepInterval];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.PassiveInstanceSleepInterval] = value;
			}
		}

		[Parameter]
		public bool IsEnabled
		{
			get
			{
				return (bool)base.Fields[SyncServiceInstanceSchema.IsEnabled];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.IsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version NewTenantMinVersion
		{
			get
			{
				return (Version)base.Fields[SyncServiceInstanceSchema.NewTenantMinVersion];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.NewTenantMinVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version NewTenantMaxVersion
		{
			get
			{
				return (Version)base.Fields[SyncServiceInstanceSchema.NewTenantMaxVersion];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.NewTenantMaxVersion] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewSyncServiceInstance(this.DataObject.Name);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return ForwardSyncDataAccessHelper.CreateSession(false);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			SyncServiceInstance syncServiceInstance = (SyncServiceInstance)base.PrepareDataObject();
			syncServiceInstance.SetId(SyncServiceInstance.GetServiceInstanceObjectId(this.Name.InstanceId));
			if (this.AccountPartition != null)
			{
				AccountPartition accountPartition = (AccountPartition)base.GetDataObject<AccountPartition>(this.AccountPartition, this.ConfigurationSession, null, null, null);
				syncServiceInstance.AccountPartition = accountPartition.Id;
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.MinVersion))
			{
				syncServiceInstance.MinVersion = this.MinVersion;
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.MaxVersion))
			{
				syncServiceInstance.MaxVersion = this.MaxVersion;
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.ActiveInstanceSleepInterval))
			{
				syncServiceInstance.ActiveInstanceSleepInterval = this.ActiveInstanceSleepInterval;
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.PassiveInstanceSleepInterval))
			{
				syncServiceInstance.PassiveInstanceSleepInterval = this.PassiveInstanceSleepInterval;
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.IsEnabled))
			{
				syncServiceInstance.IsEnabled = this.IsEnabled;
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.NewTenantMinVersion))
			{
				syncServiceInstance.NewTenantMinVersion = this.NewTenantMinVersion;
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.NewTenantMaxVersion))
			{
				syncServiceInstance.NewTenantMaxVersion = this.NewTenantMaxVersion;
			}
			syncServiceInstance.IsMultiObjectCookieEnabled = true;
			TaskLogger.LogExit();
			return syncServiceInstance;
		}
	}
}

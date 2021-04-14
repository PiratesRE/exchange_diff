using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class BackgroundJobMgrTimedOutInstance : BackgroundJobBackendBase
	{
		public BackgroundJobMgrTimedOutInstance(Guid roleId, Guid machineId, Guid syncContext, Guid newSyncContext)
		{
			this[BackgroundJobMgrTimedOutInstance.RoleIdProperty] = roleId;
			this[BackgroundJobMgrTimedOutInstance.MachineIdProperty] = machineId;
			this[BackgroundJobMgrTimedOutInstance.SyncContextProperty] = syncContext;
			this[BackgroundJobMgrTimedOutInstance.NewSyncContextProperty] = newSyncContext;
			this.UpdatedInstance = false;
		}

		public Guid RoleId
		{
			get
			{
				return (Guid)this[BackgroundJobMgrTimedOutInstance.RoleIdProperty];
			}
		}

		public Guid MachineId
		{
			get
			{
				return (Guid)this[BackgroundJobMgrTimedOutInstance.MachineIdProperty];
			}
		}

		public Guid SyncContext
		{
			get
			{
				return (Guid)this[BackgroundJobMgrTimedOutInstance.SyncContextProperty];
			}
			set
			{
				this[BackgroundJobMgrTimedOutInstance.SyncContextProperty] = value;
			}
		}

		public Guid NewSyncContext
		{
			get
			{
				return (Guid)this[BackgroundJobMgrTimedOutInstance.NewSyncContextProperty];
			}
			set
			{
				this[BackgroundJobMgrTimedOutInstance.NewSyncContextProperty] = value;
			}
		}

		public bool UpdatedInstance
		{
			get
			{
				return (bool)this[BackgroundJobMgrTimedOutInstance.UpdatedInstanceProperty];
			}
			set
			{
				this[BackgroundJobMgrTimedOutInstance.UpdatedInstanceProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition MachineIdProperty = BackgroundJobMgrInstanceProperties.MachineIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = BackgroundJobMgrInstanceProperties.RoleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SyncContextProperty = BackgroundJobMgrInstanceProperties.SyncContextProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition NewSyncContextProperty = new BackgroundJobBackendPropertyDefinition("NewSyncContext", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition UpdatedInstanceProperty = new BackgroundJobBackendPropertyDefinition("UpdatedInstance", typeof(bool), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, false);
	}
}

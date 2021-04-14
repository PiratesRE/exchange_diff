using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class BackgroundJobMgrHeartBeatUpdate : BackgroundJobBackendBase
	{
		public BackgroundJobMgrHeartBeatUpdate(Guid roleId, Guid machineId, bool active, Guid syncContext, Guid newSyncContext)
		{
			this[BackgroundJobMgrHeartBeatUpdate.RoleIdProperty] = roleId;
			this[BackgroundJobMgrHeartBeatUpdate.MachineIdProperty] = machineId;
			this.HeartBeat = DateTime.UtcNow;
			this.Active = active;
			this[BackgroundJobMgrHeartBeatUpdate.SyncContextProperty] = syncContext;
			this[BackgroundJobMgrHeartBeatUpdate.NewSyncContextProperty] = newSyncContext;
			this.UpdatedInstance = false;
		}

		public Guid RoleId
		{
			get
			{
				return (Guid)this[BackgroundJobMgrHeartBeatUpdate.RoleIdProperty];
			}
		}

		public Guid MachineId
		{
			get
			{
				return (Guid)this[BackgroundJobMgrHeartBeatUpdate.MachineIdProperty];
			}
		}

		public DateTime HeartBeat
		{
			get
			{
				return (DateTime)this[BackgroundJobMgrHeartBeatUpdate.HeartBeatProperty];
			}
			set
			{
				this[BackgroundJobMgrHeartBeatUpdate.HeartBeatProperty] = value;
			}
		}

		public bool Active
		{
			get
			{
				return (bool)this[BackgroundJobMgrHeartBeatUpdate.ActiveProperty];
			}
			set
			{
				this[BackgroundJobMgrHeartBeatUpdate.ActiveProperty] = value;
			}
		}

		public Guid SyncContext
		{
			get
			{
				return (Guid)this[BackgroundJobMgrHeartBeatUpdate.SyncContextProperty];
			}
			set
			{
				this[BackgroundJobMgrHeartBeatUpdate.SyncContextProperty] = value;
			}
		}

		public Guid NewSyncContext
		{
			get
			{
				return (Guid)this[BackgroundJobMgrHeartBeatUpdate.NewSyncContextProperty];
			}
			set
			{
				this[BackgroundJobMgrHeartBeatUpdate.NewSyncContextProperty] = value;
			}
		}

		public bool UpdatedInstance
		{
			get
			{
				return (bool)this[BackgroundJobMgrHeartBeatUpdate.UpdatedInstanceProperty];
			}
			set
			{
				this[BackgroundJobMgrHeartBeatUpdate.UpdatedInstanceProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition MachineIdProperty = BackgroundJobMgrInstanceProperties.MachineIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = BackgroundJobMgrInstanceProperties.RoleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition HeartBeatProperty = BackgroundJobMgrInstanceProperties.HeartBeatProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveProperty = BackgroundJobMgrInstanceProperties.ActiveProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SyncContextProperty = BackgroundJobMgrInstanceProperties.SyncContextProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition NewSyncContextProperty = new BackgroundJobBackendPropertyDefinition("NewSyncContext", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition UpdatedInstanceProperty = new BackgroundJobBackendPropertyDefinition("UpdatedInstance", typeof(bool), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, false);
	}
}

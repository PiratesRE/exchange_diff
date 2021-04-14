using System;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class BackgroundJobMgrInstance : BackgroundJobBackendBase
	{
		public Guid MachineId
		{
			get
			{
				return (Guid)this[BackgroundJobMgrInstance.MachineIdProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.MachineIdProperty] = value;
			}
		}

		public string MachineName
		{
			get
			{
				return (string)this[BackgroundJobMgrInstance.MachineNameProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.MachineNameProperty] = value;
			}
		}

		public Guid RoleId
		{
			get
			{
				return (Guid)this[BackgroundJobMgrInstance.RoleIdProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.RoleIdProperty] = value;
			}
		}

		public DateTime HeartBeat
		{
			get
			{
				return (DateTime)this[BackgroundJobMgrInstance.HeartBeatProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.HeartBeatProperty] = value;
			}
		}

		public bool Active
		{
			get
			{
				return (bool)this[BackgroundJobMgrInstance.ActiveProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.ActiveProperty] = value;
			}
		}

		public long DataCenter
		{
			get
			{
				return (long)this[BackgroundJobMgrInstance.DCProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.DCProperty] = value;
			}
		}

		public Regions Region
		{
			get
			{
				return (Regions)this[BackgroundJobMgrInstance.RegionProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.RegionProperty] = (int)value;
			}
		}

		public Guid SyncContext
		{
			get
			{
				return (Guid)this[BackgroundJobMgrInstance.SyncContextProperty];
			}
			set
			{
				this[BackgroundJobMgrInstance.SyncContextProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition MachineIdProperty = BackgroundJobMgrInstanceProperties.MachineIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition MachineNameProperty = BackgroundJobMgrInstanceProperties.MachineNameProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = BackgroundJobMgrInstanceProperties.RoleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition HeartBeatProperty = BackgroundJobMgrInstanceProperties.HeartBeatProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveProperty = BackgroundJobMgrInstanceProperties.ActiveProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition DCProperty = BackgroundJobMgrInstanceProperties.DCProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RegionProperty = BackgroundJobMgrInstanceProperties.RegionProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SyncContextProperty = BackgroundJobMgrInstanceProperties.SyncContextProperty;
	}
}

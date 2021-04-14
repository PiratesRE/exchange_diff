using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ActiveSyncDeviceAutoblockThreshold : ADConfigurationObject
	{
		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		public AutoblockThresholdType BehaviorType
		{
			get
			{
				return (AutoblockThresholdType)this[ActiveSyncDeviceAutoblockThresholdSchema.BehaviorType];
			}
			internal set
			{
				this[ActiveSyncDeviceAutoblockThresholdSchema.BehaviorType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int BehaviorTypeIncidenceLimit
		{
			get
			{
				return (int)this[ActiveSyncDeviceAutoblockThresholdSchema.BehaviorTypeIncidenceLimit];
			}
			set
			{
				this[ActiveSyncDeviceAutoblockThresholdSchema.BehaviorTypeIncidenceLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan BehaviorTypeIncidenceDuration
		{
			get
			{
				return (EnhancedTimeSpan)this[ActiveSyncDeviceAutoblockThresholdSchema.BehaviorTypeIncidenceDuration];
			}
			set
			{
				this[ActiveSyncDeviceAutoblockThresholdSchema.BehaviorTypeIncidenceDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan DeviceBlockDuration
		{
			get
			{
				return (EnhancedTimeSpan)this[ActiveSyncDeviceAutoblockThresholdSchema.DeviceBlockDuration];
			}
			set
			{
				this[ActiveSyncDeviceAutoblockThresholdSchema.DeviceBlockDuration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AdminEmailInsert
		{
			get
			{
				return (string)this[ActiveSyncDeviceAutoblockThresholdSchema.AdminEmailInsert];
			}
			set
			{
				this[ActiveSyncDeviceAutoblockThresholdSchema.AdminEmailInsert] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ActiveSyncDeviceAutoblockThreshold.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchActiveSyncDeviceAutoblockThreshold";
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return ActiveSyncDeviceAutoblockThreshold.ContainerPath;
			}
		}

		internal const string MostDerivedClass = "msExchActiveSyncDeviceAutoblockThreshold";

		public static ADObjectId ContainerPath = new ADObjectId("CN=Mobile Mailbox Policies");

		private static ActiveSyncDeviceAutoblockThresholdSchema schema = ObjectSchema.GetInstance<ActiveSyncDeviceAutoblockThresholdSchema>();
	}
}

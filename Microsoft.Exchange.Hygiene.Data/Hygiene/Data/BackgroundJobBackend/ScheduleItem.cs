using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class ScheduleItem : BackgroundJobBackendBase
	{
		public Guid ScheduleId
		{
			get
			{
				return (Guid)this[ScheduleItem.ScheduleIdProperty];
			}
			set
			{
				this[ScheduleItem.ScheduleIdProperty] = value;
			}
		}

		public Guid BackgroundJobId
		{
			get
			{
				return (Guid)this[ScheduleItem.BackgroundJobIdProperty];
			}
			set
			{
				this[ScheduleItem.BackgroundJobIdProperty] = value;
			}
		}

		public SchedulingType SchedulingType
		{
			get
			{
				return (SchedulingType)this[ScheduleItem.SchedulingTypeProperty];
			}
			set
			{
				this[ScheduleItem.SchedulingTypeProperty] = value;
			}
		}

		public bool HasStartTime
		{
			get
			{
				return this[ScheduleItem.StartTimeProperty] != null;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return (DateTime)this[ScheduleItem.StartTimeProperty];
			}
			set
			{
				this[ScheduleItem.StartTimeProperty] = value;
			}
		}

		public bool HasSchedulingInterval
		{
			get
			{
				return this[ScheduleItem.SchedulingIntervalProperty] != null;
			}
		}

		public int SchedulingInterval
		{
			get
			{
				return (int)this[ScheduleItem.SchedulingIntervalProperty];
			}
			set
			{
				this[ScheduleItem.SchedulingIntervalProperty] = value;
			}
		}

		public bool HasScheduleDaysSet
		{
			get
			{
				return this[ScheduleItem.ScheduleDaysSetProperty] != null;
			}
		}

		public Days ScheduleDaysSet
		{
			get
			{
				return (Days)this[ScheduleItem.ScheduleDaysSetProperty];
			}
			set
			{
				this[ScheduleItem.ScheduleDaysSetProperty] = (byte)value;
			}
		}

		public bool HasLastScheduledTime
		{
			get
			{
				return this[ScheduleItem.LastScheduledTimeProperty] != null;
			}
		}

		public DateTime LastScheduledTime
		{
			get
			{
				return (DateTime)this[ScheduleItem.LastScheduledTimeProperty];
			}
			set
			{
				this[ScheduleItem.LastScheduledTimeProperty] = value;
			}
		}

		public DateTime CreatedDatetime
		{
			get
			{
				return (DateTime)this[ScheduleItem.CreatedDatetimeProperty];
			}
		}

		public DateTime ChangedDatetime
		{
			get
			{
				return (DateTime)this[ScheduleItem.ChangedDatetimeProperty];
			}
		}

		public long DCSelectionSet
		{
			get
			{
				return (long)this[ScheduleItem.DCSelectionSetProperty];
			}
			set
			{
				this[ScheduleItem.DCSelectionSetProperty] = value;
			}
		}

		public List<long> DCCollectionSet
		{
			get
			{
				return this.dataCenterIdCollection;
			}
			set
			{
				this.dataCenterIdCollection = value;
			}
		}

		public Regions RegionSelectionSet
		{
			get
			{
				return (Regions)this[ScheduleItem.RegionSelectionSetProperty];
			}
			set
			{
				this[ScheduleItem.RegionSelectionSetProperty] = (int)value;
			}
		}

		public string TargetMachineName
		{
			get
			{
				return (string)this[ScheduleItem.TargetMachineNameProperty];
			}
			set
			{
				this[ScheduleItem.TargetMachineNameProperty] = value;
			}
		}

		public byte InstancesToRun
		{
			get
			{
				return (byte)this[ScheduleItem.InstancesToRunProperty];
			}
			set
			{
				this[ScheduleItem.InstancesToRunProperty] = value;
			}
		}

		public Guid RoleId
		{
			get
			{
				return (Guid)this[ScheduleItem.RoleIdProperty];
			}
			set
			{
				this[ScheduleItem.RoleIdProperty] = value;
			}
		}

		public bool SingleInstancePerMachine
		{
			get
			{
				return (bool)this[ScheduleItem.SingleInstancePerMachineProperty];
			}
			set
			{
				this[ScheduleItem.SingleInstancePerMachineProperty] = value;
			}
		}

		public SchedulingStrategyType SchedulingStrategy
		{
			get
			{
				return (SchedulingStrategyType)this[ScheduleItem.SchedulingStrategyProperty];
			}
			set
			{
				this[ScheduleItem.SchedulingStrategyProperty] = (byte)value;
			}
		}

		public int Timeout
		{
			get
			{
				return (int)this[ScheduleItem.TimeoutProperty];
			}
			set
			{
				this[ScheduleItem.TimeoutProperty] = value;
			}
		}

		public byte MaxLocalRetryCount
		{
			get
			{
				return (byte)this[ScheduleItem.MaxLocalRetryCountProperty];
			}
			set
			{
				this[ScheduleItem.MaxLocalRetryCountProperty] = value;
			}
		}

		public short MaxFailoverCount
		{
			get
			{
				return (short)this[ScheduleItem.MaxFailoverCountProperty];
			}
			set
			{
				this[ScheduleItem.MaxFailoverCountProperty] = value;
			}
		}

		public Guid NextActiveJobId
		{
			get
			{
				return (Guid)this[ScheduleItem.NextActiveJobIdProperty];
			}
			set
			{
				this[ScheduleItem.NextActiveJobIdProperty] = value;
			}
		}

		public bool Active
		{
			get
			{
				return (bool)this[ScheduleItem.ActiveProperty];
			}
			set
			{
				this[ScheduleItem.ActiveProperty] = value;
			}
		}

		public void AddDCId(long dcId)
		{
			this.DCSelectionSet |= dcId;
			if (!this.dataCenterIdCollection.Contains(dcId))
			{
				this.dataCenterIdCollection.Add(dcId);
			}
		}

		public bool RemoveDCId(long dcId)
		{
			this.DCSelectionSet &= ~dcId;
			return this.dataCenterIdCollection.Remove(dcId);
		}

		public bool ContainsDCId(long dcId)
		{
			return this.dataCenterIdCollection.Contains(dcId);
		}

		public void ClearDCIds()
		{
			this.dataCenterIdCollection.Clear();
		}

		public void PrepareForSerialization()
		{
			Guid scheduleId = this.ScheduleId;
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (long num in this.dataCenterIdCollection)
			{
				batchPropertyTable.AddPropertyValue(Guid.NewGuid(), DataCenterDefinition.DataCenterIdProperty, num);
			}
			this[ScheduleItem.SchedIdToDCIdMappingProperty] = batchPropertyTable;
		}

		public void Deserialize()
		{
		}

		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleIdProperty = ScheduleItemProperties.ScheduleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = ScheduleItemProperties.BackgroundJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingTypeProperty = ScheduleItemProperties.SchedulingTypeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition StartTimeProperty = ScheduleItemProperties.StartTimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingIntervalProperty = ScheduleItemProperties.SchedulingIntervalProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ScheduleDaysSetProperty = ScheduleItemProperties.ScheduleDaysSetProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition DCSelectionSetProperty = ScheduleItemProperties.DCSelectionSetProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RegionSelectionSetProperty = ScheduleItemProperties.RegionSelectionSetProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TargetMachineNameProperty = ScheduleItemProperties.TargetMachineNameProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition InstancesToRunProperty = ScheduleItemProperties.InstancesToRunProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = ScheduleItemProperties.RoleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SingleInstancePerMachineProperty = ScheduleItemProperties.SingleInstancePerMachineProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingStrategyProperty = ScheduleItemProperties.SchedulingStrategyProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition TimeoutProperty = ScheduleItemProperties.TimeoutProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition MaxLocalRetryCountProperty = ScheduleItemProperties.MaxLocalRetryCountProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition MaxFailoverCountProperty = ScheduleItemProperties.MaxFailoverCountProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition NextActiveJobIdProperty = ScheduleItemProperties.NextActiveJobIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition LastScheduledTimeProperty = ScheduleItemProperties.LastScheduledTimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition CreatedDatetimeProperty = ScheduleItemProperties.CreatedDatetimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ChangedDatetimeProperty = ScheduleItemProperties.ChangedDatetimeProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition ActiveProperty = ScheduleItemProperties.ActiveProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition SchedIdToDCIdMappingProperty = ScheduleItemProperties.SchedIdToDCIdMappingProperty;

		private List<long> dataCenterIdCollection = new List<long>();
	}
}

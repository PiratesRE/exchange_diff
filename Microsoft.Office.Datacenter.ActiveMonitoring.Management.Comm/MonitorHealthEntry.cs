using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common
{
	[Serializable]
	public class MonitorHealthEntry : ConfigurableObject
	{
		public MonitorHealthEntry() : base(new SimpleProviderPropertyBag())
		{
		}

		internal MonitorHealthEntry(string server, RpcGetMonitorHealthStatus.RpcMonitorHealthEntry rpcEntry) : this()
		{
			this.Server = server;
			this.Name = rpcEntry.Name;
			this.TargetResource = rpcEntry.TargetResource;
			this.Description = rpcEntry.Description;
			this.IsHaImpacting = rpcEntry.IsHaImpacting;
			this.RecurranceInterval = rpcEntry.RecurranceInterval;
			this.DefinitionCreatedTime = rpcEntry.DefinitionCreatedTime.ToLocalTime();
			this.HealthSetName = rpcEntry.HealthSetName;
			this.HealthSetDescription = rpcEntry.HealthSetDescription;
			this.HealthGroupName = rpcEntry.HealthGroupName;
			this.ServerComponentName = rpcEntry.ServerComponentName;
			this.AlertValue = this.ParseEnum<MonitorAlertState>(rpcEntry.AlertValue, MonitorAlertState.Unknown);
			this.CurrentHealthSetState = this.ParseEnum<MonitorServerComponentState>(rpcEntry.CurrentHealthSetState, MonitorServerComponentState.Unknown);
			this.FirstAlertObservedTime = rpcEntry.FirstAlertObservedTime.ToLocalTime();
			this.LastTransitionTime = rpcEntry.LastTransitionTime.ToLocalTime();
			this.LastExecutionTime = rpcEntry.LastExecutionTime.ToLocalTime();
			this.LastExecutionResult = this.ParseEnum<ResultType>(rpcEntry.LastExecutionResult, ResultType.Abandoned);
			this.WorkItemId = rpcEntry.WorkItemId;
			this.ResultId = rpcEntry.ResultId;
			this.IsStale = MonitorHealthEntry.IsEntryStale(rpcEntry);
			this.ServicePriority = rpcEntry.ServicePriority;
			this.Error = rpcEntry.Error;
			this.Exception = rpcEntry.Exception;
			this.IsNotified = rpcEntry.IsNotified;
			this.LastFailedProbeId = rpcEntry.LastFailedProbeId;
			this.LastFailedProbeResultId = rpcEntry.LastFailedProbeResultId;
			this[SimpleProviderObjectSchema.Identity] = new MonitorHealthEntry.MonitorHealthEntryId(this.HealthSetName, this.Name, this.TargetResource);
		}

		public string Server
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.Server];
			}
			private set
			{
				this[MonitorHealthEntrySchema.Server] = value;
			}
		}

		public MonitorServerComponentState CurrentHealthSetState
		{
			get
			{
				return (MonitorServerComponentState)this[MonitorHealthEntrySchema.CurrentHealthSetState];
			}
			private set
			{
				this[MonitorHealthEntrySchema.CurrentHealthSetState] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.Name];
			}
			private set
			{
				this[MonitorHealthEntrySchema.Name] = value;
			}
		}

		public string TargetResource
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.TargetResource];
			}
			private set
			{
				this[MonitorHealthEntrySchema.TargetResource] = value;
			}
		}

		public string HealthSetName
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.HealthSetName];
			}
			private set
			{
				this[MonitorHealthEntrySchema.HealthSetName] = value;
			}
		}

		public string HealthGroupName
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.HealthGroupName];
			}
			private set
			{
				this[MonitorHealthEntrySchema.HealthGroupName] = value;
			}
		}

		public MonitorAlertState AlertValue
		{
			get
			{
				return (MonitorAlertState)this[MonitorHealthEntrySchema.AlertValue];
			}
			private set
			{
				this[MonitorHealthEntrySchema.AlertValue] = value;
			}
		}

		public DateTime FirstAlertObservedTime
		{
			get
			{
				return (DateTime)this[MonitorHealthEntrySchema.FirstAlertObservedTime];
			}
			private set
			{
				this[MonitorHealthEntrySchema.FirstAlertObservedTime] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.Description];
			}
			private set
			{
				this[MonitorHealthEntrySchema.Description] = value;
			}
		}

		public bool IsHaImpacting
		{
			get
			{
				return (bool)(this[MonitorHealthEntrySchema.IsHaImpacting] ?? false);
			}
			private set
			{
				this[MonitorHealthEntrySchema.IsHaImpacting] = value;
			}
		}

		public int RecurranceInterval
		{
			get
			{
				return (int)this[MonitorHealthEntrySchema.RecurranceInterval];
			}
			private set
			{
				this[MonitorHealthEntrySchema.RecurranceInterval] = value;
			}
		}

		public DateTime DefinitionCreatedTime
		{
			get
			{
				return (DateTime)this[MonitorHealthEntrySchema.DefinitionCreatedTime];
			}
			private set
			{
				this[MonitorHealthEntrySchema.DefinitionCreatedTime] = value;
			}
		}

		public string HealthSetDescription
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.HealthSetDescription];
			}
			private set
			{
				this[MonitorHealthEntrySchema.HealthSetDescription] = value;
			}
		}

		public string ServerComponentName
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.ServerComponentName];
			}
			private set
			{
				this[MonitorHealthEntrySchema.ServerComponentName] = value;
			}
		}

		public DateTime LastTransitionTime
		{
			get
			{
				return (DateTime)this[MonitorHealthEntrySchema.LastTransitionTime];
			}
			private set
			{
				this[MonitorHealthEntrySchema.LastTransitionTime] = value;
			}
		}

		public DateTime LastExecutionTime
		{
			get
			{
				return (DateTime)this[MonitorHealthEntrySchema.LastExecutionTime];
			}
			private set
			{
				this[MonitorHealthEntrySchema.LastExecutionTime] = value;
			}
		}

		public ResultType LastExecutionResult
		{
			get
			{
				return (ResultType)this[MonitorHealthEntrySchema.LastExecutionResult];
			}
			private set
			{
				this[MonitorHealthEntrySchema.LastExecutionResult] = value;
			}
		}

		public int ResultId
		{
			get
			{
				return (int)this[MonitorHealthEntrySchema.ResultId];
			}
			private set
			{
				this[MonitorHealthEntrySchema.ResultId] = value;
			}
		}

		public int WorkItemId
		{
			get
			{
				return (int)this[MonitorHealthEntrySchema.WorkItemId];
			}
			private set
			{
				this[MonitorHealthEntrySchema.WorkItemId] = value;
			}
		}

		public bool IsStale
		{
			get
			{
				return (bool)(this[MonitorHealthEntrySchema.IsStale] ?? false);
			}
			private set
			{
				this[MonitorHealthEntrySchema.IsStale] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.Error];
			}
			private set
			{
				this[MonitorHealthEntrySchema.Error] = value;
			}
		}

		public string Exception
		{
			get
			{
				return (string)this[MonitorHealthEntrySchema.Exception];
			}
			private set
			{
				this[MonitorHealthEntrySchema.Exception] = value;
			}
		}

		public bool IsNotified
		{
			get
			{
				return (bool)(this[MonitorHealthEntrySchema.IsNotified] ?? false);
			}
			private set
			{
				this[MonitorHealthEntrySchema.IsNotified] = value;
			}
		}

		public int LastFailedProbeId
		{
			get
			{
				return (int)this[MonitorHealthEntrySchema.LastFailedProbeId];
			}
			private set
			{
				this[MonitorHealthEntrySchema.LastFailedProbeId] = value;
			}
		}

		public int LastFailedProbeResultId
		{
			get
			{
				return (int)this[MonitorHealthEntrySchema.LastFailedProbeResultId];
			}
			private set
			{
				this[MonitorHealthEntrySchema.LastFailedProbeResultId] = value;
			}
		}

		public int ServicePriority
		{
			get
			{
				return (int)this[MonitorHealthEntrySchema.ServicePriority];
			}
			private set
			{
				this[MonitorHealthEntrySchema.ServicePriority] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MonitorHealthEntry.schema;
			}
		}

		internal static bool IsEntryStale(RpcGetMonitorHealthStatus.RpcMonitorHealthEntry entry)
		{
			bool result = false;
			DateTime d = DateTime.UtcNow.ToLocalTime();
			if (entry.RecurranceInterval > 0)
			{
				DateTime dateTime = entry.LastExecutionTime;
				if (entry.DefinitionCreatedTime > dateTime)
				{
					dateTime = entry.DefinitionCreatedTime;
				}
				TimeSpan t = TimeSpan.FromSeconds((double)(entry.RecurranceInterval + entry.TimeoutSeconds + 120));
				if (dateTime < d - t)
				{
					result = true;
				}
			}
			return result;
		}

		internal T ParseEnum<T>(string strEnum, T defaultValue) where T : struct
		{
			T result = defaultValue;
			if (!string.IsNullOrEmpty(strEnum) && !Enum.TryParse<T>(strEnum, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		private const int StaleGracePeriodInSeconds = 120;

		private static MonitorHealthEntrySchema schema = ObjectSchema.GetInstance<MonitorHealthEntrySchema>();

		[Serializable]
		public class MonitorHealthEntryId : ObjectId
		{
			public MonitorHealthEntryId(string healthSetName, string monitorName, string targetResource)
			{
				this.identity = string.Format("{0}\\{1}\\{2}", healthSetName, monitorName, targetResource);
			}

			public override string ToString()
			{
				return this.identity;
			}

			public override byte[] GetBytes()
			{
				return Encoding.Unicode.GetBytes(this.ToString());
			}

			private readonly string identity;
		}
	}
}

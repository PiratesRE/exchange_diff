using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public sealed class MonitorResult : WorkItemResult, IPersistence, IWorkItemResultSerialization
	{
		public MonitorResult(WorkDefinition definition) : base(definition)
		{
			this.LastFailedProbeId = -1;
			this.LastFailedProbeResultId = -1;
			this.HealthStateTransitionId = -1;
			this.Component = ((MonitorDefinition)definition).Component;
			this.IsHaImpacting = ((MonitorDefinition)definition).IsHaImpacting;
			this.SourceScope = ((MonitorDefinition)definition).SourceScope;
			this.TargetScopes = ((MonitorDefinition)definition).TargetScopes;
			this.HaScope = ((MonitorDefinition)definition).HaScope;
		}

		public MonitorResult()
		{
		}

		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public override int ResultId { get; protected internal set; }

		[Column]
		public override string ServiceName { get; set; }

		[Column]
		public override bool IsNotified { get; set; }

		[Column]
		public override string ResultName { get; set; }

		[Column]
		public override int WorkItemId { get; internal set; }

		[Column]
		public override int DeploymentId { get; internal set; }

		[Column]
		public override string MachineName { get; internal set; }

		[Column]
		public override string Error { get; set; }

		[Column]
		public override string Exception { get; set; }

		[Column]
		public override byte RetryCount { get; internal set; }

		[Column]
		public override string StateAttribute1 { get; set; }

		[Column]
		public override string StateAttribute2 { get; set; }

		[Column]
		public override string StateAttribute3 { get; set; }

		[Column]
		public override string StateAttribute4 { get; set; }

		[Column]
		public override string StateAttribute5 { get; set; }

		[Column]
		public override double StateAttribute6 { get; set; }

		[Column]
		public override double StateAttribute7 { get; set; }

		[Column]
		public override double StateAttribute8 { get; set; }

		[Column]
		public override double StateAttribute9 { get; set; }

		[Column]
		public override double StateAttribute10 { get; set; }

		[Column]
		public override ResultType ResultType { get; set; }

		[Column]
		public override int ExecutionId { get; protected set; }

		[Column]
		public override DateTime ExecutionStartTime { get; set; }

		[Column]
		public override DateTime ExecutionEndTime { get; set; }

		[Column]
		public override byte PoisonedCount { get; set; }

		[Column]
		public bool IsAlert { get; set; }

		[Column]
		public double TotalValue { get; set; }

		[Column]
		public int TotalSampleCount { get; set; }

		[Column]
		public int TotalFailedCount { get; set; }

		[Column]
		public double NewValue { get; set; }

		[Column]
		public int NewSampleCount { get; set; }

		[Column]
		public int NewFailedCount { get; set; }

		[Column]
		public int LastFailedProbeId { get; set; }

		[Column]
		public int LastFailedProbeResultId { get; set; }

		[Column]
		public ServiceHealthStatus HealthState { get; set; }

		[Column]
		public int HealthStateTransitionId { get; set; }

		[Column]
		public DateTime? HealthStateChangedTime { get; set; }

		[Column]
		public DateTime? FirstAlertObservedTime { get; set; }

		[Column]
		public DateTime? FirstInsufficientSamplesObservedTime { get; set; }

		public Component Component { get; set; }

		[Column]
		public string NewStateAttribute1Value { get; set; }

		[Column]
		public int NewStateAttribute1Count { get; set; }

		[Column]
		public double NewStateAttribute1Percent { get; set; }

		[Column]
		public string TotalStateAttribute1Value { get; set; }

		[Column]
		public int TotalStateAttribute1Count { get; set; }

		[Column]
		public double TotalStateAttribute1Percent { get; set; }

		[Column]
		public int NewFailureCategoryValue { get; set; }

		[Column]
		public int NewFailureCategoryCount { get; set; }

		[Column]
		public double NewFailureCategoryPercent { get; set; }

		[Column]
		public int TotalFailureCategoryValue { get; set; }

		[Column]
		public int TotalFailureCategoryCount { get; set; }

		[Column]
		public double TotalFailureCategoryPercent { get; set; }

		[Column]
		internal string ComponentName
		{
			get
			{
				if (!(this.Component != null))
				{
					return string.Empty;
				}
				return this.Component.ToString();
			}
			set
			{
				this.Component = new Component(value);
			}
		}

		[Column]
		internal bool IsHaImpacting { get; set; }

		[Column]
		public string SourceScope { get; set; }

		[Column]
		public string TargetScopes { get; set; }

		[Column]
		public string HaScope { get; set; }

		[Column]
		internal override int Version { get; set; }

		public HaScopeEnum GetHaScope()
		{
			return MonitorDefinition.HaScopeToEnum(this.HaScope);
		}

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		internal static int SchemaVersion
		{
			get
			{
				return MonitorResult.schemaVersion;
			}
		}

		internal static Action<MonitorResult> ResultWriter { private get; set; }

		public void Initialize(Dictionary<string, string> propertyBag, LocalDataAccessMetaData metaData)
		{
			this.LocalDataAccessMetaData = metaData;
			this.SetProperties(propertyBag);
		}

		public void SetProperties(Dictionary<string, string> propertyBag)
		{
			string text;
			if (propertyBag.TryGetValue("ResultId", out text) && !string.IsNullOrEmpty(text))
			{
				this.ResultId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ServiceName", out text))
			{
				this.ServiceName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("IsNotified", out text) && !string.IsNullOrEmpty(text))
			{
				this.IsNotified = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("ResultName", out text))
			{
				this.ResultName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("WorkItemId", out text) && !string.IsNullOrEmpty(text))
			{
				this.WorkItemId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("DeploymentId", out text) && !string.IsNullOrEmpty(text))
			{
				this.DeploymentId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("MachineName", out text))
			{
				this.MachineName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Error", out text))
			{
				this.Error = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Exception", out text))
			{
				this.Exception = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("RetryCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.RetryCount = byte.Parse(text);
			}
			if (propertyBag.TryGetValue("StateAttribute1", out text))
			{
				this.StateAttribute1 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute2", out text))
			{
				this.StateAttribute2 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute3", out text))
			{
				this.StateAttribute3 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute4", out text))
			{
				this.StateAttribute4 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute5", out text))
			{
				this.StateAttribute5 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute6", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute6 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute7", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute7 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute8", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute8 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute9", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute9 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute10", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute10 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("ResultType", out text) && !string.IsNullOrEmpty(text))
			{
				this.ResultType = (ResultType)Enum.Parse(typeof(ResultType), text);
			}
			if (propertyBag.TryGetValue("ExecutionId", out text) && !string.IsNullOrEmpty(text))
			{
				this.ExecutionId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ExecutionStartTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.ExecutionStartTime = DateTime.Parse(text).ToUniversalTime();
			}
			if (propertyBag.TryGetValue("ExecutionEndTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.ExecutionEndTime = DateTime.Parse(text).ToUniversalTime();
			}
			if (propertyBag.TryGetValue("PoisonedCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.PoisonedCount = byte.Parse(text);
			}
			if (propertyBag.TryGetValue("IsAlert", out text) && !string.IsNullOrEmpty(text))
			{
				this.IsAlert = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("TotalValue", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalValue = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("TotalSampleCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalSampleCount = int.Parse(text);
			}
			if (propertyBag.TryGetValue("TotalFailedCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalFailedCount = int.Parse(text);
			}
			if (propertyBag.TryGetValue("NewValue", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewValue = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("NewSampleCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewSampleCount = int.Parse(text);
			}
			if (propertyBag.TryGetValue("NewFailedCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewFailedCount = int.Parse(text);
			}
			if (propertyBag.TryGetValue("LastFailedProbeId", out text) && !string.IsNullOrEmpty(text))
			{
				this.LastFailedProbeId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("LastFailedProbeResultId", out text) && !string.IsNullOrEmpty(text))
			{
				this.LastFailedProbeResultId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("HealthState", out text) && !string.IsNullOrEmpty(text))
			{
				this.HealthState = (ServiceHealthStatus)Enum.Parse(typeof(ServiceHealthStatus), text);
			}
			if (propertyBag.TryGetValue("HealthStateTransitionId", out text) && !string.IsNullOrEmpty(text))
			{
				this.HealthStateTransitionId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("HealthStateChangedTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.HealthStateChangedTime = (string.Equals(text, "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(text).ToUniversalTime()));
			}
			if (propertyBag.TryGetValue("FirstAlertObservedTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.FirstAlertObservedTime = (string.Equals(text, "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(text).ToUniversalTime()));
			}
			if (propertyBag.TryGetValue("FirstInsufficientSamplesObservedTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.FirstInsufficientSamplesObservedTime = (string.Equals(text, "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(text).ToUniversalTime()));
			}
			if (propertyBag.TryGetValue("NewStateAttribute1Value", out text))
			{
				this.NewStateAttribute1Value = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("NewStateAttribute1Count", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewStateAttribute1Count = int.Parse(text);
			}
			if (propertyBag.TryGetValue("NewStateAttribute1Percent", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewStateAttribute1Percent = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("TotalStateAttribute1Value", out text))
			{
				this.TotalStateAttribute1Value = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TotalStateAttribute1Count", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalStateAttribute1Count = int.Parse(text);
			}
			if (propertyBag.TryGetValue("TotalStateAttribute1Percent", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalStateAttribute1Percent = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("NewFailureCategoryValue", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewFailureCategoryValue = int.Parse(text);
			}
			if (propertyBag.TryGetValue("NewFailureCategoryCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewFailureCategoryCount = int.Parse(text);
			}
			if (propertyBag.TryGetValue("NewFailureCategoryPercent", out text) && !string.IsNullOrEmpty(text))
			{
				this.NewFailureCategoryPercent = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("TotalFailureCategoryValue", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalFailureCategoryValue = int.Parse(text);
			}
			if (propertyBag.TryGetValue("TotalFailureCategoryCount", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalFailureCategoryCount = int.Parse(text);
			}
			if (propertyBag.TryGetValue("TotalFailureCategoryPercent", out text) && !string.IsNullOrEmpty(text))
			{
				this.TotalFailureCategoryPercent = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("ComponentName", out text))
			{
				this.ComponentName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("IsHaImpacting", out text) && !string.IsNullOrEmpty(text))
			{
				this.IsHaImpacting = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("SourceScope", out text))
			{
				this.SourceScope = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetScopes", out text))
			{
				this.TargetScopes = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("HaScope", out text))
			{
				this.HaScope = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Version", out text) && !string.IsNullOrEmpty(text))
			{
				this.Version = int.Parse(text);
			}
		}

		public override void AssignResultId()
		{
			if (this.ResultId == 0)
			{
				this.ResultId = MonitorResult.idGenerator.NextId();
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			this.AssignResultId();
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			if (MonitorResult.ResultWriter != null)
			{
				MonitorResult.ResultWriter(this);
				return;
			}
			NativeMethods.MonitorResultUnmanaged monitorResultUnmanaged = this.ToUnmanaged();
			ResultSeverityLevel severity = CrimsonHelper.ConvertResultTypeToSeverityLevel(this.ResultType);
			NativeMethods.WriteMonitorResult(ref monitorResultUnmanaged, severity);
			LocalDataAccess.MonitorResultLogger.LogEvent(DateTime.UtcNow, this.ToDictionary());
		}

		public string Serialize()
		{
			return CrimsonHelper.Serialize(this.ToDictionary(), false);
		}

		public void Deserialize(string result)
		{
			string[] array = CrimsonHelper.ClearResultString(result).Split(new char[]
			{
				'|'
			});
			if (!string.IsNullOrEmpty(array[0]))
			{
				this.ResultId = int.Parse(array[0]);
			}
			this.ServiceName = CrimsonHelper.NullDecode(array[1]);
			if (!string.IsNullOrEmpty(array[2]))
			{
				this.IsNotified = bool.Parse(array[2]);
			}
			this.ResultName = CrimsonHelper.NullDecode(array[3]);
			if (!string.IsNullOrEmpty(array[4]))
			{
				this.WorkItemId = int.Parse(array[4]);
			}
			if (!string.IsNullOrEmpty(array[5]))
			{
				this.DeploymentId = int.Parse(array[5]);
			}
			this.MachineName = CrimsonHelper.NullDecode(array[6]);
			this.Error = CrimsonHelper.NullDecode(array[7]);
			this.Exception = CrimsonHelper.NullDecode(array[8]);
			if (!string.IsNullOrEmpty(array[9]))
			{
				this.RetryCount = byte.Parse(array[9]);
			}
			this.StateAttribute1 = CrimsonHelper.NullDecode(array[10]);
			this.StateAttribute2 = CrimsonHelper.NullDecode(array[11]);
			this.StateAttribute3 = CrimsonHelper.NullDecode(array[12]);
			this.StateAttribute4 = CrimsonHelper.NullDecode(array[13]);
			this.StateAttribute5 = CrimsonHelper.NullDecode(array[14]);
			if (!string.IsNullOrEmpty(array[15]))
			{
				this.StateAttribute6 = CrimsonHelper.ParseDouble(array[15]);
			}
			if (!string.IsNullOrEmpty(array[16]))
			{
				this.StateAttribute7 = CrimsonHelper.ParseDouble(array[16]);
			}
			if (!string.IsNullOrEmpty(array[17]))
			{
				this.StateAttribute8 = CrimsonHelper.ParseDouble(array[17]);
			}
			if (!string.IsNullOrEmpty(array[18]))
			{
				this.StateAttribute9 = CrimsonHelper.ParseDouble(array[18]);
			}
			if (!string.IsNullOrEmpty(array[19]))
			{
				this.StateAttribute10 = CrimsonHelper.ParseDouble(array[19]);
			}
			if (!string.IsNullOrEmpty(array[20]))
			{
				this.ResultType = (ResultType)Enum.Parse(typeof(ResultType), array[20]);
			}
			if (!string.IsNullOrEmpty(array[21]))
			{
				this.ExecutionId = int.Parse(array[21]);
			}
			if (!string.IsNullOrEmpty(array[22]))
			{
				this.ExecutionStartTime = DateTime.Parse(array[22]).ToUniversalTime();
			}
			if (!string.IsNullOrEmpty(array[23]))
			{
				this.ExecutionEndTime = DateTime.Parse(array[23]).ToUniversalTime();
			}
			if (!string.IsNullOrEmpty(array[24]))
			{
				this.PoisonedCount = byte.Parse(array[24]);
			}
			if (!string.IsNullOrEmpty(array[25]))
			{
				this.IsAlert = bool.Parse(array[25]);
			}
			if (!string.IsNullOrEmpty(array[26]))
			{
				this.TotalValue = CrimsonHelper.ParseDouble(array[26]);
			}
			if (!string.IsNullOrEmpty(array[27]))
			{
				this.TotalSampleCount = int.Parse(array[27]);
			}
			if (!string.IsNullOrEmpty(array[28]))
			{
				this.TotalFailedCount = int.Parse(array[28]);
			}
			if (!string.IsNullOrEmpty(array[29]))
			{
				this.NewValue = CrimsonHelper.ParseDouble(array[29]);
			}
			if (!string.IsNullOrEmpty(array[30]))
			{
				this.NewSampleCount = int.Parse(array[30]);
			}
			if (!string.IsNullOrEmpty(array[31]))
			{
				this.NewFailedCount = int.Parse(array[31]);
			}
			if (!string.IsNullOrEmpty(array[32]))
			{
				this.LastFailedProbeId = int.Parse(array[32]);
			}
			if (!string.IsNullOrEmpty(array[33]))
			{
				this.LastFailedProbeResultId = int.Parse(array[33]);
			}
			if (!string.IsNullOrEmpty(array[34]))
			{
				this.HealthState = (ServiceHealthStatus)Enum.Parse(typeof(ServiceHealthStatus), array[34]);
			}
			if (!string.IsNullOrEmpty(array[35]))
			{
				this.HealthStateTransitionId = int.Parse(array[35]);
			}
			if (!string.IsNullOrEmpty(array[36]))
			{
				this.HealthStateChangedTime = (string.Equals(array[36], "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(array[36]).ToUniversalTime()));
			}
			if (!string.IsNullOrEmpty(array[37]))
			{
				this.FirstAlertObservedTime = (string.Equals(array[37], "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(array[37]).ToUniversalTime()));
			}
			if (!string.IsNullOrEmpty(array[38]))
			{
				this.FirstInsufficientSamplesObservedTime = (string.Equals(array[38], "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(array[38]).ToUniversalTime()));
			}
			this.NewStateAttribute1Value = CrimsonHelper.NullDecode(array[39]);
			if (!string.IsNullOrEmpty(array[40]))
			{
				this.NewStateAttribute1Count = int.Parse(array[40]);
			}
			if (!string.IsNullOrEmpty(array[41]))
			{
				this.NewStateAttribute1Percent = CrimsonHelper.ParseDouble(array[41]);
			}
			this.TotalStateAttribute1Value = CrimsonHelper.NullDecode(array[42]);
			if (!string.IsNullOrEmpty(array[43]))
			{
				this.TotalStateAttribute1Count = int.Parse(array[43]);
			}
			if (!string.IsNullOrEmpty(array[44]))
			{
				this.TotalStateAttribute1Percent = CrimsonHelper.ParseDouble(array[44]);
			}
			if (!string.IsNullOrEmpty(array[45]))
			{
				this.NewFailureCategoryValue = int.Parse(array[45]);
			}
			if (!string.IsNullOrEmpty(array[46]))
			{
				this.NewFailureCategoryCount = int.Parse(array[46]);
			}
			if (!string.IsNullOrEmpty(array[47]))
			{
				this.NewFailureCategoryPercent = CrimsonHelper.ParseDouble(array[47]);
			}
			if (!string.IsNullOrEmpty(array[48]))
			{
				this.TotalFailureCategoryValue = int.Parse(array[48]);
			}
			if (!string.IsNullOrEmpty(array[49]))
			{
				this.TotalFailureCategoryCount = int.Parse(array[49]);
			}
			if (!string.IsNullOrEmpty(array[50]))
			{
				this.TotalFailureCategoryPercent = CrimsonHelper.ParseDouble(array[50]);
			}
			this.ComponentName = CrimsonHelper.NullDecode(array[51]);
			if (!string.IsNullOrEmpty(array[52]))
			{
				this.IsHaImpacting = bool.Parse(array[52]);
			}
			this.SourceScope = CrimsonHelper.NullDecode(array[53]);
			this.TargetScopes = CrimsonHelper.NullDecode(array[54]);
			this.HaScope = CrimsonHelper.NullDecode(array[55]);
			if (!string.IsNullOrEmpty(array[56]))
			{
				this.Version = int.Parse(array[56]);
			}
		}

		internal NativeMethods.MonitorResultUnmanaged ToUnmanaged()
		{
			return new NativeMethods.MonitorResultUnmanaged
			{
				ResultId = this.ResultId,
				ServiceName = CrimsonHelper.NullCode(this.ServiceName),
				IsNotified = (this.IsNotified ? 1 : 0),
				ResultName = CrimsonHelper.NullCode(this.ResultName),
				WorkItemId = this.WorkItemId,
				DeploymentId = this.DeploymentId,
				MachineName = CrimsonHelper.NullCode(this.MachineName),
				Error = CrimsonHelper.NullCode(this.Error),
				Exception = CrimsonHelper.NullCode(this.Exception),
				RetryCount = this.RetryCount,
				StateAttribute1 = CrimsonHelper.NullCode(this.StateAttribute1),
				StateAttribute2 = CrimsonHelper.NullCode(this.StateAttribute2),
				StateAttribute3 = CrimsonHelper.NullCode(this.StateAttribute3),
				StateAttribute4 = CrimsonHelper.NullCode(this.StateAttribute4),
				StateAttribute5 = CrimsonHelper.NullCode(this.StateAttribute5),
				StateAttribute6 = this.StateAttribute6,
				StateAttribute7 = this.StateAttribute7,
				StateAttribute8 = this.StateAttribute8,
				StateAttribute9 = this.StateAttribute9,
				StateAttribute10 = this.StateAttribute10,
				ResultType = this.ResultType,
				ExecutionId = this.ExecutionId,
				ExecutionStartTime = this.ExecutionStartTime.ToUniversalTime().ToString("o"),
				ExecutionEndTime = this.ExecutionEndTime.ToUniversalTime().ToString("o"),
				PoisonedCount = this.PoisonedCount,
				IsAlert = (this.IsAlert ? 1 : 0),
				TotalValue = this.TotalValue,
				TotalSampleCount = this.TotalSampleCount,
				TotalFailedCount = this.TotalFailedCount,
				NewValue = this.NewValue,
				NewSampleCount = this.NewSampleCount,
				NewFailedCount = this.NewFailedCount,
				LastFailedProbeId = this.LastFailedProbeId,
				LastFailedProbeResultId = this.LastFailedProbeResultId,
				HealthState = this.HealthState,
				HealthStateTransitionId = this.HealthStateTransitionId,
				HealthStateChangedTime = ((this.HealthStateChangedTime != null) ? this.HealthStateChangedTime.Value.ToUniversalTime().ToString("o") : "[null]"),
				FirstAlertObservedTime = ((this.FirstAlertObservedTime != null) ? this.FirstAlertObservedTime.Value.ToUniversalTime().ToString("o") : "[null]"),
				FirstInsufficientSamplesObservedTime = ((this.FirstInsufficientSamplesObservedTime != null) ? this.FirstInsufficientSamplesObservedTime.Value.ToUniversalTime().ToString("o") : "[null]"),
				NewStateAttribute1Value = CrimsonHelper.NullCode(this.NewStateAttribute1Value),
				NewStateAttribute1Count = this.NewStateAttribute1Count,
				NewStateAttribute1Percent = this.NewStateAttribute1Percent,
				TotalStateAttribute1Value = CrimsonHelper.NullCode(this.TotalStateAttribute1Value),
				TotalStateAttribute1Count = this.TotalStateAttribute1Count,
				TotalStateAttribute1Percent = this.TotalStateAttribute1Percent,
				NewFailureCategoryValue = this.NewFailureCategoryValue,
				NewFailureCategoryCount = this.NewFailureCategoryCount,
				NewFailureCategoryPercent = this.NewFailureCategoryPercent,
				TotalFailureCategoryValue = this.TotalFailureCategoryValue,
				TotalFailureCategoryCount = this.TotalFailureCategoryCount,
				TotalFailureCategoryPercent = this.TotalFailureCategoryPercent,
				ComponentName = CrimsonHelper.NullCode(this.ComponentName),
				IsHaImpacting = (this.IsHaImpacting ? 1 : 0),
				SourceScope = CrimsonHelper.NullCode(this.SourceScope),
				TargetScopes = CrimsonHelper.NullCode(this.TargetScopes),
				HaScope = CrimsonHelper.NullCode(this.HaScope),
				Version = this.Version
			};
		}

		internal Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object>(50)
			{
				{
					"ResultId",
					this.ResultId
				},
				{
					"ServiceName",
					this.ServiceName
				},
				{
					"IsNotified",
					this.IsNotified
				},
				{
					"ResultName",
					this.ResultName
				},
				{
					"WorkItemId",
					this.WorkItemId
				},
				{
					"DeploymentId",
					this.DeploymentId
				},
				{
					"MachineName",
					this.MachineName
				},
				{
					"Error",
					this.Error
				},
				{
					"Exception",
					this.Exception
				},
				{
					"RetryCount",
					this.RetryCount
				},
				{
					"StateAttribute1",
					this.StateAttribute1
				},
				{
					"StateAttribute2",
					this.StateAttribute2
				},
				{
					"StateAttribute3",
					this.StateAttribute3
				},
				{
					"StateAttribute4",
					this.StateAttribute4
				},
				{
					"StateAttribute5",
					this.StateAttribute5
				},
				{
					"StateAttribute6",
					this.StateAttribute6
				},
				{
					"StateAttribute7",
					this.StateAttribute7
				},
				{
					"StateAttribute8",
					this.StateAttribute8
				},
				{
					"StateAttribute9",
					this.StateAttribute9
				},
				{
					"StateAttribute10",
					this.StateAttribute10
				},
				{
					"ResultType",
					this.ResultType
				},
				{
					"ExecutionId",
					this.ExecutionId
				},
				{
					"ExecutionStartTime",
					this.ExecutionStartTime
				},
				{
					"ExecutionEndTime",
					this.ExecutionEndTime
				},
				{
					"PoisonedCount",
					this.PoisonedCount
				},
				{
					"IsAlert",
					this.IsAlert
				},
				{
					"TotalValue",
					this.TotalValue
				},
				{
					"TotalSampleCount",
					this.TotalSampleCount
				},
				{
					"TotalFailedCount",
					this.TotalFailedCount
				},
				{
					"NewValue",
					this.NewValue
				},
				{
					"NewSampleCount",
					this.NewSampleCount
				},
				{
					"NewFailedCount",
					this.NewFailedCount
				},
				{
					"LastFailedProbeId",
					this.LastFailedProbeId
				},
				{
					"LastFailedProbeResultId",
					this.LastFailedProbeResultId
				},
				{
					"HealthState",
					this.HealthState
				},
				{
					"HealthStateTransitionId",
					this.HealthStateTransitionId
				},
				{
					"HealthStateChangedTime",
					this.HealthStateChangedTime
				},
				{
					"FirstAlertObservedTime",
					this.FirstAlertObservedTime
				},
				{
					"FirstInsufficientSamplesObservedTime",
					this.FirstInsufficientSamplesObservedTime
				},
				{
					"NewStateAttribute1Value",
					this.NewStateAttribute1Value
				},
				{
					"NewStateAttribute1Count",
					this.NewStateAttribute1Count
				},
				{
					"NewStateAttribute1Percent",
					this.NewStateAttribute1Percent
				},
				{
					"TotalStateAttribute1Value",
					this.TotalStateAttribute1Value
				},
				{
					"TotalStateAttribute1Count",
					this.TotalStateAttribute1Count
				},
				{
					"TotalStateAttribute1Percent",
					this.TotalStateAttribute1Percent
				},
				{
					"NewFailureCategoryValue",
					this.NewFailureCategoryValue
				},
				{
					"NewFailureCategoryCount",
					this.NewFailureCategoryCount
				},
				{
					"NewFailureCategoryPercent",
					this.NewFailureCategoryPercent
				},
				{
					"TotalFailureCategoryValue",
					this.TotalFailureCategoryValue
				},
				{
					"TotalFailureCategoryCount",
					this.TotalFailureCategoryCount
				},
				{
					"TotalFailureCategoryPercent",
					this.TotalFailureCategoryPercent
				},
				{
					"ComponentName",
					this.ComponentName
				},
				{
					"IsHaImpacting",
					this.IsHaImpacting
				},
				{
					"SourceScope",
					this.SourceScope
				},
				{
					"TargetScopes",
					this.TargetScopes
				},
				{
					"HaScope",
					this.HaScope
				},
				{
					"Version",
					this.Version
				}
			};
		}

		private static int schemaVersion = 65536;

		private static MonitorResultIdGenerator idGenerator = new MonitorResultIdGenerator();
	}
}

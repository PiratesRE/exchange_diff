using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public sealed class ResponderResult : WorkItemResult, IPersistence, IWorkItemResultSerialization
	{
		public ResponderResult(WorkDefinition definition) : base(definition)
		{
		}

		public ResponderResult()
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
		public bool IsThrottled { get; set; }

		[Column]
		public string ResponseResource { get; set; }

		[Column]
		public string ResponseAction { get; set; }

		[Column]
		public ServiceHealthStatus TargetHealthState { get; set; }

		[Column]
		public int TargetHealthStateTransitionId { get; set; }

		[Column]
		public DateTime? FirstAlertObservedTime { get; set; }

		[Column]
		public ServiceRecoveryResult RecoveryResult { get; set; }

		[Column]
		public bool IsRecoveryAttempted { get; set; }

		[Column]
		public string CorrelationResultsXml { get; set; }

		[Column]
		public CorrelatedMonitorAction CorrelationAction { get; set; }

		[Column]
		internal override int Version { get; set; }

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		internal static int SchemaVersion
		{
			get
			{
				return ResponderResult.schemaVersion;
			}
		}

		internal static Action<ResponderResult> ResultWriter { private get; set; }

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
			if (propertyBag.TryGetValue("IsThrottled", out text) && !string.IsNullOrEmpty(text))
			{
				this.IsThrottled = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("ResponseResource", out text))
			{
				this.ResponseResource = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("ResponseAction", out text))
			{
				this.ResponseAction = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("TargetHealthState", out text) && !string.IsNullOrEmpty(text))
			{
				this.TargetHealthState = (ServiceHealthStatus)Enum.Parse(typeof(ServiceHealthStatus), text);
			}
			if (propertyBag.TryGetValue("TargetHealthStateTransitionId", out text) && !string.IsNullOrEmpty(text))
			{
				this.TargetHealthStateTransitionId = int.Parse(text);
			}
			if (propertyBag.TryGetValue("FirstAlertObservedTime", out text) && !string.IsNullOrEmpty(text))
			{
				this.FirstAlertObservedTime = (string.Equals(text, "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(text).ToUniversalTime()));
			}
			if (propertyBag.TryGetValue("RecoveryResult", out text) && !string.IsNullOrEmpty(text))
			{
				this.RecoveryResult = (ServiceRecoveryResult)Enum.Parse(typeof(ServiceRecoveryResult), text);
			}
			if (propertyBag.TryGetValue("IsRecoveryAttempted", out text) && !string.IsNullOrEmpty(text))
			{
				this.IsRecoveryAttempted = CrimsonHelper.ParseIntStringAsBool(text);
			}
			if (propertyBag.TryGetValue("CorrelationResultsXml", out text))
			{
				this.CorrelationResultsXml = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("CorrelationAction", out text) && !string.IsNullOrEmpty(text))
			{
				this.CorrelationAction = (CorrelatedMonitorAction)Enum.Parse(typeof(CorrelatedMonitorAction), text);
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
				this.ResultId = ResponderResult.idGenerator.NextId();
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			this.AssignResultId();
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			if (ResponderResult.ResultWriter != null)
			{
				ResponderResult.ResultWriter(this);
				return;
			}
			NativeMethods.ResponderResultUnmanaged responderResultUnmanaged = this.ToUnmanaged();
			ResultSeverityLevel severity = CrimsonHelper.ConvertResultTypeToSeverityLevel(this.ResultType);
			NativeMethods.WriteResponderResult(ref responderResultUnmanaged, severity);
			LocalDataAccess.ResponderResultLogger.LogEvent(DateTime.UtcNow, this.ToDictionary());
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
				this.IsThrottled = bool.Parse(array[25]);
			}
			this.ResponseResource = CrimsonHelper.NullDecode(array[26]);
			this.ResponseAction = CrimsonHelper.NullDecode(array[27]);
			if (!string.IsNullOrEmpty(array[28]))
			{
				this.TargetHealthState = (ServiceHealthStatus)Enum.Parse(typeof(ServiceHealthStatus), array[28]);
			}
			if (!string.IsNullOrEmpty(array[29]))
			{
				this.TargetHealthStateTransitionId = int.Parse(array[29]);
			}
			if (!string.IsNullOrEmpty(array[30]))
			{
				this.FirstAlertObservedTime = (string.Equals(array[30], "[null]", StringComparison.OrdinalIgnoreCase) ? null : new DateTime?(DateTime.Parse(array[30]).ToUniversalTime()));
			}
			if (!string.IsNullOrEmpty(array[31]))
			{
				this.RecoveryResult = (ServiceRecoveryResult)Enum.Parse(typeof(ServiceRecoveryResult), array[31]);
			}
			if (!string.IsNullOrEmpty(array[32]))
			{
				this.IsRecoveryAttempted = bool.Parse(array[32]);
			}
			this.CorrelationResultsXml = CrimsonHelper.NullDecode(array[33]);
			if (!string.IsNullOrEmpty(array[34]))
			{
				this.CorrelationAction = (CorrelatedMonitorAction)Enum.Parse(typeof(CorrelatedMonitorAction), array[34]);
			}
			if (!string.IsNullOrEmpty(array[35]))
			{
				this.Version = int.Parse(array[35]);
			}
		}

		internal NativeMethods.ResponderResultUnmanaged ToUnmanaged()
		{
			return new NativeMethods.ResponderResultUnmanaged
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
				IsThrottled = (this.IsThrottled ? 1 : 0),
				ResponseResource = CrimsonHelper.NullCode(this.ResponseResource),
				ResponseAction = CrimsonHelper.NullCode(this.ResponseAction),
				TargetHealthState = this.TargetHealthState,
				TargetHealthStateTransitionId = this.TargetHealthStateTransitionId,
				FirstAlertObservedTime = ((this.FirstAlertObservedTime != null) ? this.FirstAlertObservedTime.Value.ToUniversalTime().ToString("o") : "[null]"),
				RecoveryResult = this.RecoveryResult,
				IsRecoveryAttempted = (this.IsRecoveryAttempted ? 1 : 0),
				CorrelationResultsXml = CrimsonHelper.NullCode(this.CorrelationResultsXml),
				CorrelationAction = this.CorrelationAction,
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
					"IsThrottled",
					this.IsThrottled
				},
				{
					"ResponseResource",
					this.ResponseResource
				},
				{
					"ResponseAction",
					this.ResponseAction
				},
				{
					"TargetHealthState",
					this.TargetHealthState
				},
				{
					"TargetHealthStateTransitionId",
					this.TargetHealthStateTransitionId
				},
				{
					"FirstAlertObservedTime",
					this.FirstAlertObservedTime
				},
				{
					"RecoveryResult",
					this.RecoveryResult
				},
				{
					"IsRecoveryAttempted",
					this.IsRecoveryAttempted
				},
				{
					"CorrelationResultsXml",
					this.CorrelationResultsXml
				},
				{
					"CorrelationAction",
					this.CorrelationAction
				},
				{
					"Version",
					this.Version
				}
			};
		}

		private static int schemaVersion = 65536;

		private static ResponderResultIdGenerator idGenerator = new ResponderResultIdGenerator();
	}
}

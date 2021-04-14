using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public sealed class ProbeResult : WorkItemResult, ICloneable, IPersistence, IWorkItemResultSerialization
	{
		public ProbeResult(WorkDefinition definition) : base(definition)
		{
			this.FailureCategory = -1;
		}

		public ProbeResult()
		{
			this.FailureCategory = -1;
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
		public string StateAttribute11 { get; set; }

		[Column]
		public string StateAttribute12 { get; set; }

		[Column]
		public string StateAttribute13 { get; set; }

		[Column]
		public string StateAttribute14 { get; set; }

		[Column]
		public string StateAttribute15 { get; set; }

		[Column]
		public double StateAttribute16 { get; set; }

		[Column]
		public double StateAttribute17 { get; set; }

		[Column]
		public double StateAttribute18 { get; set; }

		[Column]
		public double StateAttribute19 { get; set; }

		[Column]
		public double StateAttribute20 { get; set; }

		[Column]
		public string StateAttribute21 { get; set; }

		[Column]
		public string StateAttribute22 { get; set; }

		[Column]
		public string StateAttribute23 { get; set; }

		[Column]
		public string StateAttribute24 { get; set; }

		[Column]
		public string StateAttribute25 { get; set; }

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

		[Column(Name = "NotificationParametersXml")]
		public string ExtensionXml { get; set; }

		[Column(Name = "Latency")]
		public double SampleValue { get; set; }

		[Column]
		public string ExecutionContext { get; set; }

		[Column]
		public string FailureContext { get; set; }

		[Column]
		public int FailureCategory { get; set; }

		[Column]
		public string ScopeName { get; set; }

		[Column]
		public string ScopeType { get; set; }

		[Column]
		public string HealthSetName { get; set; }

		[Column]
		public string Data { get; set; }

		[Column]
		internal override int Version { get; set; }

		public override void SetCompleted(ResultType resultType)
		{
			this.StateAttribute11 = base.TruncateStringProperty(this.StateAttribute11, 1024);
			this.StateAttribute12 = base.TruncateStringProperty(this.StateAttribute12, 1024);
			this.StateAttribute13 = base.TruncateStringProperty(this.StateAttribute13, 1024);
			this.StateAttribute14 = base.TruncateStringProperty(this.StateAttribute14, 1024);
			this.StateAttribute15 = base.TruncateStringProperty(this.StateAttribute15, 1024);
			this.StateAttribute21 = base.TruncateStringProperty(this.StateAttribute21, 1024);
			this.StateAttribute22 = base.TruncateStringProperty(this.StateAttribute22, 1024);
			this.StateAttribute23 = base.TruncateStringProperty(this.StateAttribute23, 1024);
			this.StateAttribute24 = base.TruncateStringProperty(this.StateAttribute24, 1024);
			this.StateAttribute25 = base.TruncateStringProperty(this.StateAttribute25, 1024);
			this.ExecutionContext = base.TruncateStringProperty(this.ExecutionContext, 4000);
			this.FailureContext = base.TruncateStringProperty(this.FailureContext, 4000);
			base.SetCompleted(resultType);
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		public LocalDataAccessMetaData LocalDataAccessMetaData { get; private set; }

		internal static int SchemaVersion
		{
			get
			{
				return ProbeResult.schemaVersion;
			}
		}

		internal static Action<ProbeResult> ResultWriter { private get; set; }

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
			if (propertyBag.TryGetValue("StateAttribute11", out text))
			{
				this.StateAttribute11 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute12", out text))
			{
				this.StateAttribute12 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute13", out text))
			{
				this.StateAttribute13 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute14", out text))
			{
				this.StateAttribute14 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute15", out text))
			{
				this.StateAttribute15 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute16", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute16 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute17", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute17 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute18", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute18 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute19", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute19 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute20", out text) && !string.IsNullOrEmpty(text))
			{
				this.StateAttribute20 = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("StateAttribute21", out text))
			{
				this.StateAttribute21 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute22", out text))
			{
				this.StateAttribute22 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute23", out text))
			{
				this.StateAttribute23 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute24", out text))
			{
				this.StateAttribute24 = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("StateAttribute25", out text))
			{
				this.StateAttribute25 = CrimsonHelper.NullDecode(text);
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
			if (propertyBag.TryGetValue("ExtensionXml", out text))
			{
				this.ExtensionXml = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("SampleValue", out text) && !string.IsNullOrEmpty(text))
			{
				this.SampleValue = CrimsonHelper.ParseDouble(text);
			}
			if (propertyBag.TryGetValue("ExecutionContext", out text))
			{
				this.ExecutionContext = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("FailureContext", out text))
			{
				this.FailureContext = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("FailureCategory", out text) && !string.IsNullOrEmpty(text))
			{
				this.FailureCategory = int.Parse(text);
			}
			if (propertyBag.TryGetValue("ScopeName", out text))
			{
				this.ScopeName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("ScopeType", out text))
			{
				this.ScopeType = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("HealthSetName", out text))
			{
				this.HealthSetName = CrimsonHelper.NullDecode(text);
			}
			if (propertyBag.TryGetValue("Data", out text))
			{
				this.Data = CrimsonHelper.NullDecode(text);
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
				this.ResultId = ProbeResult.idGenerator.NextId();
			}
		}

		public void Write(Action<IPersistence> preWriteHandler = null)
		{
			this.AssignResultId();
			if (preWriteHandler != null)
			{
				preWriteHandler(this);
			}
			if (ProbeResult.ResultWriter != null)
			{
				ProbeResult.ResultWriter(this);
				return;
			}
			NativeMethods.ProbeResultUnmanaged probeResultUnmanaged = this.ToUnmanaged();
			ResultSeverityLevel severity = CrimsonHelper.ConvertResultTypeToSeverityLevel(this.ResultType);
			NativeMethods.WriteProbeResult(ref probeResultUnmanaged, severity);
			LocalDataAccess.ProbeResultLogger.LogEvent(DateTime.UtcNow, this.ToDictionary());
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
			this.StateAttribute11 = CrimsonHelper.NullDecode(array[20]);
			this.StateAttribute12 = CrimsonHelper.NullDecode(array[21]);
			this.StateAttribute13 = CrimsonHelper.NullDecode(array[22]);
			this.StateAttribute14 = CrimsonHelper.NullDecode(array[23]);
			this.StateAttribute15 = CrimsonHelper.NullDecode(array[24]);
			if (!string.IsNullOrEmpty(array[25]))
			{
				this.StateAttribute16 = CrimsonHelper.ParseDouble(array[25]);
			}
			if (!string.IsNullOrEmpty(array[26]))
			{
				this.StateAttribute17 = CrimsonHelper.ParseDouble(array[26]);
			}
			if (!string.IsNullOrEmpty(array[27]))
			{
				this.StateAttribute18 = CrimsonHelper.ParseDouble(array[27]);
			}
			if (!string.IsNullOrEmpty(array[28]))
			{
				this.StateAttribute19 = CrimsonHelper.ParseDouble(array[28]);
			}
			if (!string.IsNullOrEmpty(array[29]))
			{
				this.StateAttribute20 = CrimsonHelper.ParseDouble(array[29]);
			}
			this.StateAttribute21 = CrimsonHelper.NullDecode(array[30]);
			this.StateAttribute22 = CrimsonHelper.NullDecode(array[31]);
			this.StateAttribute23 = CrimsonHelper.NullDecode(array[32]);
			this.StateAttribute24 = CrimsonHelper.NullDecode(array[33]);
			this.StateAttribute25 = CrimsonHelper.NullDecode(array[34]);
			if (!string.IsNullOrEmpty(array[35]))
			{
				this.ResultType = (ResultType)Enum.Parse(typeof(ResultType), array[35]);
			}
			if (!string.IsNullOrEmpty(array[36]))
			{
				this.ExecutionId = int.Parse(array[36]);
			}
			if (!string.IsNullOrEmpty(array[37]))
			{
				this.ExecutionStartTime = DateTime.Parse(array[37]).ToUniversalTime();
			}
			if (!string.IsNullOrEmpty(array[38]))
			{
				this.ExecutionEndTime = DateTime.Parse(array[38]).ToUniversalTime();
			}
			if (!string.IsNullOrEmpty(array[39]))
			{
				this.PoisonedCount = byte.Parse(array[39]);
			}
			this.ExtensionXml = CrimsonHelper.NullDecode(array[40]);
			if (!string.IsNullOrEmpty(array[41]))
			{
				this.SampleValue = CrimsonHelper.ParseDouble(array[41]);
			}
			this.ExecutionContext = CrimsonHelper.NullDecode(array[42]);
			this.FailureContext = CrimsonHelper.NullDecode(array[43]);
			if (!string.IsNullOrEmpty(array[44]))
			{
				this.FailureCategory = int.Parse(array[44]);
			}
			this.ScopeName = CrimsonHelper.NullDecode(array[45]);
			this.ScopeType = CrimsonHelper.NullDecode(array[46]);
			this.HealthSetName = CrimsonHelper.NullDecode(array[47]);
			this.Data = CrimsonHelper.NullDecode(array[48]);
			if (!string.IsNullOrEmpty(array[49]))
			{
				this.Version = int.Parse(array[49]);
			}
		}

		internal NativeMethods.ProbeResultUnmanaged ToUnmanaged()
		{
			return new NativeMethods.ProbeResultUnmanaged
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
				StateAttribute11 = CrimsonHelper.NullCode(this.StateAttribute11),
				StateAttribute12 = CrimsonHelper.NullCode(this.StateAttribute12),
				StateAttribute13 = CrimsonHelper.NullCode(this.StateAttribute13),
				StateAttribute14 = CrimsonHelper.NullCode(this.StateAttribute14),
				StateAttribute15 = CrimsonHelper.NullCode(this.StateAttribute15),
				StateAttribute16 = this.StateAttribute16,
				StateAttribute17 = this.StateAttribute17,
				StateAttribute18 = this.StateAttribute18,
				StateAttribute19 = this.StateAttribute19,
				StateAttribute20 = this.StateAttribute20,
				StateAttribute21 = CrimsonHelper.NullCode(this.StateAttribute21),
				StateAttribute22 = CrimsonHelper.NullCode(this.StateAttribute22),
				StateAttribute23 = CrimsonHelper.NullCode(this.StateAttribute23),
				StateAttribute24 = CrimsonHelper.NullCode(this.StateAttribute24),
				StateAttribute25 = CrimsonHelper.NullCode(this.StateAttribute25),
				ResultType = this.ResultType,
				ExecutionId = this.ExecutionId,
				ExecutionStartTime = this.ExecutionStartTime.ToUniversalTime().ToString("o"),
				ExecutionEndTime = this.ExecutionEndTime.ToUniversalTime().ToString("o"),
				PoisonedCount = this.PoisonedCount,
				ExtensionXml = CrimsonHelper.NullCode(this.ExtensionXml),
				SampleValue = this.SampleValue,
				ExecutionContext = CrimsonHelper.NullCode(this.ExecutionContext),
				FailureContext = CrimsonHelper.NullCode(this.FailureContext),
				FailureCategory = this.FailureCategory,
				ScopeName = CrimsonHelper.NullCode(this.ScopeName),
				ScopeType = CrimsonHelper.NullCode(this.ScopeType),
				HealthSetName = CrimsonHelper.NullCode(this.HealthSetName),
				Data = CrimsonHelper.NullCode(this.Data),
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
					"StateAttribute11",
					this.StateAttribute11
				},
				{
					"StateAttribute12",
					this.StateAttribute12
				},
				{
					"StateAttribute13",
					this.StateAttribute13
				},
				{
					"StateAttribute14",
					this.StateAttribute14
				},
				{
					"StateAttribute15",
					this.StateAttribute15
				},
				{
					"StateAttribute16",
					this.StateAttribute16
				},
				{
					"StateAttribute17",
					this.StateAttribute17
				},
				{
					"StateAttribute18",
					this.StateAttribute18
				},
				{
					"StateAttribute19",
					this.StateAttribute19
				},
				{
					"StateAttribute20",
					this.StateAttribute20
				},
				{
					"StateAttribute21",
					this.StateAttribute21
				},
				{
					"StateAttribute22",
					this.StateAttribute22
				},
				{
					"StateAttribute23",
					this.StateAttribute23
				},
				{
					"StateAttribute24",
					this.StateAttribute24
				},
				{
					"StateAttribute25",
					this.StateAttribute25
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
					"ExtensionXml",
					this.ExtensionXml
				},
				{
					"SampleValue",
					this.SampleValue
				},
				{
					"ExecutionContext",
					this.ExecutionContext
				},
				{
					"FailureContext",
					this.FailureContext
				},
				{
					"FailureCategory",
					this.FailureCategory
				},
				{
					"ScopeName",
					this.ScopeName
				},
				{
					"ScopeType",
					this.ScopeType
				},
				{
					"HealthSetName",
					this.HealthSetName
				},
				{
					"Data",
					this.Data
				},
				{
					"Version",
					this.Version
				}
			};
		}

		internal const int ContextColumnsSize = 4000;

		private static int schemaVersion = 65536;

		private static ProbeResultIdGenerator idGenerator = new ProbeResultIdGenerator();
	}
}

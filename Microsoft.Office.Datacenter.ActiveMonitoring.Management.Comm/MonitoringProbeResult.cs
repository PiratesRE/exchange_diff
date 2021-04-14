using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common
{
	[Serializable]
	public class MonitoringProbeResult : ConfigurableObject
	{
		public MonitoringProbeResult() : base(new SimpleProviderPropertyBag())
		{
		}

		internal MonitoringProbeResult(string server, RpcInvokeMonitoringProbe.RpcMonitorProbeResult rpcResult) : this()
		{
			this.Server = server;
			this.MonitorIdentity = rpcResult.MonitorIdentity;
			this.Error = rpcResult.Error;
			this.Exception = rpcResult.Exception;
			this.ExecutionContext = rpcResult.ExecutionContext;
			this.ExecutionEndTime = rpcResult.ExecutionEndTime;
			this.ExecutionId = rpcResult.ExecutionId;
			this.ExecutionStartTime = rpcResult.ExecutionStartTime;
			this.ExtensionXml = rpcResult.ExtensionXml;
			this.FailureContext = rpcResult.FailureContext;
			this.IsNotified = rpcResult.IsNotified;
			this.PoisonedCount = (int)rpcResult.PoisonedCount;
			this.RequestId = rpcResult.RequestId;
			this.ResultId = rpcResult.ResultId;
			this.ResultName = rpcResult.ResultName;
			this.ResultType = rpcResult.ResultType;
			this.RetryCount = (int)rpcResult.RetryCount;
			this.SampleValue = rpcResult.SampleValue;
			this.ServiceName = rpcResult.ServiceName;
			this.StateAttribute1 = rpcResult.StateAttribute1;
			this.StateAttribute2 = rpcResult.StateAttribute2;
			this.StateAttribute3 = rpcResult.StateAttribute3;
			this.StateAttribute4 = rpcResult.StateAttribute4;
			this.StateAttribute5 = rpcResult.StateAttribute5;
			this.StateAttribute6 = rpcResult.StateAttribute6;
			this.StateAttribute7 = rpcResult.StateAttribute7;
			this.StateAttribute8 = rpcResult.StateAttribute8;
			this.StateAttribute9 = rpcResult.StateAttribute9;
			this.StateAttribute10 = rpcResult.StateAttribute10;
			this.StateAttribute11 = rpcResult.StateAttribute11;
			this.StateAttribute12 = rpcResult.StateAttribute12;
			this.StateAttribute13 = rpcResult.StateAttribute13;
			this.StateAttribute14 = rpcResult.StateAttribute14;
			this.StateAttribute15 = rpcResult.StateAttribute15;
			this.StateAttribute16 = rpcResult.StateAttribute16;
			this.StateAttribute17 = rpcResult.StateAttribute17;
			this.StateAttribute18 = rpcResult.StateAttribute18;
			this.StateAttribute19 = rpcResult.StateAttribute19;
			this.StateAttribute20 = rpcResult.StateAttribute20;
			this.StateAttribute21 = rpcResult.StateAttribute21;
			this.StateAttribute22 = rpcResult.StateAttribute22;
			this.StateAttribute23 = rpcResult.StateAttribute23;
			this.StateAttribute24 = rpcResult.StateAttribute24;
			this.StateAttribute25 = rpcResult.StateAttribute25;
			this[SimpleProviderObjectSchema.Identity] = new MonitoringProbeResult.MonitoringProbeResultId(Guid.NewGuid());
		}

		public string Server
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.Server];
			}
			private set
			{
				this[MonitoringProbeResultSchema.Server] = value;
			}
		}

		public string MonitorIdentity
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.MonitorIdentity];
			}
			private set
			{
				this[MonitoringProbeResultSchema.MonitorIdentity] = value;
			}
		}

		public Guid RequestId
		{
			get
			{
				return (Guid)this[MonitoringProbeResultSchema.RequestId];
			}
			private set
			{
				this[MonitoringProbeResultSchema.RequestId] = value;
			}
		}

		public DateTime ExecutionStartTime
		{
			get
			{
				return (DateTime)this[MonitoringProbeResultSchema.ExecutionStartTime];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ExecutionStartTime] = value;
			}
		}

		public DateTime ExecutionEndTime
		{
			get
			{
				return (DateTime)this[MonitoringProbeResultSchema.ExecutionEndTime];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ExecutionEndTime] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.Error];
			}
			private set
			{
				this[MonitoringProbeResultSchema.Error] = value;
			}
		}

		public string Exception
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.Exception];
			}
			private set
			{
				this[MonitoringProbeResultSchema.Exception] = value;
			}
		}

		public int PoisonedCount
		{
			get
			{
				return (int)this[MonitoringProbeResultSchema.PoisonedCount];
			}
			private set
			{
				this[MonitoringProbeResultSchema.PoisonedCount] = value;
			}
		}

		public int ExecutionId
		{
			get
			{
				return (int)this[MonitoringProbeResultSchema.ExecutionId];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ExecutionId] = value;
			}
		}

		public double SampleValue
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.SampleValue];
			}
			private set
			{
				this[MonitoringProbeResultSchema.SampleValue] = value;
			}
		}

		public string ExecutionContext
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.ExecutionContext];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ExecutionContext] = value;
			}
		}

		public string FailureContext
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.FailureContext];
			}
			private set
			{
				this[MonitoringProbeResultSchema.FailureContext] = value;
			}
		}

		public string ExtensionXml
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.ExtensionXml];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ExtensionXml] = value;
			}
		}

		public ResultType ResultType
		{
			get
			{
				return (ResultType)this[MonitoringProbeResultSchema.ResultType];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ResultType] = value;
			}
		}

		public int RetryCount
		{
			get
			{
				return (int)this[MonitoringProbeResultSchema.RetryCount];
			}
			private set
			{
				this[MonitoringProbeResultSchema.RetryCount] = value;
			}
		}

		public string ResultName
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.ResultName];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ResultName] = value;
			}
		}

		public bool IsNotified
		{
			get
			{
				return (bool)this[MonitoringProbeResultSchema.IsNotified];
			}
			private set
			{
				this[MonitoringProbeResultSchema.IsNotified] = value;
			}
		}

		public int ResultId
		{
			get
			{
				return (int)this[MonitoringProbeResultSchema.ResultId];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ResultId] = value;
			}
		}

		public string ServiceName
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.ServiceName];
			}
			private set
			{
				this[MonitoringProbeResultSchema.ServiceName] = value;
			}
		}

		public string StateAttribute1
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute1];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute1] = value;
			}
		}

		public string StateAttribute2
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute2];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute2] = value;
			}
		}

		public string StateAttribute3
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute3];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute3] = value;
			}
		}

		public string StateAttribute4
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute4];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute4] = value;
			}
		}

		public string StateAttribute5
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute5];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute5] = value;
			}
		}

		public double StateAttribute6
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute6];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute6] = value;
			}
		}

		public double StateAttribute7
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute7];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute7] = value;
			}
		}

		public double StateAttribute8
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute8];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute8] = value;
			}
		}

		public double StateAttribute9
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute9];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute9] = value;
			}
		}

		public double StateAttribute10
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute10];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute10] = value;
			}
		}

		public string StateAttribute11
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute11];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute11] = value;
			}
		}

		public string StateAttribute12
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute12];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute12] = value;
			}
		}

		public string StateAttribute13
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute13];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute13] = value;
			}
		}

		public string StateAttribute14
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute14];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute14] = value;
			}
		}

		public string StateAttribute15
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute15];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute15] = value;
			}
		}

		public double StateAttribute16
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute16];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute16] = value;
			}
		}

		public double StateAttribute17
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute17];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute17] = value;
			}
		}

		public double StateAttribute18
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute18];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute18] = value;
			}
		}

		public double StateAttribute19
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute19];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute19] = value;
			}
		}

		public double StateAttribute20
		{
			get
			{
				return (double)this[MonitoringProbeResultSchema.StateAttribute20];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute20] = value;
			}
		}

		public string StateAttribute21
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute21];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute21] = value;
			}
		}

		public string StateAttribute22
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute22];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute22] = value;
			}
		}

		public string StateAttribute23
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute23];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute23] = value;
			}
		}

		public string StateAttribute24
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute24];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute24] = value;
			}
		}

		public string StateAttribute25
		{
			get
			{
				return (string)this[MonitoringProbeResultSchema.StateAttribute25];
			}
			private set
			{
				this[MonitoringProbeResultSchema.StateAttribute25] = value;
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
				return MonitoringProbeResult.schema;
			}
		}

		private static MonitoringProbeResultSchema schema = ObjectSchema.GetInstance<MonitoringProbeResultSchema>();

		[Serializable]
		public class MonitoringProbeResultId : ObjectId
		{
			public MonitoringProbeResultId(Guid requestId)
			{
				this.identity = requestId.ToString("N");
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

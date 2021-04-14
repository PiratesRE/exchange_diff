using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class TopologyServiceOutcome : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TopologyServiceOutcome.schema;
			}
		}

		public string LatencyInMillisecondsString
		{
			get
			{
				return Math.Round(this.Latency.TotalMilliseconds, 2).ToString("F2", CultureInfo.InvariantCulture);
			}
		}

		public string Server
		{
			get
			{
				return (string)this[TopologyServiceOutcomeSchema.Server];
			}
			internal set
			{
				this[TopologyServiceOutcomeSchema.Server] = value;
			}
		}

		public string OperationType
		{
			get
			{
				return (string)this[TopologyServiceOutcomeSchema.OperationType];
			}
			internal set
			{
				this[TopologyServiceOutcomeSchema.OperationType] = value;
			}
		}

		public TopologyServiceResult Result
		{
			get
			{
				return (TopologyServiceResult)this[TopologyServiceOutcomeSchema.Result];
			}
			internal set
			{
				this[TopologyServiceOutcomeSchema.Result] = value;
			}
		}

		public TimeSpan Latency
		{
			get
			{
				return (TimeSpan)(this[TopologyServiceOutcomeSchema.Latency] ?? TimeSpan.Zero);
			}
			internal set
			{
				this[TopologyServiceOutcomeSchema.Latency] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this.propertyBag[TopologyServiceOutcomeSchema.Error];
			}
			internal set
			{
				this.propertyBag[TopologyServiceOutcomeSchema.Error] = value;
			}
		}

		public string Output
		{
			get
			{
				return (string)this.propertyBag[TopologyServiceOutcomeSchema.Output];
			}
			internal set
			{
				this.propertyBag[TopologyServiceOutcomeSchema.Output] = value;
			}
		}

		public TopologyServiceOutcome(string server, string operationType) : base(new SimpleProviderPropertyBag())
		{
			this.Server = server;
			this.OperationType = operationType;
			this.Result = new TopologyServiceResult(TopologyServiceResultEnum.Undefined);
		}

		internal void Update(TopologyServiceResultEnum resultEnum, TimeSpan latency, string error, string output)
		{
			lock (this.thisLock)
			{
				this.Result = new TopologyServiceResult(resultEnum);
				this.Latency = latency;
				this.Error = (error ?? string.Empty);
				this.Output = (output ?? string.Empty);
			}
		}

		[NonSerialized]
		private object thisLock = new object();

		private static TopologyServiceOutcomeSchema schema = ObjectSchema.GetInstance<TopologyServiceOutcomeSchema>();
	}
}

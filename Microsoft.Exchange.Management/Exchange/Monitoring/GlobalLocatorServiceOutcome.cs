using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class GlobalLocatorServiceOutcome : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return GlobalLocatorServiceOutcome.schema;
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
				return (string)this[GlobalLocatorServiceOutcomeSchema.Server];
			}
			internal set
			{
				this[GlobalLocatorServiceOutcomeSchema.Server] = value;
			}
		}

		public GlobalLocatorServiceResult Result
		{
			get
			{
				return (GlobalLocatorServiceResult)this[GlobalLocatorServiceOutcomeSchema.Result];
			}
			internal set
			{
				this[GlobalLocatorServiceOutcomeSchema.Result] = value;
			}
		}

		public TimeSpan Latency
		{
			get
			{
				return (TimeSpan)(this[GlobalLocatorServiceOutcomeSchema.Latency] ?? TimeSpan.Zero);
			}
			internal set
			{
				this[GlobalLocatorServiceOutcomeSchema.Latency] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this.propertyBag[GlobalLocatorServiceOutcomeSchema.Error];
			}
			internal set
			{
				this.propertyBag[GlobalLocatorServiceOutcomeSchema.Error] = value;
			}
		}

		public string Output
		{
			get
			{
				return (string)this.propertyBag[GlobalLocatorServiceOutcomeSchema.Output];
			}
			internal set
			{
				this.propertyBag[GlobalLocatorServiceOutcomeSchema.Output] = value;
			}
		}

		public GlobalLocatorServiceOutcome(string server) : base(new SimpleProviderPropertyBag())
		{
			this.Server = server;
			this.Result = new GlobalLocatorServiceResult(GlobalLocatorServiceResultEnum.Undefined);
		}

		internal void Update(GlobalLocatorServiceResultEnum resultEnum, TimeSpan latency, string error, string output)
		{
			lock (this.thisLock)
			{
				this.Result = new GlobalLocatorServiceResult(resultEnum);
				this.Latency = latency;
				this.Error = (error ?? string.Empty);
				this.Output = (output ?? string.Empty);
			}
		}

		[NonSerialized]
		private object thisLock = new object();

		private static GlobalLocatorServiceOutcomeSchema schema = ObjectSchema.GetInstance<GlobalLocatorServiceOutcomeSchema>();
	}
}

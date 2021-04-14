using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class MonitoringServiceBasicCmdletOutcome : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MonitoringServiceBasicCmdletOutcome.schema;
			}
		}

		public string Server
		{
			get
			{
				return (string)this[MonitoringServiceBasicCmdletOutcomeSchema.Server];
			}
			internal set
			{
				this[MonitoringServiceBasicCmdletOutcomeSchema.Server] = value;
			}
		}

		public MonitoringServiceBasicCmdletResult Result
		{
			get
			{
				return (MonitoringServiceBasicCmdletResult)this[MonitoringServiceBasicCmdletOutcomeSchema.Result];
			}
			internal set
			{
				this[MonitoringServiceBasicCmdletOutcomeSchema.Result] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this.propertyBag[MonitoringServiceBasicCmdletOutcomeSchema.Error];
			}
			internal set
			{
				this.propertyBag[MonitoringServiceBasicCmdletOutcomeSchema.Error] = value;
			}
		}

		public MonitoringServiceBasicCmdletOutcome(string server) : base(new SimpleProviderPropertyBag())
		{
			this.Server = server;
			this.Result = new MonitoringServiceBasicCmdletResult(MonitoringServiceBasicCmdletResultEnum.Undefined);
		}

		internal void Update(MonitoringServiceBasicCmdletResultEnum resultEnum, string error)
		{
			lock (this.thisLock)
			{
				this.Result = new MonitoringServiceBasicCmdletResult(resultEnum);
				this.Error = (error ?? string.Empty);
			}
		}

		[NonSerialized]
		private object thisLock = new object();

		private static MonitoringServiceBasicCmdletOutcomeSchema schema = ObjectSchema.GetInstance<MonitoringServiceBasicCmdletOutcomeSchema>();
	}
}

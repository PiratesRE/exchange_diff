using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class LiveIdAuthenticationOutcome : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return LiveIdAuthenticationOutcome.schema;
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
				return (string)this[LiveIdAuthenticationOutcomeSchema.Server];
			}
			internal set
			{
				this[LiveIdAuthenticationOutcomeSchema.Server] = value;
			}
		}

		public string Mailbox
		{
			get
			{
				return (string)this[LiveIdAuthenticationOutcomeSchema.Mailbox];
			}
			internal set
			{
				this[LiveIdAuthenticationOutcomeSchema.Mailbox] = value;
			}
		}

		public LiveIdAuthenticationResult Result
		{
			get
			{
				return (LiveIdAuthenticationResult)this[LiveIdAuthenticationOutcomeSchema.Result];
			}
			internal set
			{
				this[LiveIdAuthenticationOutcomeSchema.Result] = value;
			}
		}

		public TimeSpan Latency
		{
			get
			{
				return (TimeSpan)(this[LiveIdAuthenticationOutcomeSchema.Latency] ?? TimeSpan.Zero);
			}
			internal set
			{
				this[LiveIdAuthenticationOutcomeSchema.Latency] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this.propertyBag[LiveIdAuthenticationOutcomeSchema.Error];
			}
			internal set
			{
				this.propertyBag[LiveIdAuthenticationOutcomeSchema.Error] = value;
			}
		}

		public LiveIdAuthenticationOutcome(string server, string username) : base(new SimpleProviderPropertyBag())
		{
			this.Server = server;
			this.Mailbox = username;
			this.Result = new LiveIdAuthenticationResult(LiveIdAuthenticationResultEnum.Undefined);
		}

		internal void Update(LiveIdAuthenticationResultEnum resultEnum, TimeSpan latency, string error)
		{
			lock (this.thisLock)
			{
				this.Result = new LiveIdAuthenticationResult(resultEnum);
				this.Latency = latency;
				this.Error = (error ?? string.Empty);
			}
		}

		[NonSerialized]
		private object thisLock = new object();

		private static LiveIdAuthenticationOutcomeSchema schema = ObjectSchema.GetInstance<LiveIdAuthenticationOutcomeSchema>();
	}
}

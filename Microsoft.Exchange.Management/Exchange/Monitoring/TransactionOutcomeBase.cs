using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public abstract class TransactionOutcomeBase : ConfigurableObject
	{
		internal TransactionOutcomeBase(string clientAccessServer, string scenarioName, string scenarioDescription, string performanceCounterName, string userName) : base(new CasTransactionPropertyBag())
		{
			this.ClientAccessServer = clientAccessServer;
			this.Scenario = scenarioName;
			this.ScenarioDescription = scenarioDescription;
			this.PerformanceCounterName = performanceCounterName;
			this.Result = new CasTransactionResult(CasTransactionResultEnum.Undefined);
			this.Error = null;
			this.UserName = userName;
			this.StartTime = ExDateTime.Now;
		}

		internal virtual void Update(CasTransactionResultEnum result)
		{
			this.Update(result, null);
		}

		internal virtual void Update(CasTransactionResultEnum result, string additionalInformation)
		{
			this.Update(result, (result == CasTransactionResultEnum.Success) ? TransactionOutcomeBase.ComputeLatency(this.StartTime) : TimeSpan.FromMilliseconds(-1.0), additionalInformation);
		}

		internal virtual void Update(CasTransactionResultEnum result, string additionalInformation, EventTypeEnumeration eventType)
		{
			this.Update(result, (result == CasTransactionResultEnum.Success) ? TransactionOutcomeBase.ComputeLatency(this.StartTime) : TimeSpan.FromMilliseconds(-1.0), additionalInformation, eventType);
		}

		internal virtual void Update(CasTransactionResultEnum result, TimeSpan latency, string additionalInformation)
		{
			EventTypeEnumeration eventType;
			if (result == CasTransactionResultEnum.Failure)
			{
				eventType = EventTypeEnumeration.Error;
			}
			else if (result == CasTransactionResultEnum.Skipped)
			{
				eventType = EventTypeEnumeration.Warning;
			}
			else
			{
				if (result != CasTransactionResultEnum.Success)
				{
					throw new ArgumentException("Unhandled CasTransactionResultEnum type.");
				}
				eventType = EventTypeEnumeration.Success;
			}
			this.Update(result, latency, additionalInformation, eventType);
		}

		internal virtual void Update(CasTransactionResultEnum result, TimeSpan latency, string additionalInformation, EventTypeEnumeration eventType)
		{
			this.Result = new CasTransactionResult(result);
			this.Latency = latency;
			this.Error = additionalInformation;
			this.EventType = eventType;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal void UpdateScenario(string scenarioName, string scenarioDescription)
		{
			this.Scenario = scenarioName;
			this.ScenarioDescription = scenarioDescription;
		}

		internal void UpdateLatency(TimeSpan latency)
		{
			this.Latency = latency;
		}

		private static TimeSpan ComputeLatency(ExDateTime startTime)
		{
			ExDateTime now = ExDateTime.Now;
			TimeSpan result = now - startTime;
			if (result.Ticks == 0L)
			{
				return new TimeSpan(1L);
			}
			return result;
		}

		public string ClientAccessServer
		{
			get
			{
				return (string)this.propertyBag[TransactionOutcomeBaseSchema.ClientAccessServer];
			}
			protected set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.ClientAccessServer] = value;
			}
		}

		public string Scenario
		{
			get
			{
				return (string)this.propertyBag[TransactionOutcomeBaseSchema.ScenarioName];
			}
			private set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.ScenarioName] = value;
			}
		}

		public string ScenarioDescription
		{
			get
			{
				return (string)this.propertyBag[TransactionOutcomeBaseSchema.ScenarioDescription];
			}
			private set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.ScenarioDescription] = value;
			}
		}

		public string PerformanceCounterName
		{
			get
			{
				return (string)this.propertyBag[TransactionOutcomeBaseSchema.PerformanceCounterName];
			}
			internal set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.PerformanceCounterName] = value;
			}
		}

		public CasTransactionResult Result
		{
			get
			{
				return (CasTransactionResult)this.propertyBag[TransactionOutcomeBaseSchema.Result];
			}
			private set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.Result] = value;
			}
		}

		public string Error
		{
			get
			{
				return (string)this.propertyBag[TransactionOutcomeBaseSchema.AdditionalInformation];
			}
			private set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.AdditionalInformation] = value;
			}
		}

		public string UserName
		{
			get
			{
				return (string)this.propertyBag[TransactionOutcomeBaseSchema.UserName];
			}
			private set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.UserName] = value;
			}
		}

		public ExDateTime StartTime
		{
			get
			{
				return (ExDateTime)this.propertyBag[TransactionOutcomeBaseSchema.StartTime];
			}
			internal set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.StartTime] = value;
			}
		}

		public EnhancedTimeSpan Latency
		{
			get
			{
				return (EnhancedTimeSpan)this.propertyBag[TransactionOutcomeBaseSchema.Latency];
			}
			private set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.Latency] = value;
			}
		}

		public EventTypeEnumeration EventType
		{
			get
			{
				return (EventTypeEnumeration)this.propertyBag[TransactionOutcomeBaseSchema.EventType];
			}
			private set
			{
				this.propertyBag[TransactionOutcomeBaseSchema.EventType] = value;
			}
		}

		public string LatencyInMillisecondsString
		{
			get
			{
				if (string.IsNullOrEmpty(this.Error))
				{
					return Math.Round(this.Latency.TotalMilliseconds, 2).ToString("F2", CultureInfo.InvariantCulture);
				}
				return string.Empty;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpAvailabilityPerfCountersWrapper : IDisposable, ISmtpAvailabilityPerfCounters
	{
		public SmtpAvailabilityPerfCountersWrapper(ProcessTransportRole transportRole, string instanceName, int smtpAvailabilityMinConnectionsToMonitor)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("instanceName", instanceName);
			this.connectorInstance = instanceName;
			this.smtpAvailabilityMinConnectionsToMonitor = smtpAvailabilityMinConnectionsToMonitor;
			this.lastEventReportTime = DateTime.MinValue;
			this.inLowAvailabilityRedState = false;
			this.availabilityPercentage = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.failuresDueToMaxInboundConnectionLimit = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.failuresDueToWLIDDown = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.failuresDueToADDown = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.failuresDueToBackPressure = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.failuresDueToIOExceptions = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.failuresDueToTLSErrors = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.failuresDueToMaxLocalLoopCount = new SlidingTotalCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0), () => DateTime.UtcNow);
			this.totalConnections = new SlidingPercentageCounter(SmtpAvailabilityPerfCountersWrapper.SlidingCounterInterval, SmtpAvailabilityPerfCountersWrapper.SlidingCounterPrecision);
			this.loopingMessagesLastHour = new SlidingTotalCounter(TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(1.0), () => DateTime.UtcNow);
			this.refreshTimer = new GuardedTimer(delegate(object state)
			{
				this.RefreshMessageLoopsInLastHourCounter();
			}, null, TimeSpan.FromMinutes(5.0));
			try
			{
				SmtpAvailabilityPerfCounters.SetCategoryName(SmtpAvailabilityPerfCountersWrapper.perfCounterCategoryMap[transportRole]);
				this.perfCountersInstance = SmtpAvailabilityPerfCounters.GetInstance(instanceName);
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<string, InvalidOperationException>((long)this.GetHashCode(), "Failed to initialize performance counters instance '{0}': {1}", instanceName, ex);
				SmtpAvailabilityPerfCountersWrapper.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveAvailabilityCounterFailure, null, new object[]
				{
					instanceName,
					ex
				});
				this.perfCountersInstance = null;
			}
		}

		protected SmtpAvailabilityPerfCountersWrapper()
		{
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (this.refreshTimer != null)
				{
					this.refreshTimer.Change(-1, -1);
					this.refreshTimer.Dispose(true);
				}
				this.disposed = true;
			}
		}

		public virtual void UpdatePerformanceCounters(LegitimateSmtpAvailabilityCategory category)
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			lock (this.syncObject)
			{
				this.totalConnections.AddNumerator(1L);
				this.availabilityPercentage.AddDenominator(1L);
				this.failuresDueToMaxInboundConnectionLimit.AddDenominator(1L);
				this.failuresDueToWLIDDown.AddDenominator(1L);
				this.failuresDueToADDown.AddDenominator(1L);
				this.failuresDueToBackPressure.AddDenominator(1L);
				this.failuresDueToIOExceptions.AddDenominator(1L);
				this.failuresDueToTLSErrors.AddDenominator(1L);
				switch (category)
				{
				case LegitimateSmtpAvailabilityCategory.SuccessfulSubmission:
					this.availabilityPercentage.AddNumerator(1L);
					break;
				case LegitimateSmtpAvailabilityCategory.RejectDueToMaxInboundConnectionLimit:
					this.failuresDueToMaxInboundConnectionLimit.AddNumerator(1L);
					break;
				case LegitimateSmtpAvailabilityCategory.RejectDueToWLIDDown:
					this.failuresDueToWLIDDown.AddNumerator(1L);
					break;
				case LegitimateSmtpAvailabilityCategory.RejectDueToADDown:
					this.failuresDueToADDown.AddNumerator(1L);
					break;
				case LegitimateSmtpAvailabilityCategory.RejectDueToBackPressure:
					this.failuresDueToBackPressure.AddNumerator(1L);
					break;
				case LegitimateSmtpAvailabilityCategory.RejectDueToIOException:
					this.failuresDueToIOExceptions.AddNumerator(1L);
					break;
				case LegitimateSmtpAvailabilityCategory.RejectDueToTLSError:
					this.failuresDueToTLSErrors.AddNumerator(1L);
					break;
				case LegitimateSmtpAvailabilityCategory.RejectDueToMaxLocalLoopCount:
					this.failuresDueToMaxLocalLoopCount.AddValue(1L);
					break;
				}
				this.perfCountersInstance.TotalConnections.RawValue = this.totalConnections.Numerator;
				this.perfCountersInstance.AvailabilityPercentage.RawValue = (long)this.availabilityPercentage.GetSlidingPercentage();
				if (this.perfCountersInstance.TotalConnections.RawValue > (long)this.smtpAvailabilityMinConnectionsToMonitor)
				{
					this.perfCountersInstance.ActivityPercentage.RawValue = (long)this.availabilityPercentage.GetSlidingPercentage();
				}
				else
				{
					this.perfCountersInstance.ActivityPercentage.RawValue = 100L;
				}
				if (this.perfCountersInstance.ActivityPercentage.RawValue < 99L)
				{
					if (DateTime.UtcNow - this.lastEventReportTime > SmtpAvailabilityPerfCountersWrapper.MinReportInterval)
					{
						this.lastEventReportTime = DateTime.UtcNow;
						this.inLowAvailabilityRedState = true;
						SmtpAvailabilityPerfCountersWrapper.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveConnectorAvailabilityLow, null, new object[]
						{
							this.connectorInstance,
							this.perfCountersInstance.ActivityPercentage.RawValue
						});
						string notificationReason = string.Format("The SMTP availability of receive connector {0} was low ({1} percent) in the last 15 minutes.", this.connectorInstance, this.perfCountersInstance.ActivityPercentage.RawValue);
						EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpReceiveConnectorAvailabilityLow", null, notificationReason, ResultSeverityLevel.Error, false);
					}
				}
				else if (this.perfCountersInstance.ActivityPercentage.RawValue == 100L && this.inLowAvailabilityRedState)
				{
					this.inLowAvailabilityRedState = false;
					SmtpAvailabilityPerfCountersWrapper.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveConnectorAvailabilityNormal, null, new object[]
					{
						this.connectorInstance,
						this.perfCountersInstance.ActivityPercentage.RawValue
					});
					string notificationReason2 = string.Format("The SMTP availability of receive connector {0} was normal ({1} percent) in the last 15 minutes.", this.connectorInstance, this.perfCountersInstance.ActivityPercentage.RawValue);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "SmtpReceiveConnectorAvailabilityLow", null, notificationReason2, ResultSeverityLevel.Informational, false);
				}
				this.perfCountersInstance.FailuresDueToMaxInboundConnectionLimit.RawValue = (long)this.failuresDueToMaxInboundConnectionLimit.GetSlidingPercentage();
				this.perfCountersInstance.FailuresDueToWLIDDown.RawValue = (long)this.failuresDueToWLIDDown.GetSlidingPercentage();
				this.perfCountersInstance.FailuresDueToADDown.RawValue = (long)this.failuresDueToADDown.GetSlidingPercentage();
				this.perfCountersInstance.FailuresDueToBackPressure.RawValue = (long)this.failuresDueToBackPressure.GetSlidingPercentage();
				this.perfCountersInstance.FailuresDueToIOExceptions.RawValue = (long)this.failuresDueToIOExceptions.GetSlidingPercentage();
				this.perfCountersInstance.FailuresDueToTLSErrors.RawValue = (long)this.failuresDueToTLSErrors.GetSlidingPercentage();
				this.perfCountersInstance.FailuresDueToMaxLocalLoopCount.RawValue = this.failuresDueToMaxLocalLoopCount.Sum;
			}
		}

		public void IncrementMessageLoopsInLastHourCounter(long incrementValue)
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			lock (this.syncLoopingMessagesObject)
			{
				this.loopingMessagesLastHour.AddValue(incrementValue);
				this.perfCountersInstance.LoopingMessagesLastHour.RawValue = this.loopingMessagesLastHour.Sum;
			}
		}

		private void RefreshMessageLoopsInLastHourCounter()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			lock (this.syncLoopingMessagesObject)
			{
				this.perfCountersInstance.LoopingMessagesLastHour.RawValue = this.loopingMessagesLastHour.Sum;
			}
		}

		private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
		{
			{
				ProcessTransportRole.Edge,
				"MSExchangeTransport SmtpAvailability"
			},
			{
				ProcessTransportRole.Hub,
				"MSExchangeTransport SmtpAvailability"
			},
			{
				ProcessTransportRole.FrontEnd,
				"MSExchangeFrontEndTransport SmtpAvailability"
			},
			{
				ProcessTransportRole.MailboxDelivery,
				"MSExchange Delivery SmtpAvailability"
			}
		};

		private static readonly TimeSpan SlidingCounterInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan SlidingCounterPrecision = TimeSpan.FromSeconds(1.0);

		private static readonly TimeSpan MinReportInterval = TimeSpan.FromMinutes(15.0);

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.SmtpReceiveTracer.Category, TransportEventLog.GetEventSource());

		private readonly int smtpAvailabilityMinConnectionsToMonitor;

		private SlidingPercentageCounter availabilityPercentage;

		private SlidingPercentageCounter failuresDueToMaxInboundConnectionLimit;

		private SlidingPercentageCounter failuresDueToWLIDDown;

		private SlidingPercentageCounter failuresDueToADDown;

		private SlidingPercentageCounter failuresDueToBackPressure;

		private SlidingPercentageCounter failuresDueToIOExceptions;

		private SlidingPercentageCounter failuresDueToTLSErrors;

		private SlidingTotalCounter failuresDueToMaxLocalLoopCount;

		private SlidingPercentageCounter totalConnections;

		private SmtpAvailabilityPerfCountersInstance perfCountersInstance;

		private string connectorInstance;

		private DateTime lastEventReportTime;

		private bool inLowAvailabilityRedState;

		private object syncObject = new object();

		private SlidingTotalCounter loopingMessagesLastHour;

		private object syncLoopingMessagesObject = new object();

		private GuardedTimer refreshTimer;

		private bool disposed;
	}
}

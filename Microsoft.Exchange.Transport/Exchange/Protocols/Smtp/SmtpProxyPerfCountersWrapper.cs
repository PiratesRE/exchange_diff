using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpProxyPerfCountersWrapper
	{
		public SmtpProxyPerfCountersWrapper(string instanceName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("instanceName", instanceName);
			this.connectorInstance = instanceName;
			this.proxySetupFailurePercentage = new SlidingPercentageCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.proxyAttempts = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.successfulProxyAttempts = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.userLookupFailures = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.backEndLocatorFailures = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.dnsLookupFailures = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.connectionFailures = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.protocolErrors = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			this.socketErrors = new SlidingTotalCounter(SmtpProxyPerfCountersWrapper.SlidingCounterInterval, SmtpProxyPerfCountersWrapper.SlidingCounterPrecision);
			try
			{
				this.perfCountersInstance = SmtpProxyPerfCounters.GetInstance(instanceName);
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<string, InvalidOperationException>((long)this.GetHashCode(), "Failed to initialize performance counters instance '{0}': {1}", instanceName, ex);
				SmtpProxyPerfCountersWrapper.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveProxyCounterFailure, null, new object[]
				{
					instanceName,
					ex
				});
				this.perfCountersInstance = null;
			}
		}

		public void UpdateOnProxyStepFailure(SessionSetupFailureReason failureReason)
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			lock (this.syncObject)
			{
				switch (failureReason)
				{
				case SessionSetupFailureReason.None:
				case SessionSetupFailureReason.Shutdown:
					break;
				case SessionSetupFailureReason.UserLookupFailure:
					this.userLookupFailures.AddValue(1L);
					break;
				case SessionSetupFailureReason.DnsLookupFailure:
					this.dnsLookupFailures.AddValue(1L);
					break;
				case SessionSetupFailureReason.ConnectionFailure:
					this.connectionFailures.AddValue(1L);
					break;
				case SessionSetupFailureReason.ProtocolError:
					this.protocolErrors.AddValue(1L);
					break;
				case SessionSetupFailureReason.SocketError:
					this.socketErrors.AddValue(1L);
					break;
				case SessionSetupFailureReason.BackEndLocatorFailure:
					this.backEndLocatorFailures.AddValue(1L);
					break;
				default:
					throw new InvalidOperationException("Invalid proxy failure reason");
				}
				this.UpdateAllCounters();
			}
		}

		public void UpdateOnProxyFailure()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			lock (this.syncObject)
			{
				this.proxyAttempts.AddValue(1L);
				this.proxySetupFailurePercentage.AddDenominator(1L);
				this.proxySetupFailurePercentage.AddNumerator(1L);
			}
			if (this.proxySetupFailurePercentage.GetSlidingPercentage() >= 15.0 && this.proxyAttempts.Sum >= 100L)
			{
				SmtpProxyPerfCountersWrapper.eventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveTooManyProxySessionFailures, this.connectorInstance, new object[0]);
			}
			this.UpdateAllCounters();
		}

		public void UpdateOnProxySuccess()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			lock (this.syncObject)
			{
				this.proxyAttempts.AddValue(1L);
				this.proxySetupFailurePercentage.AddDenominator(1L);
				this.successfulProxyAttempts.AddValue(1L);
				this.UpdateAllCounters();
			}
		}

		public void UpdateBytesProxied(int bytesProxied)
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			this.perfCountersInstance.TotalBytesProxied.IncrementBy((long)bytesProxied);
		}

		public void IncrementOutboundConnectionsCurrent()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			this.perfCountersInstance.OutboundConnectionsCurrent.Increment();
		}

		public void DecrementOutboundConnectionsCurrent()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			this.perfCountersInstance.OutboundConnectionsCurrent.Decrement();
		}

		public void DecrementInboundConnectionsCurrent()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			this.perfCountersInstance.InboundConnectionsCurrent.Decrement();
		}

		public void IncrementInboundConnectionsCurrent()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			this.perfCountersInstance.InboundConnectionsCurrent.Increment();
		}

		public void IncrementOutboundConnectionsTotal()
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			this.perfCountersInstance.OutboundConnectionsTotal.Increment();
		}

		public void IncrementMessagesProxiedTotalBy(int count)
		{
			if (this.perfCountersInstance == null)
			{
				return;
			}
			this.perfCountersInstance.MessagesProxiedTotal.IncrementBy((long)count);
		}

		private void UpdateAllCounters()
		{
			this.perfCountersInstance.TotalProxyAttempts.RawValue = this.proxyAttempts.Sum;
			this.perfCountersInstance.PercentageProxySetupFailures.RawValue = (long)this.proxySetupFailurePercentage.GetSlidingPercentage();
			this.perfCountersInstance.TotalProxyUserLookupFailures.RawValue = this.userLookupFailures.Sum;
			this.perfCountersInstance.TotalProxyBackEndLocatorFailures.RawValue = this.backEndLocatorFailures.Sum;
			this.perfCountersInstance.TotalProxyDnsLookupFailures.RawValue = this.dnsLookupFailures.Sum;
			this.perfCountersInstance.TotalProxyConnectionFailures.RawValue = this.connectionFailures.Sum;
			this.perfCountersInstance.TotalProxyProtocolErrors.RawValue = this.protocolErrors.Sum;
			this.perfCountersInstance.TotalProxySocketErrors.RawValue = this.socketErrors.Sum;
			this.perfCountersInstance.TotalSuccessfulProxySessions.RawValue = this.successfulProxyAttempts.Sum;
		}

		private static readonly TimeSpan SlidingCounterInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan SlidingCounterPrecision = TimeSpan.FromSeconds(1.0);

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.SmtpReceiveTracer.Category, TransportEventLog.GetEventSource());

		private readonly string connectorInstance;

		private SlidingPercentageCounter proxySetupFailurePercentage;

		private SlidingTotalCounter proxyAttempts;

		private SlidingTotalCounter successfulProxyAttempts;

		private SlidingTotalCounter userLookupFailures;

		private SlidingTotalCounter backEndLocatorFailures;

		private SlidingTotalCounter dnsLookupFailures;

		private SlidingTotalCounter connectionFailures;

		private SlidingTotalCounter protocolErrors;

		private SlidingTotalCounter socketErrors;

		private SmtpProxyPerfCountersInstance perfCountersInstance;

		private object syncObject = new object();
	}
}

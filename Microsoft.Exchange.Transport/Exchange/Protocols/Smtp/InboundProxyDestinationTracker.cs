using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class InboundProxyDestinationTracker : DisposeTrackableBase, IInboundProxyDestinationTracker
	{
		public InboundProxyDestinationTracker(InboundProxyTrackerType trackerType, bool trackingEnabled, bool rejectBasedOnInboundProxyDestinationTrackingEnabled, int perDestinationConnectionPercentageThreshold, InboundProxyDestinationTracker.TryGetDestinationConnectionThresholdDelegate tryGetDestinationConnectionThresholdDelegate, ConnectionsTracker.GetExPerfCounterDelegate getConnectionsCurrent, ConnectionsTracker.GetExPerfCounterDelegate getConnectionsTotal, IExEventLog eventLog, TimeSpan trackerLogInterval, IEnumerable<ReceiveConnector> receiveConnectors)
		{
			ArgumentValidator.ThrowIfNull("trackerType", trackerType);
			ArgumentValidator.ThrowIfNull("tryGetDestinationConnectionThresholdDelegate", tryGetDestinationConnectionThresholdDelegate);
			ArgumentValidator.ThrowIfNull("getConnectionsCurrent", getConnectionsCurrent);
			ArgumentValidator.ThrowIfNull("eventLog", eventLog);
			ArgumentValidator.ThrowIfNull("trackerLogInterval", trackerLogInterval);
			ArgumentValidator.ThrowIfNull("receiveConnectors", receiveConnectors);
			this.smtpProxyTracker = new ConnectionsTracker(getConnectionsCurrent, getConnectionsTotal);
			if (!trackingEnabled)
			{
				return;
			}
			this.trackingEnabled = true;
			this.trackerType = trackerType;
			this.rejectBasedOnInboundProxyDestinationTrackingEnabled = rejectBasedOnInboundProxyDestinationTrackingEnabled;
			this.perDestinationConnectionPercentageThreshold = perDestinationConnectionPercentageThreshold;
			this.tryGetDestinationConnectionThresholdDelegate = tryGetDestinationConnectionThresholdDelegate;
			this.perDestinationConnectionThreshold = this.GetThresholdFromConnectors(receiveConnectors);
			this.eventLog = eventLog;
			this.logTimer = new GuardedTimer(new TimerCallback(this.LogData), null, trackerLogInterval);
		}

		public void IncrementProxyCount(string destination)
		{
			if (!this.trackingEnabled)
			{
				return;
			}
			this.smtpProxyTracker.IncrementProxyCount(destination);
		}

		public void DecrementProxyCount(string destination)
		{
			if (!this.trackingEnabled)
			{
				return;
			}
			this.smtpProxyTracker.DecrementProxyCount(destination);
		}

		public bool ShouldRejectMessage(string destination, out SmtpResponse rejectResponse)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("destination", destination);
			if (!this.trackingEnabled)
			{
				rejectResponse = SmtpResponse.Empty;
				return false;
			}
			int usage = this.smtpProxyTracker.GetUsage(destination);
			int threhsoldValue;
			if (this.tryGetDestinationConnectionThresholdDelegate(destination, out threhsoldValue) && this.IsOverThreshold(usage, threhsoldValue, destination))
			{
				if (this.rejectBasedOnInboundProxyDestinationTrackingEnabled)
				{
					rejectResponse = SmtpResponse.InboundProxyDestinationTrackerReject;
					return true;
				}
			}
			else if (this.IsOverThreshold(usage, this.perDestinationConnectionThreshold, destination) && this.rejectBasedOnInboundProxyDestinationTrackingEnabled)
			{
				rejectResponse = SmtpResponse.InboundProxyDestinationTrackerReject;
				return true;
			}
			rejectResponse = SmtpResponse.Empty;
			return false;
		}

		public bool TryGetDiagnosticInfo(DiagnosableParameters parameters, out XElement diagnosticInfo)
		{
			if (this.trackingEnabled && (parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1 || parameters.Argument.Equals(this.trackerType.ToString(), StringComparison.InvariantCultureIgnoreCase)))
			{
				diagnosticInfo = this.GetDiagnosticInfo();
				return true;
			}
			diagnosticInfo = null;
			return false;
		}

		public void UpdateReceiveConnectors(IEnumerable<ReceiveConnector> receiveConnectors)
		{
			ArgumentValidator.ThrowIfNull("receiveConnectors", receiveConnectors);
			if (!this.trackingEnabled)
			{
				return;
			}
			this.perDestinationConnectionThreshold = this.GetThresholdFromConnectors(receiveConnectors);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<InboundProxyDestinationTracker>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.logTimer != null)
			{
				this.logTimer.Dispose(true);
				this.logTimer = null;
			}
		}

		private bool IsOverThreshold(int currentValue, int threhsoldValue, string destination)
		{
			if (currentValue >= threhsoldValue)
			{
				this.eventLog.LogEvent((this.trackerType == InboundProxyTrackerType.InboundProxyDestinationTracker) ? TransportEventLogConstants.Tuple_InboundProxyDestinationsTrackerReject : TransportEventLogConstants.Tuple_InboundProxyAccountForestsTrackerReject, destination, new object[]
				{
					destination,
					currentValue,
					threhsoldValue
				});
				EventNotificationItem.PublishPeriodic(ExchangeComponent.FrontendTransport.Name, "InboundProxyDestinationsTrackerReject", null, string.Format("{0} will reject connections for destination {1} as current usage {2} is greater than or equal to threshold {3}", new object[]
				{
					this.trackerType,
					destination,
					currentValue,
					threhsoldValue
				}), destination, TimeSpan.FromMinutes(15.0), ResultSeverityLevel.Error, false);
				return true;
			}
			if ((double)currentValue >= 0.8 * (double)threhsoldValue)
			{
				this.eventLog.LogEvent((this.trackerType == InboundProxyTrackerType.InboundProxyDestinationTracker) ? TransportEventLogConstants.Tuple_InboundProxyDestinationsTrackerNearThreshold : TransportEventLogConstants.Tuple_InboundProxyAccountForestsTrackerNearThreshold, destination, new object[]
				{
					destination,
					currentValue,
					threhsoldValue
				});
				EventNotificationItem.PublishPeriodic(ExchangeComponent.FrontendTransport.Name, "InboundProxyDestinationsTrackerNearThreshold", null, string.Format("{0} shows that connections for next hop {1} is currently {2} and close to threshold {3}.", new object[]
				{
					this.trackerType,
					destination,
					currentValue,
					threhsoldValue
				}), destination, TimeSpan.FromMinutes(15.0), ResultSeverityLevel.Error, false);
			}
			return false;
		}

		private XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement(this.trackerType.ToString());
			xelement.SetAttributeValue("PerDestinationThreshold", this.perDestinationConnectionThreshold);
			xelement.Add(this.smtpProxyTracker.GetDiagnosticInfo("Destination"));
			return xelement;
		}

		private void LogData(object state)
		{
			this.eventLog.LogEvent(TransportEventLogConstants.Tuple_InboundProxyDestinationsTrackerDiagnosticInfo, null, new object[]
			{
				this.trackerType.ToString(),
				this.GetDiagnosticInfo()
			});
		}

		private int GetThresholdFromConnectors(IEnumerable<ReceiveConnector> connectors)
		{
			int num = 0;
			bool flag = false;
			foreach (ReceiveConnector receiveConnector in connectors)
			{
				if (receiveConnector.Enabled)
				{
					flag = true;
					num = Math.Max(num, receiveConnector.MaxInboundConnection.Value);
				}
			}
			if (flag)
			{
				return num * this.perDestinationConnectionPercentageThreshold / 100;
			}
			return 1000;
		}

		private const int DefaultPerDestinationMaxConnectionsThreshold = 1000;

		private readonly InboundProxyTrackerType trackerType;

		private readonly bool trackingEnabled;

		public bool rejectBasedOnInboundProxyDestinationTrackingEnabled;

		private readonly int perDestinationConnectionPercentageThreshold;

		private readonly InboundProxyDestinationTracker.TryGetDestinationConnectionThresholdDelegate tryGetDestinationConnectionThresholdDelegate;

		private int perDestinationConnectionThreshold;

		private readonly IExEventLog eventLog;

		private GuardedTimer logTimer;

		private readonly ConnectionsTracker smtpProxyTracker;

		public delegate bool TryGetDestinationConnectionThresholdDelegate(string destination, out int threshold);
	}
}

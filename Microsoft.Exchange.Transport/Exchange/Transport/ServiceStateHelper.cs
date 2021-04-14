using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal class ServiceStateHelper
	{
		public ServiceStateHelper(ITransportConfiguration configuration, string hostName)
		{
			this.configuration = configuration;
			this.hostName = hostName;
			this.stateChangeEventTuple = this.GetEventLogStateChangeTuple(hostName);
		}

		public static ServiceState GetServiceState(ITransportConfiguration configuration, string hostName)
		{
			return ServerComponentStates.ReadEffectiveComponentState(null, configuration.LocalServer.TransportServer.ComponentStates, hostName, ServiceStateHelper.GetDefaultServiceState());
		}

		public static ServiceState GetDefaultServiceState()
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Transport.DefaultTransportServiceStateToInactive.Enabled)
			{
				return ServiceState.Active;
			}
			return ServiceState.Inactive;
		}

		public bool CheckState(ServiceState? initialState)
		{
			if (!this.configuration.AppConfig.StateManagement.StateChangeDetectionEnabled || initialState == null)
			{
				return false;
			}
			if (Components.ShuttingDown)
			{
				ExTraceGlobals.ServiceTracer.TraceDebug(0L, "Service shutting down. State checking skipped.");
				return false;
			}
			ServiceState serviceState = ServiceStateHelper.GetServiceState(this.configuration, this.hostName);
			if (initialState.Value != serviceState)
			{
				if (initialState.Value == ServiceState.Active && Components.IsPaused && serviceState == ServiceState.Draining)
				{
					ExTraceGlobals.ServiceTracer.TraceDebug(0L, "Initial service state is Active, but service is paused and target state is draining.");
				}
				else
				{
					ServiceStateHelper.eventLogger.LogEvent(this.stateChangeEventTuple, null, new object[]
					{
						initialState.Value,
						serviceState
					});
					EventNotificationItem.Publish(this.hostName, "ServiceStateChange", null, string.Format("Inconsistent state: current = {0}, target = {1}", initialState.Value, serviceState), ResultSeverityLevel.Warning, false);
				}
				return true;
			}
			return false;
		}

		private ExEventLog.EventTuple GetEventLogStateChangeTuple(string hostname)
		{
			if (this.hostName == ServerComponentStates.GetComponentId(ServerComponentEnum.HubTransport))
			{
				return TransportEventLogConstants.Tuple_HubTransportServiceStateChanged;
			}
			if (this.hostName == ServerComponentStates.GetComponentId(ServerComponentEnum.EdgeTransport))
			{
				return TransportEventLogConstants.Tuple_EdgeTransportServiceStateChanged;
			}
			if (this.hostName == ServerComponentStates.GetComponentId(ServerComponentEnum.FrontendTransport))
			{
				return TransportEventLogConstants.Tuple_FrontendTransportServiceStateChanged;
			}
			throw new ArgumentException("hostname");
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ServiceTracer.Category, TransportEventLog.GetEventSource());

		private readonly string hostName;

		private readonly ExEventLog.EventTuple stateChangeEventTuple;

		private ITransportConfiguration configuration;
	}
}

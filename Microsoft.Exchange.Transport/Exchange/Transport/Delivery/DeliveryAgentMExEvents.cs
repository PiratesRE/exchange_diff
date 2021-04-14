using System;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Delivery;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class DeliveryAgentMExEvents
	{
		public ExEventLog EventLogger
		{
			get
			{
				return this.eventLogger;
			}
		}

		public virtual DeliveryAgentMExEvents.DeliveryAgentMExSession GetExecutionContext(string deliveryProtocol, SmtpServer smtpServer, Action startAsyncAgentCallback, Action completedAsyncAgentCallback, Func<bool> resumeAgentCallback)
		{
			if (this.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before GetExecutionContext()");
			}
			return new DeliveryAgentMExEvents.DeliveryAgentMExSession(this.mexRuntime.CreateSession(smtpServer, deliveryProtocol, startAsyncAgentCallback, completedAsyncAgentCallback, resumeAgentCallback));
		}

		public virtual void FreeExecutionContext(DeliveryAgentMExEvents.DeliveryAgentMExSession context)
		{
			if (this.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before FreeExecutionContext()");
			}
			context.Close();
		}

		public virtual IAsyncResult RaiseEvent(DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession, string eventTopic, AsyncCallback callback, object state, params object[] contexts)
		{
			if (this.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before RaiseEvent()");
			}
			IAsyncResult result;
			try
			{
				result = mexSession.BeginInvoke(eventTopic, contexts[0], contexts[1], callback, state);
			}
			catch (LocalizedException e)
			{
				this.HandleAgentExchangeExceptions(mexSession, e);
				throw;
			}
			return result;
		}

		public virtual void EndEvent(DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession, IAsyncResult ar)
		{
			try
			{
				mexSession.EndInvoke(ar);
			}
			catch (LocalizedException e)
			{
				this.HandleAgentExchangeExceptions(mexSession, e);
				throw;
			}
		}

		public virtual void Initialize(string configFilePath)
		{
			if (this.mexRuntime != null)
			{
				throw new InvalidOperationException("Cannot Initialize() again without calling Shutdown() first");
			}
			this.mexRuntime = new MExRuntime();
			this.mexRuntime.Initialize(configFilePath, typeof(DeliveryAgent).ToString(), Components.Configuration.ProcessTransportRole, ConfigurationContext.Setup.InstallPath, new FactoryInitializer(ProcessAccessManager.RegisterAgentFactory));
			AgentLatencyTracker.RegisterMExRuntime(LatencyAgentGroup.Delivery, this.mexRuntime);
		}

		public virtual void Shutdown()
		{
			if (this.mexRuntime != null)
			{
				this.mexRuntime.Shutdown();
				this.mexRuntime = null;
			}
		}

		private void HandleAgentExchangeExceptions(DeliveryAgentMExEvents.DeliveryAgentMExSession mexSession, LocalizedException e)
		{
			ExTraceGlobals.ExtensibilityTracer.TraceError<string, string, LocalizedException>(0L, "Agent {0} running in context {1} hit Unhandled Exception {2}", mexSession.CurrentAgentName, mexSession.EventTopic, e);
			ExEventLog.EventTuple tuple;
			if (string.Equals(mexSession.EventTopic, "OnOpenConnection", StringComparison.OrdinalIgnoreCase))
			{
				tuple = TransportEventLogConstants.Tuple_OnOpenConnectionAgentException;
			}
			else if (string.Equals(mexSession.EventTopic, "OnDeliverMailItem", StringComparison.OrdinalIgnoreCase))
			{
				tuple = TransportEventLogConstants.Tuple_OnDeliverMailItemAgentException;
			}
			else
			{
				if (!string.Equals(mexSession.EventTopic, "OnCloseConnection", StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException("Unknown agent type");
				}
				tuple = TransportEventLogConstants.Tuple_OnCloseConnectionAgentException;
			}
			this.EventLogger.LogEvent(tuple, null, new object[]
			{
				mexSession.CurrentAgentName,
				e
			});
		}

		public XElement[] GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			if (this.mexRuntime != null)
			{
				return this.mexRuntime.GetDiagnosticInfo(parameters, "DeliveryAgent");
			}
			return null;
		}

		private MExRuntime mexRuntime;

		private ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ExtensibilityTracer.Category, TransportEventLog.GetEventSource());

		public class DeliveryAgentMExSession
		{
			public DeliveryAgentMExSession(IMExSession mexSession)
			{
				this.mexSession = mexSession;
				this.agentLatencyTracker = new AgentLatencyTracker(this.mexSession);
			}

			protected DeliveryAgentMExSession()
			{
			}

			public virtual string CurrentAgentName
			{
				get
				{
					if (this.mexSession.CurrentAgent != null)
					{
						return this.mexSession.CurrentAgent.Name;
					}
					return string.Empty;
				}
			}

			public virtual string EventTopic
			{
				get
				{
					return this.mexSession.EventTopic;
				}
			}

			public virtual AgentLatencyTracker AgentLatencyTracker
			{
				get
				{
					return this.agentLatencyTracker;
				}
			}

			public virtual IAsyncResult BeginInvoke(string topic, object source, object e, AsyncCallback callback, object callbackState)
			{
				return this.mexSession.BeginInvoke(topic, source, e, callback, callbackState);
			}

			public virtual void EndInvoke(IAsyncResult ar)
			{
				this.mexSession.EndInvoke(ar);
			}

			public virtual void Close()
			{
				this.agentLatencyTracker.Dispose();
				this.agentLatencyTracker = null;
				this.mexSession.Close();
				this.mexSession = null;
			}

			private IMExSession mexSession;

			private AgentLatencyTracker agentLatencyTracker;
		}
	}
}

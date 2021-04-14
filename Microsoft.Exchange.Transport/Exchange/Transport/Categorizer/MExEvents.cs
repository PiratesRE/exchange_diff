using System;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MExEvents
	{
		public static ExEventLog EventLogger
		{
			get
			{
				return MExEvents.eventLogger;
			}
		}

		public static IMExSession GetExecutionContext(TransportMailItem currentMailItem, AcceptedDomainCollection acceptedDomains, Action asyncStartAgentCallback, Action asyncCompleteAgentCallback, Func<bool> resumeAgentCallback)
		{
			if (MExEvents.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before GetExecutionContext()");
			}
			return MExEvents.mexRuntime.CreateSession(CatServer.GetInstance(currentMailItem, acceptedDomains), "CAT", asyncStartAgentCallback, asyncCompleteAgentCallback, resumeAgentCallback);
		}

		public static void FreeExecutionContext(IMExSession context)
		{
			if (MExEvents.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before FreeExecutionContext()");
			}
			context.Close();
		}

		public static IMExSession CloneExecutionContext(IMExSession mexSession)
		{
			return (IMExSession)mexSession.Clone();
		}

		public static IAsyncResult RaiseEvent(IMExSession mexSession, string eventTopic, AsyncCallback callback, object state, params object[] contexts)
		{
			if (MExEvents.mexRuntime == null)
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
				MExEvents.HandleAgentExchangeExceptions(mexSession, e);
				throw;
			}
			return result;
		}

		public static void EndEvent(IMExSession mexSession, IAsyncResult ar)
		{
			try
			{
				mexSession.EndInvoke(ar);
			}
			catch (LocalizedException e)
			{
				MExEvents.HandleAgentExchangeExceptions(mexSession, e);
				throw;
			}
		}

		public static void Initialize(string configFilePath)
		{
			if (MExEvents.mexRuntime != null)
			{
				throw new InvalidOperationException("Cannot Initialize() again without calling Shutdown() first");
			}
			MExEvents.mexRuntime = new MExRuntime();
			MExEvents.mexRuntime.Initialize(configFilePath, "Microsoft.Exchange.Data.Transport.Routing.RoutingAgent", Components.Configuration.ProcessTransportRole, ConfigurationContext.Setup.InstallPath, delegate(AgentFactory agentFactory)
			{
				IDiagnosable diagnosable = agentFactory as IDiagnosable;
				if (diagnosable != null)
				{
					ProcessAccessManager.RegisterComponent(diagnosable);
				}
			});
			AgentLatencyTracker.RegisterMExRuntime(LatencyAgentGroup.Categorizer, MExEvents.mexRuntime);
		}

		public static void Shutdown()
		{
			if (MExEvents.mexRuntime != null)
			{
				MExEvents.mexRuntime.Shutdown();
				MExEvents.mexRuntime = null;
			}
		}

		public static IAsyncResult RaiseOnSubmittedMessage(TaskContext context, AsyncCallback callback, MailItem mailItem)
		{
			InternalSubmittedMessageSource internalSubmittedMessageSource = new InternalSubmittedMessageSource();
			AgentSubmittedMessageSource agentSubmittedMessageSource = new AgentSubmittedMessageSource();
			internalSubmittedMessageSource.Initialize(context.MexSession, context);
			agentSubmittedMessageSource.Initialize(internalSubmittedMessageSource);
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = new AgentQueuedMessageEventArgs(mailItem);
			return MExEvents.RaiseEvent(context.MexSession, "OnSubmittedMessage", callback, context, new object[]
			{
				agentSubmittedMessageSource,
				agentQueuedMessageEventArgs
			});
		}

		public static IAsyncResult RaiseOnResolvedMessage(TaskContext context, AsyncCallback callback, MailItem mailItem)
		{
			InternalResolvedMessageSource internalResolvedMessageSource = new InternalResolvedMessageSource();
			AgentResolvedMessageSource agentResolvedMessageSource = new AgentResolvedMessageSource();
			internalResolvedMessageSource.Initialize(context.MexSession, context);
			agentResolvedMessageSource.Initialize(internalResolvedMessageSource);
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = new AgentQueuedMessageEventArgs(mailItem);
			return MExEvents.RaiseEvent(context.MexSession, "OnResolvedMessage", callback, context, new object[]
			{
				agentResolvedMessageSource,
				agentQueuedMessageEventArgs
			});
		}

		public static IAsyncResult RaiseOnRoutedMessage(TaskContext context, AsyncCallback callback, MailItem mailItem)
		{
			InternalRoutedMessageSource internalRoutedMessageSource = new InternalRoutedMessageSource();
			AgentRoutedMessageSource agentRoutedMessageSource = new AgentRoutedMessageSource();
			internalRoutedMessageSource.Initialize(context.MexSession, context);
			agentRoutedMessageSource.Initialize(internalRoutedMessageSource);
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = new AgentQueuedMessageEventArgs(mailItem);
			return MExEvents.RaiseEvent(context.MexSession, "OnRoutedMessage", callback, context, new object[]
			{
				agentRoutedMessageSource,
				agentQueuedMessageEventArgs
			});
		}

		public static IAsyncResult RaiseOnCategorizedMessage(TaskContext context, AsyncCallback callback, MailItem mailItem)
		{
			InternalCategorizedMessageSource internalCategorizedMessageSource = new InternalCategorizedMessageSource();
			AgentCategorizedMessageSource agentCategorizedMessageSource = new AgentCategorizedMessageSource();
			internalCategorizedMessageSource.Initialize(context.MexSession, context);
			agentCategorizedMessageSource.Initialize(internalCategorizedMessageSource);
			AgentQueuedMessageEventArgs agentQueuedMessageEventArgs = new AgentQueuedMessageEventArgs(mailItem);
			return MExEvents.RaiseEvent(context.MexSession, "OnCategorizedMessage", callback, context, new object[]
			{
				agentCategorizedMessageSource,
				agentQueuedMessageEventArgs
			});
		}

		public static void HandleAgentExchangeExceptions(IMExSession mexSession, LocalizedException e)
		{
			ExTraceGlobals.ExtensibilityTracer.TraceError<string, string, LocalizedException>(0L, "Agent {0} running in context {1} hit Unhandled Exception {2}", (mexSession.CurrentAgent == null) ? null : mexSession.CurrentAgent.Name, mexSession.EventTopic, e);
			ExEventLog.EventTuple tuple;
			if (string.Equals(mexSession.EventTopic, "OnSubmittedMessage", StringComparison.OrdinalIgnoreCase))
			{
				tuple = TransportEventLogConstants.Tuple_OnSubmittedMessageAgentException;
			}
			else if (string.Equals(mexSession.EventTopic, "OnRoutedMessage", StringComparison.OrdinalIgnoreCase))
			{
				tuple = TransportEventLogConstants.Tuple_OnRoutedMessageAgentException;
			}
			else if (string.Equals(mexSession.EventTopic, "OnResolvedMessage", StringComparison.OrdinalIgnoreCase))
			{
				tuple = TransportEventLogConstants.Tuple_OnResolvedMessageAgentException;
			}
			else
			{
				if (!string.Equals(mexSession.EventTopic, "OnCategorizedMessage", StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException("Unknown agent type");
				}
				tuple = TransportEventLogConstants.Tuple_OnCategorizedMessageAgentException;
			}
			MExEvents.eventLogger.LogEvent(tuple, null, new object[]
			{
				(mexSession.CurrentAgent == null) ? null : mexSession.CurrentAgent.Name,
				e
			});
		}

		public static XElement[] GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			if (MExEvents.mexRuntime != null)
			{
				return MExEvents.mexRuntime.GetDiagnosticInfo(parameters, "RoutingAgent");
			}
			return null;
		}

		private static MExRuntime mexRuntime;

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ExtensibilityTracer.Category, TransportEventLog.GetEventSource());
	}
}

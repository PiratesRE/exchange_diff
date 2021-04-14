using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver.Shared
{
	internal static class MExEvents
	{
		public static IMExSession GetExecutionContext(StoreDriverServer storeDriverServer)
		{
			if (MExEvents.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before GetExecutionContext()");
			}
			IMExSession imexSession = MExEvents.mexRuntime.CreateSession(storeDriverServer, "SD");
			imexSession.Dispatcher.OnAgentInvokeStart += new AgentInvokeStartHandler(MExEvents.OnAgentInvokeStart);
			imexSession.Dispatcher.OnAgentInvokeEnd += new AgentInvokeEndHandler(MExEvents.OnAgentInvokeReturns);
			return imexSession;
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

		public static void RaiseEvent(IMExSession mexSession, string eventTopic, params object[] contexts)
		{
			if (MExEvents.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before RaiseEvent()");
			}
			IAsyncResult asyncResult = null;
			try
			{
				asyncResult = mexSession.BeginInvoke(eventTopic, contexts[0], contexts[1], null, null);
				mexSession.EndInvoke(asyncResult);
			}
			catch (LocalizedException e)
			{
				MExEvents.HandleAgentExchangeExceptions(mexSession, e);
			}
			MExAsyncResult mexAsyncResult = (MExAsyncResult)asyncResult;
			if (mexAsyncResult != null && mexAsyncResult.AsyncException != null)
			{
				throw new StoreDriverAgentRaisedException(mexAsyncResult.FaultyAgentName, mexAsyncResult.AsyncException);
			}
		}

		public static void Initialize(string configFilePath, ProcessTransportRole role, LatencyAgentGroup latencyAgentGroup, string agentGroup)
		{
			if (MExEvents.mexRuntime != null)
			{
				throw new InvalidOperationException("Cannot Initialize() again without calling Shutdown() first");
			}
			MExEvents.processTransportRole = role;
			MExEvents.latencyAgentGroup = latencyAgentGroup;
			MExEvents.agentGroup = agentGroup;
			MExEvents.mexRuntime = new MExRuntime();
			MExEvents.mexRuntime.Initialize(configFilePath, agentGroup, role, ConfigurationContext.Setup.InstallPath, new FactoryInitializer(ProcessAccessManager.RegisterAgentFactory));
			AgentLatencyTracker.RegisterMExRuntime(latencyAgentGroup, MExEvents.mexRuntime);
		}

		public static void Shutdown()
		{
			if (MExEvents.mexRuntime != null)
			{
				MExEvents.mexRuntime.Shutdown();
				MExEvents.mexRuntime = null;
			}
		}

		public static void HandleAgentExchangeExceptions(IMExSession mexSession, LocalizedException e)
		{
			TraceHelper.ExtensibilityTracer.TraceFail<string, string, LocalizedException>(TraceHelper.MessageProbeActivityId, 0L, "Agent {0} running in context {1} hit Unhandled Exception {2}", (mexSession.CurrentAgent == null) ? null : mexSession.CurrentAgent.Name, mexSession.EventTopic, e);
		}

		private static void OnAgentInvokeStart(object dispatcher, IMExSession context)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				currentActivityScope.Action = context.CurrentAgent.Name;
			}
		}

		private static void OnAgentInvokeReturns(object dispatcher, IMExSession context)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				currentActivityScope.Action = null;
			}
		}

		private static MExRuntime mexRuntime;

		private static ProcessTransportRole processTransportRole;

		private static LatencyAgentGroup latencyAgentGroup;

		private static string agentGroup;
	}
}

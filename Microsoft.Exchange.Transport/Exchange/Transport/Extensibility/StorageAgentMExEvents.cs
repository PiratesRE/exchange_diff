using System;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal static class StorageAgentMExEvents
	{
		public static void Initialize(string configFilePath)
		{
			if (StorageAgentMExEvents.mexRuntime != null)
			{
				throw new InvalidOperationException("Cannot Initialize() again without calling Shutdown() first");
			}
			StorageAgentMExEvents.mexRuntime = new MExRuntime();
			StorageAgentMExEvents.mexRuntime.Initialize(configFilePath, "Microsoft.Exchange.Data.Transport.Storage.StorageAgent", Components.Configuration.ProcessTransportRole, ConfigurationContext.Setup.InstallPath, new FactoryInitializer(ProcessAccessManager.RegisterAgentFactory));
			AgentLatencyTracker.RegisterMExRuntime(LatencyAgentGroup.Storage, StorageAgentMExEvents.mexRuntime);
		}

		public static void Shutdown()
		{
			if (StorageAgentMExEvents.mexRuntime != null)
			{
				StorageAgentMExEvents.mexRuntime.Shutdown();
				StorageAgentMExEvents.mexRuntime = null;
			}
		}

		public static IMExSession GetExecutionContext()
		{
			if (StorageAgentMExEvents.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize must be called before GetExecutionContext");
			}
			StorageAgentState state = new StorageAgentState();
			return StorageAgentMExEvents.mexRuntime.CreateSession(state, "BootScanner");
		}

		public static void FreeExecutionContext(MExSession context)
		{
			if (StorageAgentMExEvents.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize must be called before FreeExecutionContext");
			}
			context.Close();
		}

		public static void RaiseEvent(IMExSession mexSession, string eventTopic, params object[] contexts)
		{
			if (StorageAgentMExEvents.mexRuntime == null)
			{
				throw new InvalidOperationException("Initialize() must be called before RaiseEvent()");
			}
			try
			{
				mexSession.Invoke(eventTopic, contexts[0], contexts[1]);
			}
			catch (LocalizedException e)
			{
				StorageAgentMExEvents.TraceAgentExchangeExceptions(mexSession, e);
				throw;
			}
		}

		public static void TraceAgentExchangeExceptions(IMExSession mexSession, LocalizedException e)
		{
			ExTraceGlobals.ExtensibilityTracer.TraceError<string, string, LocalizedException>(0L, "Agent {0} running in context {1} hit Unhandled Exception {2}", (mexSession.CurrentAgent == null) ? null : mexSession.CurrentAgent.Name, mexSession.EventTopic, e);
		}

		public static XElement[] GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			if (StorageAgentMExEvents.mexRuntime != null)
			{
				return StorageAgentMExEvents.mexRuntime.GetDiagnosticInfo(parameters, "StorageAgent");
			}
			return null;
		}

		private static MExRuntime mexRuntime;
	}
}

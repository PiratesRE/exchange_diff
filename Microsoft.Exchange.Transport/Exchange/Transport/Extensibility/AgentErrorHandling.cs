using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Internal.MExRuntime;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandling
	{
		public AgentErrorHandling(IEnumerable<AgentErrorHandlingDefinition> agentMap)
		{
			this.agentMap = agentMap;
			if (AgentErrorHandling.overrideMap == null)
			{
				try
				{
					AgentErrorHandlingOverrideSection agentErrorHandlingOverrideSection = (AgentErrorHandlingOverrideSection)ConfigurationManager.GetSection("agentErrorHandlingOverride");
					if (agentErrorHandlingOverrideSection != null && agentErrorHandlingOverrideSection.Overrides != null)
					{
						AgentErrorHandling.overrideMap = agentErrorHandlingOverrideSection.Overrides.Cast<AgentErrorHandlingOverrideSection.Override>().Select(new Func<AgentErrorHandlingOverrideSection.Override, AgentErrorHandlingDefinition>(this.GetDefinitionFromConfig)).ToArray<AgentErrorHandlingDefinition>();
					}
				}
				catch (ConfigurationErrorsException ex)
				{
					Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_AgentErrorHandlingOverrideConfigError, null, new object[]
					{
						ex
					});
				}
			}
		}

		internal AgentErrorHandling(IEnumerable<AgentErrorHandlingDefinition> defaultMap, IEnumerable<AgentErrorHandlingDefinition> agentMap) : this(agentMap)
		{
			AgentErrorHandlingMap.DefaultMap = defaultMap;
		}

		internal AgentErrorHandling(IEnumerable<AgentErrorHandlingDefinition> defaultMap, IEnumerable<AgentErrorHandlingDefinition> agentMap, IEnumerable<AgentErrorHandlingDefinition> overrideMap)
		{
			this.agentMap = agentMap;
			AgentErrorHandling.overrideMap = overrideMap;
			AgentErrorHandlingMap.DefaultMap = defaultMap;
		}

		public bool TryHandle(string contextId, Exception exception, QueuedMessageEventSource source, MailItem mailItem)
		{
			IErrorHandlingAction errorHandlingAction = this.GetErrorHandlingAction(contextId, exception, mailItem);
			if (errorHandlingAction != null)
			{
				errorHandlingAction.TakeAction(source, mailItem);
				return true;
			}
			return false;
		}

		public IErrorHandlingAction GetErrorHandlingAction(string contextId, Exception exception, MailItem mailItem)
		{
			TransportMailItem mailItem2 = null;
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = mailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade != null)
			{
				mailItem2 = (transportMailItemWrapperFacade.TransportMailItem as TransportMailItem);
			}
			return this.GetErrorHandlingAction(contextId, exception, mailItem2);
		}

		public IErrorHandlingAction GetErrorHandlingAction(string contextId, Exception exception, TransportMailItem mailItem)
		{
			AgentErrorHandlingDefinition agentErrorHandlingDefinition = null;
			if (AgentErrorHandling.overrideMap != null)
			{
				agentErrorHandlingDefinition = AgentErrorHandling.overrideMap.FirstOrDefault((AgentErrorHandlingDefinition d) => d.Condition.IsMatch(contextId, exception, mailItem));
				if (agentErrorHandlingDefinition != null)
				{
					MExCounters.GetInstance(contextId).TotalAgentErrorHandlingOverrides.Increment();
					ExTraceGlobals.ExtensibilityTracer.TraceDebug(0L, "Using override[{2}({3})] for error [{0}],[{1}].", new object[]
					{
						contextId,
						exception,
						agentErrorHandlingDefinition.Name,
						agentErrorHandlingDefinition.Action.ActionType
					});
				}
			}
			if (agentErrorHandlingDefinition == null && this.agentMap != null)
			{
				agentErrorHandlingDefinition = this.agentMap.FirstOrDefault((AgentErrorHandlingDefinition o) => o.Condition.IsMatch(contextId, exception, mailItem));
			}
			if (agentErrorHandlingDefinition == null && AgentErrorHandlingMap.DefaultMap != null)
			{
				agentErrorHandlingDefinition = AgentErrorHandlingMap.DefaultMap.FirstOrDefault((AgentErrorHandlingDefinition o) => o.Condition.IsMatch(contextId, exception, mailItem));
			}
			if (agentErrorHandlingDefinition != null && mailItem != null)
			{
				int num = contextId.IndexOf('.');
				string agentName = (num == -1) ? contextId : contextId.Substring(0, num);
				mailItem.AddAgentInfo(agentName, "Error handling action", new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("ErrorHandlingContextId", contextId),
					new KeyValuePair<string, string>("ErrorHandlingRuleName", agentErrorHandlingDefinition.Name),
					new KeyValuePair<string, string>("ErrorHandlingActionType", agentErrorHandlingDefinition.Action.ActionType.ToString())
				});
			}
			else
			{
				ExTraceGlobals.ExtensibilityTracer.TraceDebug<string, Exception>(0L, "No error handler or override defined for [{0}],[{1}].", contextId, exception);
			}
			if (agentErrorHandlingDefinition != null)
			{
				return agentErrorHandlingDefinition.Action;
			}
			return null;
		}

		private AgentErrorHandlingDefinition GetDefinitionFromConfig(AgentErrorHandlingOverrideSection.Override configOverride)
		{
			if (configOverride == null)
			{
				throw new ConfigurationErrorsException("configOverride is null");
			}
			AgentErrorHandlingDefinition result;
			try
			{
				AgentErrorHandlingCondition condition = new AgentErrorHandlingCondition(configOverride.Condition.ContextId, configOverride.Condition.ExceptionType, configOverride.Condition.DeferCount, configOverride.Condition.ExceptionMessage);
				IErrorHandlingAction action;
				switch (configOverride.Action.ActionType)
				{
				case ErrorHandlingActionType.NDR:
					action = new AgentErrorHandlingNdrAction(new SmtpResponse(configOverride.Action.NDRStatusCode, configOverride.Action.NDREnhancedStatusCode, new string[]
					{
						configOverride.Action.NDRStatusText,
						configOverride.Action.NDRMessage
					}));
					break;
				case ErrorHandlingActionType.Defer:
					action = new AgentErrorHandlingDeferAction(configOverride.Action.DeferInterval, configOverride.Action.IsIntervalProgressive);
					break;
				case ErrorHandlingActionType.Drop:
					action = new AgentErrorHandlingDropAction();
					break;
				case ErrorHandlingActionType.Allow:
					action = new AgentErrorHandlingAllowAction();
					break;
				default:
					throw new ConfigurationErrorsException("Unexpected ErrorHandlingActionType.");
				}
				result = new AgentErrorHandlingDefinition(configOverride.Name, condition, action);
			}
			catch (ArgumentNullException inner)
			{
				throw new ConfigurationErrorsException("Bad Error Handling Override config.", inner);
			}
			return result;
		}

		private const string ConfigSectionName = "agentErrorHandlingOverride";

		internal const string AgentErrorDeferCount = "Microsoft.Exchange.Transport.AgentErrorDeferCount";

		private readonly IEnumerable<AgentErrorHandlingDefinition> agentMap;

		private static IEnumerable<AgentErrorHandlingDefinition> overrideMap;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Extensibility;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Filtering;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class TransportRulesErrorHandler
	{
		private static List<Type> InitializeKnownRuleEvaluationExceptions()
		{
			List<Type> list = new List<Type>
			{
				typeof(TransportRuleTimeoutException),
				typeof(ADTransientException),
				typeof(TransportRuleTransientException),
				typeof(RegexMatchTimeoutException),
				typeof(TransportRulePermanentException),
				typeof(RuleInvalidOperationException),
				typeof(InvalidTransportRuleEventSourceTypeException),
				typeof(OutboundConnectorNotFoundException)
			};
			list.AddRange(TransportRulesErrorHandler.filteringServiceExceptions);
			return list;
		}

		internal static IErrorHandlingAction GetErrorHandlingAction(Exception exception, MailItem mailItem)
		{
			IErrorHandlingAction errorHandlingAction = TransportRulesErrorHandler.AgentErrorHandler.GetErrorHandlingAction(TrackAgentInfoAgentName.TRA.ToString("G"), exception, mailItem);
			if (errorHandlingAction != null)
			{
				AgentErrorHandlingDeferAction agentErrorHandlingDeferAction = errorHandlingAction as AgentErrorHandlingDeferAction;
				if (agentErrorHandlingDeferAction != null)
				{
					string unicodeString = string.Format("Message deferred. {0}, tenant - {1}", exception.Message, TransportUtils.GetOrganizationID(mailItem).ToString());
					SmtpResponse value = new SmtpResponse("421", "4.7.11", "Message deferred by Transport Rules Agent", true, new string[]
					{
						TransportRulesErrorHandler.EncodeStringToUtf7(unicodeString)
					});
					agentErrorHandlingDeferAction.SmtpResponse = new SmtpResponse?(value);
				}
			}
			return errorHandlingAction;
		}

		internal static bool IsKnownFipsException(Exception ex)
		{
			return TransportRulesErrorHandler.filteringServiceExceptions.Any((Type fipsException) => fipsException.IsInstanceOfType(ex));
		}

		internal static bool IsTimeoutException(Exception ex)
		{
			return TransportRulesErrorHandler.timeoutExceptions.Any((Type timeoutException) => timeoutException.IsInstanceOfType(ex));
		}

		internal static ExecutionStatus ErrorActionToExecutionStatus(ErrorHandlingActionType errorActionType)
		{
			switch (errorActionType)
			{
			case ErrorHandlingActionType.NDR:
			case ErrorHandlingActionType.Drop:
				return ExecutionStatus.PermanentError;
			case ErrorHandlingActionType.Defer:
				return ExecutionStatus.TransientError;
			case ErrorHandlingActionType.Allow:
				return ExecutionStatus.Success;
			default:
				return ExecutionStatus.PermanentError;
			}
		}

		internal static void LogFailureEvent(MailItem mailItem, ExEventLog.EventTuple eventLog, string errorMessage)
		{
			string text = string.Format("{0} Message-Id:{1}", errorMessage, TransportUtils.GetMessageID(mailItem));
			ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, errorMessage);
			TransportAction.Logger.LogEvent(eventLog, null, new object[]
			{
				text
			});
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, string.Format("{0}.Event-0x{1:X}", TransportRulesEvaluator.ActiveMonitoringComponentName, eventLog.EventId), null, text, ResultSeverityLevel.Error, false);
			SystemProbeHelper.EtrTracer.TraceFail(mailItem, 0L, "Error processing rules. Details: {0}", text);
		}

		internal static bool IsDeferredOrDeleted(ExecutionStatus ruleExecutionStatus)
		{
			return ruleExecutionStatus == ExecutionStatus.SuccessMailItemDeleted || ruleExecutionStatus == ExecutionStatus.SuccessMailItemDeferred || ruleExecutionStatus == ExecutionStatus.PermanentError || ruleExecutionStatus == ExecutionStatus.TransientError;
		}

		internal static void LogRuleEvaluationFailureEvent(TransportRulesEvaluationContext context, Exception exception, string tenantId, string messageId)
		{
			Type type = exception.GetType();
			if (TransportRulesErrorHandler.IsKnownFipsException(exception))
			{
				TransportRulesErrorHandler.LogRuleEvaluationFailureEvent(context, MessagingPoliciesEventLogConstants.Tuple_RuleEvaluationFilteringServiceFailure, type.Name, exception.ToString(), tenantId, messageId);
				return;
			}
			TransportRulesErrorHandler.LogRuleEvaluationFailureEvent(context, MessagingPoliciesEventLogConstants.Tuple_RuleEvaluationFailure, type.Name, exception.ToString(), tenantId, messageId);
		}

		internal static void LogRuleEvaluationIgnoredFailureEvent(TransportRulesEvaluationContext context, Exception exception, string tenantId, string messageId)
		{
			Type type = exception.GetType();
			if (TransportRulesErrorHandler.IsKnownFipsException(exception))
			{
				TransportRulesErrorHandler.LogRuleEvaluationFailureEvent(context, MessagingPoliciesEventLogConstants.Tuple_RuleEvaluationIgnoredFilteringServiceFailure, type.Name, exception.ToString(), tenantId, messageId);
				return;
			}
			TransportRulesErrorHandler.LogRuleEvaluationFailureEvent(context, MessagingPoliciesEventLogConstants.Tuple_RuleEvaluationIgnoredFailure, type.Name, exception.ToString(), tenantId, messageId);
		}

		internal static string EncodeStringToUtf7(string unicodeString)
		{
			return Encoding.ASCII.GetString(Encoding.UTF7.GetBytes(unicodeString));
		}

		internal static bool IsIgnorableException(Exception ex, TransportRulesEvaluationContext context)
		{
			if (context != null && context.CurrentRule != null && context.CurrentRule.ErrorAction == RuleErrorAction.Ignore && TransportRulesErrorHandler.knownRuleEvaluationExceptions.Any((Type knownExceptionType) => knownExceptionType == ex.GetType()))
			{
				return true;
			}
			IErrorHandlingAction errorHandlingAction = TransportRulesErrorHandler.GetErrorHandlingAction(ex, context.MailItem);
			return errorHandlingAction != null && errorHandlingAction.ActionType == ErrorHandlingActionType.Allow;
		}

		private static void LogRuleEvaluationFailureEvent(TransportRulesEvaluationContext context, ExEventLog.EventTuple eventLog, string exceptionName, string exceptionMessage, string tenantId, string messageId)
		{
			string errorMessage = string.Format("Organization: '{0}' Message ID '{1}' Rule ID '{2}' Predicate '{3}' Action '{4}'. {5} Error: {6}.", new object[]
			{
				tenantId,
				messageId,
				TransportUtils.GetCurrentRuleId(context),
				TransportUtils.GetCurrentPredicateName(context),
				TransportUtils.GetCurrentActionName(context),
				exceptionName,
				exceptionMessage
			});
			TransportRulesErrorHandler.LogFailureEvent(context.MailItem, eventLog, errorMessage);
		}

		private static readonly List<Type> filteringServiceExceptions = new List<Type>
		{
			typeof(FilteringServiceTimeoutException),
			typeof(FilteringServiceFailureException),
			typeof(ScannerCrashException),
			typeof(ScanQueueTimeoutException),
			typeof(ScanTimeoutException),
			typeof(ResultsValidationException),
			typeof(ClassificationEngineInvalidOobConfigurationException),
			typeof(ClassificationEngineInvalidCustomConfigurationException),
			typeof(BiasException),
			typeof(QueueFullException),
			typeof(ConfigurationException),
			typeof(ServiceUnavailableException),
			typeof(ScanAbortedException),
			typeof(FilteringException)
		};

		private static readonly List<Type> knownRuleEvaluationExceptions = TransportRulesErrorHandler.InitializeKnownRuleEvaluationExceptions();

		private static readonly List<Type> timeoutExceptions = new List<Type>
		{
			typeof(FilteringServiceTimeoutException),
			typeof(ScanQueueTimeoutException),
			typeof(ScanTimeoutException),
			typeof(RegexMatchTimeoutException)
		};

		private static readonly List<Type> exceptionsToNdr = new List<Type>
		{
			typeof(ParserException),
			typeof(DataSourceOperationException)
		};

		private static readonly int DeferCount = 5;

		private static readonly TimeSpan DeferInteval = TimeSpan.FromMinutes(10.0);

		private static readonly IErrorHandlingAction DeferActionProgressive = new AgentErrorHandlingDeferAction(TransportRulesErrorHandler.DeferInteval, true);

		private static readonly List<AgentErrorHandlingDefinition> TransportRulesAgentErrorHandlingMap = new List<AgentErrorHandlingDefinition>
		{
			new AgentErrorHandlingDefinition("Transport Rule - NDR when defer count reaches 5", new AgentErrorHandlingCondition(TrackAgentInfoAgentName.TRA.ToString("G"), TransportRulesErrorHandler.knownRuleEvaluationExceptions, TransportRulesErrorHandler.DeferCount, null), AgentErrorHandlingMap.NdrActionBadContent),
			new AgentErrorHandlingDefinition("Transport Rule - Defer", new AgentErrorHandlingCondition(TrackAgentInfoAgentName.TRA.ToString("G"), TransportRulesErrorHandler.knownRuleEvaluationExceptions, 0, null), TransportRulesErrorHandler.DeferActionProgressive),
			new AgentErrorHandlingDefinition("Transport Rule - NDR Immediately", new AgentErrorHandlingCondition(TrackAgentInfoAgentName.TRA.ToString("G"), TransportRulesErrorHandler.exceptionsToNdr, 0, null), AgentErrorHandlingMap.NdrActionBadContent)
		};

		private static readonly AgentErrorHandling AgentErrorHandler = new AgentErrorHandling(TransportRulesErrorHandler.TransportRulesAgentErrorHandlingMap);
	}
}

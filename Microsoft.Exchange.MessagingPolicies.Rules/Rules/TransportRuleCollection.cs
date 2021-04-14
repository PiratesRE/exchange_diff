using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Extensibility;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class TransportRuleCollection : RuleCollection
	{
		public TransportRuleCollection(string name) : base(name)
		{
			this.supportsBifurcation = false;
			this.ResetInternalState();
		}

		public bool SupportsBifurcation
		{
			get
			{
				return this.supportsBifurcation;
			}
			set
			{
				this.supportsBifurcation = value;
			}
		}

		public RulesCountersInstance TotalPerformanceCounterInstance
		{
			set
			{
				this.totalCounter = value;
			}
		}

		public ExecutionStatus Run(SmtpServer server, MailItem mailItem, ReceiveMessageEventSource endOfDataSource, SmtpSession session, TransportRulesCostMonitor ruleSetExecutionMonitor = null)
		{
			TransportRulesEvaluationContext context = new TransportRulesEvaluationContext(this, mailItem, server, endOfDataSource, session, ruleSetExecutionMonitor);
			return this.Run(context);
		}

		public ExecutionStatus Run(SmtpServer server, MailItem mailItem, QueuedMessageEventSource eventSource, bool shouldAudit = false, TransportRulesTracer tracer = null, TestMessageConfig testMessageConfig = null, TransportRulesCostMonitor ruleSetExecutionMonitor = null, TenantConfigurationCache<TransportRulesPerTenantSettings> transportRulesCache = null)
		{
			return this.Run(new TransportRulesEvaluationContext(this, mailItem, server, eventSource, ruleSetExecutionMonitor, shouldAudit, transportRulesCache, tracer)
			{
				TestMessageConfig = testMessageConfig
			});
		}

		public void CreatePerformanceCounters()
		{
			foreach (Rule rule in this)
			{
				TransportRule transportRule = (TransportRule)rule;
				if (transportRule.Enabled == RuleState.Enabled)
				{
					transportRule.CreatePerformanceCounter(base.Name);
				}
			}
		}

		internal ExecutionStatus Run(TransportRulesEvaluationContext context)
		{
			context.Tracer.TraceDebug("A rule collection is executing on a message.");
			ExecutionStatus result;
			try
			{
				TransportRulesEvaluator transportRulesEvaluator = new TransportRulesEvaluator(context);
				transportRulesEvaluator.Run();
				result = context.ExecutionStatus;
			}
			catch (Exception exception)
			{
				OrganizationId organizationID = TransportUtils.GetOrganizationID(context.MailItem);
				TransportRulesErrorHandler.LogRuleEvaluationFailureEvent(context, exception, (organizationID == null) ? "No org Id available" : organizationID.ToString(), TransportUtils.GetMessageID(context.MailItem));
				this.IncrementMessagesDeferredDueToErrors();
				IErrorHandlingAction errorHandlingAction = TransportRulesErrorHandler.GetErrorHandlingAction(exception, context.MailItem);
				if (errorHandlingAction == null)
				{
					throw;
				}
				if (errorHandlingAction is AgentErrorHandlingDeferAction && context.RuleSetExecutionMonitor != null)
				{
					context.RuleSetExecutionMonitor.StopAndSetReporter(null);
				}
				errorHandlingAction.TakeAction(context.OnResolvedSource, context.MailItem);
				result = TransportRulesErrorHandler.ErrorActionToExecutionStatus(errorHandlingAction.ActionType);
			}
			return result;
		}

		public void IncrementMessagesEvaluated()
		{
			if (!this.hasMessageEvaluatedCounted && this.totalCounter != null)
			{
				this.totalCounter.MessagesEvaluated.Increment();
				this.hasMessageEvaluatedCounted = true;
			}
		}

		public void IncrementMessagesProcessed()
		{
			if (!this.hasMessageProcessedCounted && this.totalCounter != null)
			{
				this.totalCounter.MessagesProcessed.Increment();
				this.hasMessageProcessedCounted = true;
			}
		}

		public void IncrementMessagesSkipped()
		{
			if (!this.hasMessageSkippedCounted && this.totalCounter != null)
			{
				this.totalCounter.MessagesSkipped.Increment();
				this.hasMessageSkippedCounted = true;
			}
		}

		public void IncrementMessagesDeferredDueToErrors()
		{
			if (!this.hasMessageDeferredDueToErrorsCounted && this.totalCounter != null)
			{
				this.totalCounter.MessagesDeferredDueToRuleErrors.Increment();
				this.hasMessageDeferredDueToErrorsCounted = true;
			}
		}

		public void ResetInternalState()
		{
			this.hasMessageEvaluatedCounted = false;
			this.hasMessageProcessedCounted = false;
			this.hasMessageSkippedCounted = false;
			this.hasMessageDeferredDueToErrorsCounted = false;
		}

		private bool supportsBifurcation;

		private RulesCountersInstance totalCounter;

		private bool hasMessageEvaluatedCounted;

		private bool hasMessageProcessedCounted;

		private bool hasMessageSkippedCounted;

		private bool hasMessageDeferredDueToErrorsCounted;
	}
}

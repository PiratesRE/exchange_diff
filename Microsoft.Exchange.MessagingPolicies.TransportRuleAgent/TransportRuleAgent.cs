using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Extensibility;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.TransportRuleAgent
{
	internal class TransportRuleAgent : RoutingAgent
	{
		private TransportRuleCollection RulesBeingProcessed { get; set; }

		public TransportRuleAgent(SmtpServer server, TransportRuleCollection rules, bool shouldDefer, RulesCountersInstance totalCounter, TenantConfigurationCache<TransportRulesPerTenantSettings> transportRulesCache)
		{
			base.OnResolvedMessage += this.OnResolvedMessageHandler;
			this.server = server;
			this.rules = rules;
			this.shouldDefer = shouldDefer;
			this.totalCounter = totalCounter;
			this.transportRulesCache = transportRulesCache;
			this.ruleLoadMonitor = TransportRulesEvaluationContext.CreateRuleHealthMonitor(Components.TransportAppConfig.TransportRuleConfig.TransportRuleLoadTimeReportingThresholdInMilliseconds, Components.TransportAppConfig.TransportRuleConfig.TransportRuleLoadTimeAlertingThresholdInMilliseconds);
		}

		public void OnResolvedMessageHandler(ResolvedMessageEventSource source, QueuedMessageEventArgs args)
		{
			TransportRulesCostMonitor transportRulesCostMonitor = new TransportRulesCostMonitor(TransportRulesAgentCostComponents.RuleLoad)
			{
				CostReporter = delegate(long componentCost, string additionalInfo)
				{
					ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<long, string>(0L, "ETR cost: {0}, ETR components: {1}", componentCost, additionalInfo);
					source.SetComponentCost("ETR", source.GetComponentCost("ETR") + componentCost);
				}
			};
			try
			{
				TestMessageConfig testMessageConfig = new TestMessageConfig(args.MailItem.Message);
				bool flag = testMessageConfig.IsTestMessage && (testMessageConfig.LogTypes & LogTypesEnum.TransportRules) != LogTypesEnum.None;
				TransportRulesTracer transportRulesTracer = new TransportRulesTracer(args.MailItem, flag);
				this.ProcessMessage(source, args, transportRulesCostMonitor, transportRulesTracer, testMessageConfig);
				if (flag && testMessageConfig.ReportToAddress.IsValidAddress)
				{
					transportRulesTracer.TraceDebug("\r\n\r\nRules processed:\r\n\r\n");
					TransportRuleSerializer transportRuleSerializer = new TransportRuleSerializer();
					foreach (Rule rule in this.RulesBeingProcessed)
					{
						transportRulesTracer.TraceDebug(transportRuleSerializer.SaveRuleToString(rule));
					}
					GenerateNotification.GenerateMessage(args.MailItem, this.server.Name, new EmailRecipient("Microsoft Outlook", "<>"), new List<string>
					{
						testMessageConfig.ReportToAddress.ToString()
					}, "Exchange Transport Rules Tracing Report: " + args.MailItem.Message.Subject, GenerateNotification.PlainTextToHtml("Transport Rules Tracing Report\r\n\r\n" + transportRulesTracer.ToString(), Encoding.Unicode));
				}
				this.RulesBeingProcessed = null;
			}
			finally
			{
				transportRulesCostMonitor.Stop();
			}
		}

		private static void CheckRecipients(MailItem mailItem, out List<EnvelopeRecipient> expandedRecipients)
		{
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<RoutingAddress>(0L, "Checking sender: {0}", mailItem.FromAddress);
			expandedRecipients = null;
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				object obj;
				bool flag = envelopeRecipient.Properties.TryGetValue("Microsoft.Exchange.Transport.DirectoryData.RecipientType", out obj);
				bool flag2 = obj is Microsoft.Exchange.Data.Directory.Recipient.RecipientType;
				if (!flag || !flag2)
				{
					ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<RoutingAddress>(0L, "{0}: RecipientType does not exist", envelopeRecipient.Address);
					if (expandedRecipients == null)
					{
						expandedRecipients = new List<EnvelopeRecipient>();
					}
					expandedRecipients.Add(envelopeRecipient);
				}
				else
				{
					Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)obj;
					ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<RoutingAddress, Microsoft.Exchange.Data.Directory.Recipient.RecipientType>(0L, "{0}: RecipientType {1}", envelopeRecipient.Address, recipientType);
					if (recipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Group && recipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup && recipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup && recipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup && recipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup)
					{
						if (expandedRecipients == null)
						{
							expandedRecipients = new List<EnvelopeRecipient>();
						}
						expandedRecipients.Add(envelopeRecipient);
					}
				}
			}
		}

		private static bool ShouldExecuteOn(MailItem mailItem, OrganizationId orgId, TransportRulesTracer etrTracer)
		{
			if (orgId == OrganizationId.ForestWideOrgId && !VariantConfiguration.InvariantNoFlightingSnapshot.CompliancePolicy.ProcessForestWideOrgEtrs.Enabled)
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Safetenant in a Datacenter, skipping rule execution");
				return false;
			}
			if (DatacenterRegistry.IsForefrontForOffice())
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Running in FFO Datacenter");
				return true;
			}
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "OriginalAuthenticator {0}, System {1}, IPM {2}, Opaque {3}, Security type {4}", new object[]
			{
				mailItem.OriginalAuthenticator,
				mailItem.Message.IsSystemMessage,
				mailItem.Message.IsInterpersonalMessage,
				mailItem.Message.IsOpaqueMessage,
				mailItem.Message.MessageSecurityType
			});
			if (mailItem.OriginalAuthenticator != null && mailItem.OriginalAuthenticator.Equals("<>", StringComparison.Ordinal))
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Anonymous message. Should apply rules.");
				return true;
			}
			if (TransportRuleAgent.IsSentOnlyToArbitrationMailbox(mailItem))
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Arbitration message. Should not apply rules.");
				return false;
			}
			EmailMessage message = mailItem.Message;
			if (ObjectClass.IsSmsMessage(message.MapiMessageClass))
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "SMS message. Should not apply rules.");
				return false;
			}
			if (message.MapiMessageClass.Equals("IPM.Note.Microsoft.Approval.Request.Recall", StringComparison.OrdinalIgnoreCase))
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Recall message. Should not apply rules.");
				return false;
			}
			if (((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.IsJournalReport())
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Journal report. Should not apply rules.");
				return false;
			}
			if (TransportRuleAgent.IsJournalNdrWithSkipRulesStamped(mailItem))
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Journal message. Should not apply rules.");
				return false;
			}
			if (message.IsInterpersonalMessage || message.IsOpaqueMessage)
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "IPM message. Should apply rules.");
				return true;
			}
			if (TransportRuleAgent.IsSpecialMapiMessageClass(message.MapiMessageClass))
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Special MAPI message. Should apply rules.");
				return true;
			}
			if (!message.IsSystemMessage && message.MessageSecurityType == MessageSecurityType.ClearSigned)
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "Clear signed message. Should apply rules.");
				return true;
			}
			if (message.MapiMessageClass.StartsWith("IPM.Note.", StringComparison.OrdinalIgnoreCase))
			{
				TransportRulesEvaluator.Trace(etrTracer, mailItem, "IPM Note message. Should apply rules.");
				return true;
			}
			TransportRulesEvaluator.Trace(etrTracer, mailItem, "Unknown message type. Should not apply rules.");
			return false;
		}

		private static bool IsJournalNdrWithSkipRulesStamped(MailItem mailItem)
		{
			return null != mailItem.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-JournalNdr-Skip-TransportMailboxRules");
		}

		private static bool IsSpecialMapiMessageClass(string messageClass)
		{
			return messageClass.Equals("IPM.Note.Microsoft.Conversation.Voice", StringComparison.OrdinalIgnoreCase) || messageClass.Equals("IPM.Note.Microsoft.Fax.CA", StringComparison.OrdinalIgnoreCase) || messageClass.Equals("IPM.Note.Microsoft.Missed.Voice", StringComparison.OrdinalIgnoreCase) || messageClass.Equals("IPM.Note.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase) || messageClass.Equals("IPM.Note.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) || messageClass.EndsWith(".Microsoft.Voicemail", StringComparison.OrdinalIgnoreCase) || (messageClass.StartsWith("Report.IPM.Note.", StringComparison.OrdinalIgnoreCase) && (messageClass.EndsWith("IPNRN", StringComparison.OrdinalIgnoreCase) || messageClass.EndsWith("IPNNRN", StringComparison.OrdinalIgnoreCase)));
		}

		private static bool IsSentOnlyToArbitrationMailbox(MailItem mailItem)
		{
			EmailMessage message = mailItem.Message;
			if (message.To.Count != 1 || message.Cc.Count != 0 || message.Bcc.Count != 0)
			{
				return false;
			}
			if (string.IsNullOrEmpty(message.To[0].SmtpAddress) || !SmtpAddress.IsValidSmtpAddress(message.To[0].SmtpAddress))
			{
				return false;
			}
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = (ADRecipientCache<TransportMiniRecipient>)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
			return adrecipientCache != null && ApprovalInitiation.IsArbitrationMailbox(adrecipientCache, new RoutingAddress(message.To[0].SmtpAddress));
		}

		private bool IsVersionRequirementSatisfied(OrganizationId orgId, MailItem mailItem)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return true;
			}
			TransportMailItem transportMailItem = TransportUtils.GetTransportMailItem(mailItem);
			if (transportMailItem != null && transportMailItem.TransportSettings.TransportRuleMinProductVersion > Rule.HighestHonoredVersion)
			{
				if (this.totalCounter != null)
				{
					this.totalCounter.MessagesSkipped.Increment();
				}
				return false;
			}
			return true;
		}

		private void ProcessMessage(QueuedMessageEventSource source, QueuedMessageEventArgs args, TransportRulesCostMonitor transportRulesCostMonitor, TransportRulesTracer etrTracer, TestMessageConfig testMessageConfig)
		{
			TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "ETR processing started");
			if (this.shouldDefer)
			{
				TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "TransportRulesAgent is deferring all mail per the TransportRulesAgentFactory");
				transportRulesCostMonitor.StopAndSetReporter(null);
				source.Defer(TransportRuleAgent.retryInterval);
				return;
			}
			if (args.MailItem.Recipients.Count == 0)
			{
				TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Message has no recipients");
				return;
			}
			OrganizationId organizationID = TransportUtils.GetOrganizationID(args.MailItem);
			if (!TransportRuleAgent.ShouldExecuteOn(args.MailItem, organizationID, etrTracer))
			{
				return;
			}
			if (!this.IsVersionRequirementSatisfied(organizationID, args.MailItem))
			{
				return;
			}
			this.RulesBeingProcessed = this.rules;
			bool flag = TransportUtils.RuleCollectionExecuted(args.MailItem.Message);
			if (flag)
			{
				if (this.RulesBeingProcessed != null)
				{
					this.RulesBeingProcessed.TotalPerformanceCounterInstance = this.totalCounter;
					this.RulesBeingProcessed.IncrementMessagesSkipped();
				}
				else if (this.totalCounter != null)
				{
					this.totalCounter.MessagesSkipped.Increment();
				}
				TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Rules have already been executed. Skipping the message.");
				return;
			}
			if (organizationID != OrganizationId.ForestWideOrgId)
			{
				TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Loading tenant rules");
				TransportRulesPerTenantSettings transportRulesPerTenantSettings = null;
				Exception ex = null;
				try
				{
					transportRulesPerTenantSettings = this.transportRulesCache.GetValue(organizationID, this.ruleLoadMonitor);
				}
				catch (Exception ex2)
				{
					if (!(ex2 is TransientException) && !(ex2 is DataSourceOperationException) && !(ex2 is DataValidationException))
					{
						throw;
					}
					ex = ex2;
				}
				if (ex != null || transportRulesPerTenantSettings == null || transportRulesPerTenantSettings.RuleCollection == null)
				{
					string text = TransportRulesStrings.FailedToLoadRuleCollection("TransportVersioned");
					string text2;
					if (ex != null)
					{
						text2 = string.Format("Tenant: '{0}', Error: '{1}'. {2} Exception: {3}", new object[]
						{
							organizationID,
							text,
							ex.GetType().Name,
							ex.ToString()
						});
					}
					else
					{
						text2 = string.Format("Tenant: '{0}', Error: '{1}'", organizationID, text);
					}
					TransportRulesEvaluator.LogFailureEvent(args.MailItem, MessagingPoliciesEventLogConstants.Tuple_RuleCollectionLoadingError, text2);
					this.totalCounter.MessagesDeferredDueToRuleErrors.Increment();
					TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Error loading rules: " + text2);
					IErrorHandlingAction errorHandlingAction = TransportRulesErrorHandler.GetErrorHandlingAction(new TransportRuleTransientException(text2), args.MailItem);
					if (errorHandlingAction != null)
					{
						if (errorHandlingAction is AgentErrorHandlingDeferAction)
						{
							transportRulesCostMonitor.StopAndSetReporter(null);
						}
						errorHandlingAction.TakeAction(source, args.MailItem);
					}
					return;
				}
				if (transportRulesPerTenantSettings.RuleCollection.Count == 0)
				{
					TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Tenant has no active rules");
					return;
				}
				this.RulesBeingProcessed = (TransportRuleCollection)transportRulesPerTenantSettings.RuleCollection;
				if (this.RulesBeingProcessed.CountAllNotDisabled > 0)
				{
					this.RulesBeingProcessed.SupportsBifurcation = true;
				}
				else
				{
					TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Tenant has no rules");
					this.RulesBeingProcessed = null;
				}
			}
			transportRulesCostMonitor.Start(TransportRulesAgentCostComponents.PreExecution);
			if (this.RulesBeingProcessed == null)
			{
				return;
			}
			this.RulesBeingProcessed.TotalPerformanceCounterInstance = this.totalCounter;
			List<EnvelopeRecipient> list;
			TransportRuleAgent.CheckRecipients(args.MailItem, out list);
			if (list == null)
			{
				TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Message contains no recipients, message deferred for recipient expansion");
				return;
			}
			if (list.Count < args.MailItem.Recipients.Count)
			{
				TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Message contains unexpanded recipients. Forking unexpanded recipients for resolution.");
				if (!TransportRulesLoopChecker.Fork(source, args.MailItem, list))
				{
					return;
				}
			}
			this.RulesBeingProcessed.ResetInternalState();
			transportRulesCostMonitor.Start(TransportRulesAgentCostComponents.RuleExecution);
			ExecutionStatus ruleExecutionStatus = this.RulesBeingProcessed.Run(this.server, args.MailItem, source, true, etrTracer, testMessageConfig, transportRulesCostMonitor, (organizationID == OrganizationId.ForestWideOrgId) ? null : this.transportRulesCache);
			TransportRulesEvaluator.Trace(etrTracer, args.MailItem, "Rule evaluation completed");
			if (TransportRulesErrorHandler.IsDeferredOrDeleted(ruleExecutionStatus))
			{
				return;
			}
			TransportUtils.AddRuleCollectionStamp(args.MailItem.Message, this.server.Name);
			TransportRulesLoopChecker.ForkAddedRecipients(source, args.MailItem);
		}

		private const string NullReversePathString = "<>";

		private const string IpmNotePrefix = "IPM.Note.";

		private static readonly TimeSpan retryInterval = new TimeSpan(0, 10, 0);

		private readonly RuleHealthMonitor ruleLoadMonitor;

		private readonly TransportRuleCollection rules;

		private readonly SmtpServer server;

		private readonly bool shouldDefer;

		private readonly RulesCountersInstance totalCounter;

		private TenantConfigurationCache<TransportRulesPerTenantSettings> transportRulesCache;
	}
}

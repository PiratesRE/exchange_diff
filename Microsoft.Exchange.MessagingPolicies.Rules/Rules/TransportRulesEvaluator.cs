using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.TransportRuleAgent;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class TransportRulesEvaluator : RulesEvaluator
	{
		public TransportRulesEvaluator(TransportRulesEvaluationContext context) : base(context)
		{
			this.context = context;
		}

		internal static void AddRuleToExecutionHistory(TransportRulesEvaluationContext context)
		{
			Header header = context.MailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Rules-Execution-History");
			string text = TransportRulesErrorHandler.EncodeStringToUtf7(TransportRulesEvaluator.GetFullyQualifiedRuleName(context.RuleName, context.Rules.Name));
			if (header == null)
			{
				TransportUtils.AddHeaderToMail(context.MailItem.Message, "X-MS-Exchange-Organization-Rules-Execution-History", text);
				return;
			}
			header.Value = header.Value + "%%%" + text;
		}

		internal static string GetFullyQualifiedRuleName(string ruleName, string ruleCollectionName)
		{
			return string.Format("{0}.{1}", ruleCollectionName, ruleName);
		}

		internal static void Trace(TransportRulesTracer etrTracer, MailItem mailItem, string traceMessage)
		{
			Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, traceMessage);
			SystemProbeHelper.EtrTracer.TracePass(mailItem, 0L, traceMessage);
			etrTracer.TraceDebug(traceMessage);
		}

		internal static void LogFailureEvent(MailItem mailItem, ExEventLog.EventTuple eventLog, string errorMessage)
		{
			string text = string.Format("{0} Message-Id:{1}", errorMessage, TransportUtils.GetMessageID(mailItem));
			Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, errorMessage);
			TransportAction.Logger.LogEvent(eventLog, null, new object[]
			{
				text
			});
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, string.Format("{0}.Event-0x{1:X}", TransportRulesEvaluator.ActiveMonitoringComponentName, eventLog.EventId), null, text, ResultSeverityLevel.Error, false);
			SystemProbeHelper.EtrTracer.TraceFail(mailItem, 0L, "Error processing rules. Details: {0}", text);
		}

		internal static IEnumerable<string> GetRuleExecutionHistory(MailItem mailItem)
		{
			Header header = mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Rules-Execution-History");
			if (header == null)
			{
				return new List<string>();
			}
			return header.Value.Split(new string[]
			{
				"%%%"
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		internal static void SetAddedByTransportRuleProperty(EnvelopeRecipient recipient, TransportRulesEvaluationContext context)
		{
			recipient.Properties["Microsoft.Exchange.Transport.AddedByTransportRule"] = context.RuleName;
		}

		internal ExecutionControl ExitRuleImpl()
		{
			TransportRule transportRule = (TransportRule)this.context.CurrentRule;
			TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("RuleExit: Exiting rule '{0}'", transportRule.Name));
			MailItem mailItem = this.context.MailItem;
			TransportRuleCollection transportRuleCollection = (TransportRuleCollection)this.context.Rules;
			if (!transportRuleCollection.SupportsBifurcation)
			{
				if (this.context.MessageQuarantined)
				{
					this.LogToAgentLog(this.context, TransportRulesEvaluator.QuarantineResponse, "Message quarantined by edge rule");
				}
				if (this.context.EdgeRejectResponse != null)
				{
					SmtpResponse value = this.context.EdgeRejectResponse.Value;
					if (value.SmtpResponseType != SmtpResponseType.Success && value.SmtpResponseType != SmtpResponseType.IntermediateSuccess)
					{
						if (value.SmtpResponseType == SmtpResponseType.PermanentError)
						{
							if (this.context.RecipientsToAdd.Count == 0)
							{
								TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: Rejecting message");
								this.LogToAgentLog(this.context, value, "Message rejected by edge rule");
								this.context.EndOfDataSource.RejectMessage(value);
								return ExecutionControl.SkipAll;
							}
							TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: NDR all recipients");
							List<EnvelopeRecipient> list = new List<EnvelopeRecipient>(this.context.MailItem.Recipients.Count);
							foreach (EnvelopeRecipient item in this.context.MailItem.Recipients)
							{
								list.Add(item);
							}
							using (List<EnvelopeRecipient>.Enumerator enumerator2 = list.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									EnvelopeRecipient recipient = enumerator2.Current;
									this.context.MailItem.Recipients.Remove(recipient, DsnType.Failure, value);
								}
								goto IL_2C3;
							}
						}
						TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: Temp-fail the message");
						this.LogToAgentLog(this.context, value, "Message rejected by edge rule");
						this.context.EndOfDataSource.RejectMessage(value);
						return ExecutionControl.SkipAll;
					}
					if (this.context.RecipientsToAdd.Count == 0)
					{
						TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: Silently deleting message");
						this.LogToAgentLog(this.context, value, "Message rejected by edge rule");
						this.context.EndOfDataSource.RejectMessage(value);
						return ExecutionControl.SkipAll;
					}
					TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: Delete all recipients");
					mailItem.Recipients.Clear();
				}
				IL_2C3:
				this.AppendRuleAddedRecipients();
				this.context.EdgeRejectResponse = null;
				this.context.MessageQuarantined = false;
				return ExecutionControl.Execute;
			}
			this.AuditRuleActions();
			this.AppendRuleAddedRecipients();
			TransportRulesEvaluator.AddRuleToExecutionHistory(this.context);
			if (mailItem.Recipients.Count == 0)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: Halting the  the message as there is no recipient on the message");
				return ExecutionControl.SkipAll;
			}
			if (this.context.RecipientState == RecipientState.Deferred)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: Defer the message");
				return ExecutionControl.SkipAll;
			}
			TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleExit: Continue to the next rule");
			return ExecutionControl.Execute;
		}

		internal List<KeyValuePair<string, string>> GetRuleData(Rule rule)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			TransportRulesEvaluationContext.AddRuleData(list, "ruleId", rule.ImmutableId.ToString("D"));
			TransportRulesEvaluationContext.AddRuleData(list, "st", (rule.WhenChangedUTC ?? DateTime.MinValue).ToString());
			foreach (Action action in this.context.ExecutedActions)
			{
				TransportRulesEvaluationContext.AddRuleData(list, "action", action.ExternalName);
			}
			TransportRulesEvaluationContext.AddRuleData(list, "sev", ((int)this.context.CurrentAuditSeverityLevel).ToString());
			TransportRulesEvaluationContext.AddRuleData(list, "mode", rule.Mode.ToString("G"));
			Guid guid;
			if (((TransportRule)rule).TryGetDlpPolicyId(out guid))
			{
				TransportRulesEvaluationContext.AddRuleData(list, "dlpId", guid.ToString("D"));
			}
			foreach (string value in this.context.MatchedClassifications.Keys)
			{
				TransportRulesEvaluationContext.AddRuleData(list, "dcId", value);
			}
			if (this.context.SenderOverridden)
			{
				TransportRulesEvaluationContext.AddRuleData(list, "sndOverride", "or");
			}
			if (this.context.FpOverriden)
			{
				TransportRulesEvaluationContext.AddRuleData(list, "sndOverride", "fp");
			}
			return list;
		}

		protected override ExecutionControl EnterRule()
		{
			TransportRule transportRule = (TransportRule)this.context.CurrentRule;
			TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Entering transport rule '{0}'", transportRule.Name));
			if (this.context.RuleExecutionMonitor != null)
			{
				if (this.context.EventSource != null)
				{
					this.context.RuleExecutionMonitor.MtlLogWriter = new RuleHealthMonitor.MtlLogWriterDelegate(this.context.EventSource.TrackAgentInfo);
				}
				this.context.RuleExecutionMonitor.RuleId = transportRule.ImmutableId.ToString("D");
				this.context.RuleExecutionMonitor.TenantId = TransportUtils.GetOrganizationID(this.context.MailItem).ToString();
				this.context.RuleExecutionMonitor.Restart();
			}
			this.context.MatchedRecipients = null;
			this.context.ResetPerRuleData();
			this.context.RuleName = transportRule.Name;
			TransportRuleCollection transportRuleCollection = (TransportRuleCollection)this.context.Rules;
			if (!transportRuleCollection.SupportsBifurcation)
			{
				transportRule.IncrementMessagesEvaluated();
				transportRuleCollection.IncrementMessagesEvaluated();
				return ExecutionControl.Execute;
			}
			if (TransportRulesEvaluator.RuleExistsInHistory(transportRule, this.context.MailItem, this.context.Rules.Name))
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Skipping rule '{0}' because it existed in rule history", transportRule.Name));
				return ExecutionControl.SkipThis;
			}
			if (this.context.RecipientState == RecipientState.HasUnresolved && transportRule.Fork != null && transportRule.Fork.Count > 0)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "RuleEnter: Defering the message");
				this.context.Defer(TimeSpan.Zero);
				this.context.RecipientState = RecipientState.Deferred;
				return ExecutionControl.SkipAll;
			}
			transportRule.IncrementMessagesEvaluated();
			transportRuleCollection.IncrementMessagesEvaluated();
			if (transportRule.Fork != null && transportRule.Fork.Count > 0)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "Checking whether bifurcation is needed on this message");
				bool flag = false;
				foreach (Action action in transportRule.Actions)
				{
					if (string.Equals(action.Name, "ModerateMessageByUser", StringComparison.OrdinalIgnoreCase) || string.Equals(action.Name, "ModerateMessageByManager", StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				MailItem mailItem = this.context.MailItem;
				List<EnvelopeRecipient> list = null;
				foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
				{
					string text = envelopeRecipient.Address.ToString();
					if (this.RecipientMatchesForkInfo(transportRule, mailItem.FromAddress.ToString(), text, this.context.Server))
					{
						if (list == null)
						{
							list = new List<EnvelopeRecipient>();
						}
						if (flag)
						{
							IADRecipientCache iadrecipientCache = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(this.context.MailItem).ADRecipientCacheAsObject;
							ProxyAddress proxyAddress = new SmtpProxyAddress(envelopeRecipient.Address.ToString(), true);
							ADRawEntry data = iadrecipientCache.FindAndCacheRecipient(proxyAddress).Data;
							if (data != null)
							{
								RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)data[ADRecipientSchema.RecipientTypeDetailsValue];
								if (recipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox)
								{
									TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Recipient {0} not added to matched recipients because it was an arbitration mailbox", proxyAddress));
									continue;
								}
							}
						}
						list.Add(envelopeRecipient);
						TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Recipient {0} matched rule bifurcation criteria", text));
					}
					else
					{
						TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Recipient {0} did not match rule bifurcation criteria", text));
					}
				}
				if (list == null)
				{
					TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Skipping rule '{0}' because no recipient matched bifurcation criteria", transportRule.Name));
					this.ExitRule();
					return ExecutionControl.SkipThis;
				}
				if (mailItem.Recipients.Count == list.Count)
				{
					TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "All recipients on the message matched bifurcation criteria. Fork is not needed.");
				}
				else
				{
					TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "Bifurcating this message.");
					if (transportRule.MostSpecificActionType != TransportActionType.NonRecipientRelated)
					{
						this.context.MatchedRecipients = list;
					}
				}
			}
			return ExecutionControl.Execute;
		}

		protected override bool EvaluateCondition(Condition condition, RulesEvaluationContext evaluationContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)evaluationContext;
			TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Evaluating condition {0}", condition));
			bool result;
			try
			{
				Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.TransportRuleAgent.ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3775278397U, this.context.MailItem.Message.Subject);
				result = base.EvaluateCondition(condition, evaluationContext);
			}
			catch (Exception ex)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Condition evaluation failed with error(s) {0}", ex));
				if (!TransportRulesErrorHandler.IsIgnorableException(ex, this.context))
				{
					ExWatson.AddExtraData(TransportUtils.GetExtraWatsonData(transportRulesEvaluationContext, TransportUtils.GetCurrentPredicateName(transportRulesEvaluationContext)));
					throw;
				}
				Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.TransportRuleAgent.ExTraceGlobals.FaultInjectionTracer.TraceTest(44064U);
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "Agent is set to ignore errors, condition evaluated to False");
				TransportRulesErrorHandler.LogRuleEvaluationIgnoredFailureEvent(transportRulesEvaluationContext, ex, TransportUtils.GetOrganizationID(this.context.MailItem).ToString(), TransportUtils.GetMessageID(this.context.MailItem));
				result = false;
			}
			return result;
		}

		protected override ExecutionControl ExecuteAction(Action action, RulesEvaluationContext evaluationContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)evaluationContext;
			transportRulesEvaluationContext.ExecutedActions.Add(action);
			transportRulesEvaluationContext.ActionName = action.ExternalName;
			TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("Executing action {0}", action.ExternalName));
			ExecutionControl result;
			try
			{
				result = base.ExecuteAction(action, this.context);
			}
			catch (Exception ex)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("Action execution failed with error(s) {0}", ex.Message));
				if (!TransportRulesErrorHandler.IsIgnorableException(ex, this.context))
				{
					ExWatson.AddExtraData(TransportUtils.GetExtraWatsonData(transportRulesEvaluationContext, TransportUtils.GetCurrentActionName(transportRulesEvaluationContext)));
					throw;
				}
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Agent is set to ignore errors, action execution skipped", new object[0]));
				TransportRulesErrorHandler.LogRuleEvaluationIgnoredFailureEvent(transportRulesEvaluationContext, ex, TransportUtils.GetOrganizationID(this.context.MailItem).ToString(), TransportUtils.GetMessageID(this.context.MailItem));
				result = ExecutionControl.Execute;
			}
			return result;
		}

		protected override void AuditAction(Action action, RulesEvaluationContext context)
		{
			((TransportRulesEvaluationContext)context).ExecutedActions.Add(action);
			if (action is AuditSeverityLevelAction)
			{
				action.Execute(context);
				return;
			}
			base.AuditAction(action, context);
		}

		protected override ExecutionControl ExitRule()
		{
			ExecutionControl result = this.ExitRuleImpl();
			if (this.context.RuleExecutionMonitor != null)
			{
				this.context.RuleExecutionMonitor.Stop(!TransportRulesErrorHandler.IsDeferredOrDeleted(this.context.ExecutionStatus));
			}
			return result;
		}

		protected override bool EnterRuleActionBlock()
		{
			bool result = true;
			TransportRuleCollection transportRuleCollection = (TransportRuleCollection)this.context.Rules;
			TransportRule transportRule = (TransportRule)this.context.CurrentRule;
			TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("Rule '{0}' matched. Executing actions... ", transportRule.Name));
			transportRuleCollection.IncrementMessagesProcessed();
			transportRule.IncrementMessagesProcessed();
			if (this.context.MatchedRecipients != null && this.context.MatchedRecipients.Count > 0 && transportRule.MostSpecificActionType == TransportActionType.BifurcationNeeded)
			{
				if (this.context.EventSource != null)
				{
					result = TransportRulesLoopChecker.Fork(this.context.OnResolvedSource, this.context.MailItem, this.context.MatchedRecipients);
				}
				this.context.MatchedRecipients = null;
			}
			return result;
		}

		protected override ExecutionControl EnterRuleCollection()
		{
			this.AuditClientInformation();
			return base.EnterRuleCollection();
		}

		protected override void ExitRuleCollection()
		{
			ExecutionStatus executionStatus = ExecutionStatus.Success;
			if (this.context.RecipientState == RecipientState.Deferred || this.context.MailItem.Recipients.Count == 0)
			{
				executionStatus = ExecutionStatus.SuccessMailItemDeferred;
			}
			if (((TransportRuleCollection)this.context.Rules).SupportsBifurcation && this.context.RecipientState == RecipientState.HasUnresolved)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "Deferring the message before completing all rules");
				if (this.context.EventSource != null)
				{
					this.context.Defer(TimeSpan.Zero);
					executionStatus = ExecutionStatus.SuccessMailItemDeferred;
				}
			}
			if (this.context.SuppressDelivery)
			{
				TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, "Test message configured to supress delivery. Deleting the message.");
				DeleteMessage.Delete(this.context, new SmtpResponse("550", "5.2.1", new string[]
				{
					"Test  message configured to supress delivery. Message deleted."
				}));
				executionStatus = ExecutionStatus.SuccessMailItemDeleted;
			}
			this.context.ExecutionStatus = executionStatus;
		}

		private static bool RuleExistsInHistory(Rule rule, MailItem mailItem, string ruleCollectionName)
		{
			string encodedRuleName = TransportRulesErrorHandler.EncodeStringToUtf7(TransportRulesEvaluator.GetFullyQualifiedRuleName(rule.Name, ruleCollectionName));
			return TransportRulesEvaluator.GetRuleExecutionHistory(mailItem).Any((string ruleName) => ruleName.Equals(encodedRuleName));
		}

		private bool RecipientMatchesForkInfo(TransportRule rule, string sender, string recipient, SmtpServer server)
		{
			bool flag = true;
			OrganizationId orgId = (OrganizationId)TransportUtils.GetTransportMailItemFacade(this.context.MailItem).OrganizationIdAsObject;
			for (int i = 0; i < rule.Fork.Count; i++)
			{
				if (!rule.Fork[i].Exception)
				{
					RuleBifurcationInfo ruleBifurcationInfo = rule.Fork[i];
					RuleBifurcationInfo ruleBifurcationInfo2 = (rule.Fork.Count <= i + 1) ? null : rule.Fork[i + 1];
					if (this.CanConvertToBetweewnMemberOfCheck(ruleBifurcationInfo, ruleBifurcationInfo2))
					{
						if (!this.MatchesSingleBifurcationInfo(sender, recipient, ruleBifurcationInfo, server, orgId) && !this.MatchesSingleBifurcationInfo(sender, recipient, ruleBifurcationInfo2, server, orgId))
						{
							flag = false;
						}
						i++;
					}
					else if (!this.MatchesSingleBifurcationInfo(sender, recipient, ruleBifurcationInfo, server, orgId))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < rule.Fork.Count; j++)
				{
					if (rule.Fork[j].Exception)
					{
						RuleBifurcationInfo ruleBifurcationInfo3 = rule.Fork[j];
						RuleBifurcationInfo ruleBifurcationInfo4 = (rule.Fork.Count <= j + 1) ? null : rule.Fork[j + 1];
						if (this.CanConvertToBetweewnMemberOfCheck(ruleBifurcationInfo3, ruleBifurcationInfo4))
						{
							if (this.MatchesSingleBifurcationInfo(sender, recipient, ruleBifurcationInfo3, server, orgId) || this.MatchesSingleBifurcationInfo(sender, recipient, ruleBifurcationInfo4, server, orgId))
							{
								flag = false;
							}
							j++;
						}
						else if (this.MatchesSingleBifurcationInfo(sender, recipient, ruleBifurcationInfo3, server, orgId))
						{
							flag = false;
						}
					}
				}
			}
			return flag;
		}

		internal bool MatchesSingleBifurcationInfo(string sender, string recipient, RuleBifurcationInfo bifInfo, SmtpServer server, OrganizationId orgId)
		{
			bool flag = false;
			if (bifInfo.FromRecipients.Count != 0 || bifInfo.FromLists.Count != 0)
			{
				flag = true;
				foreach (string x in bifInfo.FromRecipients)
				{
					if (this.context.UserComparer.Equals(x, sender))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					foreach (string y in bifInfo.FromLists)
					{
						if (this.context.MembershipChecker.Equals(sender, y))
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag)
			{
				return false;
			}
			foreach (string x2 in bifInfo.Recipients)
			{
				if (this.context.UserComparer.Equals(x2, recipient))
				{
					return true;
				}
			}
			foreach (string y2 in bifInfo.Lists)
			{
				if (this.context.MembershipChecker.Equals(recipient, y2))
				{
					return true;
				}
			}
			if (bifInfo.RecipientAddressContainsWords.Any<string>())
			{
				string text = recipient.ToLowerInvariant();
				return TransportUtils.IsMatchTplKeyword(text, "recipientContainsWords" + text, bifInfo.RecipientAddressContainsWords, this.context);
			}
			foreach (string domain in bifInfo.RecipientDomainIs)
			{
				if (string.IsNullOrEmpty(recipient) || !SmtpAddress.IsValidSmtpAddress(recipient))
				{
					return false;
				}
				string domain2 = new SmtpAddress(recipient).Domain;
				if (!string.IsNullOrEmpty(domain2) && DomainIsPredicate.IsSubdomainOf(domain, domain2))
				{
					return true;
				}
			}
			if (bifInfo.RecipientMatchesPatterns.Any<string>())
			{
				string text2 = recipient.ToLowerInvariant();
				return TransportUtils.IsMatchTplRegex(text2, "recipientMatchesPatterns" + text2, bifInfo.RecipientMatchesPatterns, this.context, true);
			}
			if (bifInfo.RecipientMatchesRegexPatterns.Any<string>())
			{
				string str = recipient.ToLowerInvariant();
				return TransportUtils.IsMatchTplRegex(recipient, "recipientMatchesRegexPatterns" + str, bifInfo.RecipientMatchesRegexPatterns, this.context, false);
			}
			foreach (string x3 in bifInfo.Managers)
			{
				if (bifInfo.IsSenderEvaluation)
				{
					if (this.context.UserComparer.Equals(x3, TransportUtils.GetManagerAddress(this.context, sender)))
					{
						return true;
					}
				}
				else if (this.context.UserComparer.Equals(x3, TransportUtils.GetManagerAddress(this.context, recipient)))
				{
					return true;
				}
			}
			if (bifInfo.SenderInRecipientList.Any<string>())
			{
				if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(recipient) || !SmtpAddress.IsValidSmtpAddress(sender) || !SmtpAddress.IsValidSmtpAddress(recipient))
				{
					return false;
				}
				IADRecipientCache iadrecipientCache = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(this.context.MailItem).ADRecipientCacheAsObject;
				ProxyAddress proxyAddress = new SmtpProxyAddress(sender, true);
				ProxyAddress proxyAddress2 = new SmtpProxyAddress(recipient, true);
				ADRawEntry data = iadrecipientCache.FindAndCacheRecipient(proxyAddress).Data;
				ADRawEntry data2 = iadrecipientCache.FindAndCacheRecipient(proxyAddress2).Data;
				ADObjectId userADObjectId = null;
				if (data != null)
				{
					userADObjectId = (ADObjectId)data[ADObjectSchema.Id];
				}
				if (data2 != null)
				{
					SupervisionMaps supervisionMaps = new SupervisionMaps(data2, bifInfo.SenderInRecipientList);
					foreach (string tag in bifInfo.SenderInRecipientList)
					{
						if (TransportUtils.UserExistsInSupervisionMaps(tag, supervisionMaps, userADObjectId, sender, iadrecipientCache, this.context))
						{
							return true;
						}
					}
				}
			}
			if (bifInfo.RecipientInSenderList.Any<string>())
			{
				if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(recipient) || !SmtpAddress.IsValidSmtpAddress(sender) || !SmtpAddress.IsValidSmtpAddress(recipient))
				{
					return false;
				}
				IADRecipientCache iadrecipientCache2 = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(this.context.MailItem).ADRecipientCacheAsObject;
				ProxyAddress proxyAddress3 = new SmtpProxyAddress(sender, true);
				ProxyAddress proxyAddress4 = new SmtpProxyAddress(recipient, true);
				ADRawEntry data3 = iadrecipientCache2.FindAndCacheRecipient(proxyAddress3).Data;
				ADRawEntry data4 = iadrecipientCache2.FindAndCacheRecipient(proxyAddress4).Data;
				ADObjectId userADObjectId2 = null;
				if (data4 != null)
				{
					userADObjectId2 = (ADObjectId)data4[ADObjectSchema.Id];
				}
				if (data3 != null)
				{
					SupervisionMaps supervisionMaps2 = new SupervisionMaps(data3, bifInfo.RecipientInSenderList);
					foreach (string tag2 in bifInfo.RecipientInSenderList)
					{
						if (TransportUtils.UserExistsInSupervisionMaps(tag2, supervisionMaps2, userADObjectId2, recipient, iadrecipientCache2, this.context))
						{
							return true;
						}
					}
				}
			}
			using (List<string>.Enumerator enumerator9 = bifInfo.ADAttributes.GetEnumerator())
			{
				if (enumerator9.MoveNext())
				{
					string field = enumerator9.Current;
					if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(recipient) || !SmtpAddress.IsValidSmtpAddress(sender) || !SmtpAddress.IsValidSmtpAddress(recipient))
					{
						return false;
					}
					IADRecipientCache iadrecipientCache3 = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(this.context.MailItem).ADRecipientCacheAsObject;
					SmtpProxyAddress smtpProxyAddress = new SmtpProxyAddress(sender, true);
					SmtpProxyAddress smtpProxyAddress2 = new SmtpProxyAddress(recipient, true);
					ADRawEntry data5 = iadrecipientCache3.FindAndCacheRecipient(smtpProxyAddress).Data;
					ADRawEntry data6 = iadrecipientCache3.FindAndCacheRecipient(smtpProxyAddress2).Data;
					if (data5 == null || data6 == null)
					{
						return false;
					}
					string macroPropertyDefinition = TransportUtils.GetMacroPropertyDefinition(smtpProxyAddress, field, data5);
					string macroPropertyDefinition2 = TransportUtils.GetMacroPropertyDefinition(smtpProxyAddress2, field, data6);
					bool flag2 = this.AreSomeAttributeValuesAreEqual(macroPropertyDefinition, macroPropertyDefinition2);
					return bifInfo.CheckADAttributeEquality ? flag2 : (!flag2);
				}
			}
			using (List<string>.Enumerator enumerator10 = bifInfo.ADAttributesForTextMatch.GetEnumerator())
			{
				if (enumerator10.MoveNext())
				{
					string field2 = enumerator10.Current;
					IADRecipientCache iadrecipientCache4 = (IADRecipientCache)TransportUtils.GetTransportMailItemFacade(this.context.MailItem).ADRecipientCacheAsObject;
					SmtpProxyAddress smtpProxyAddress3;
					if (bifInfo.IsSenderEvaluation)
					{
						if (string.IsNullOrEmpty(sender) || !SmtpAddress.IsValidSmtpAddress(sender))
						{
							return false;
						}
						smtpProxyAddress3 = new SmtpProxyAddress(sender, true);
					}
					else
					{
						if (string.IsNullOrEmpty(recipient) || !SmtpAddress.IsValidSmtpAddress(recipient))
						{
							return false;
						}
						smtpProxyAddress3 = new SmtpProxyAddress(recipient, true);
					}
					ADRawEntry data7 = iadrecipientCache4.FindAndCacheRecipient(smtpProxyAddress3).Data;
					if (data7 == null)
					{
						return false;
					}
					string macroPropertyDefinition3 = TransportUtils.GetMacroPropertyDefinition(smtpProxyAddress3, field2, data7);
					if (bifInfo.CheckADAttributeEquality)
					{
						return string.Equals(macroPropertyDefinition3.Trim(), bifInfo.ADAttributeValue.Trim(), StringComparison.InvariantCultureIgnoreCase);
					}
					return macroPropertyDefinition3.ToLowerInvariant().Contains(bifInfo.ADAttributeValue.ToLowerInvariant().Trim());
				}
			}
			if (bifInfo.RecipientAttributeContains.Any<string>())
			{
				return TransportUtils.UserAttributeContainsWords(this.context, recipient, bifInfo.RecipientAttributeContains.ToArray(), "recipientAttributeContains");
			}
			if (bifInfo.RecipientAttributeMatches.Any<string>())
			{
				return TransportUtils.UserAttributeMatchesPatterns(this.context, recipient, bifInfo.RecipientAttributeMatches.ToArray(), "recipientAttributeMatchesRegex");
			}
			if (bifInfo.RecipientAttributeMatchesRegex.Any<string>())
			{
				return TransportUtils.UserAttributeMatchesPatterns(this.context, recipient, bifInfo.RecipientAttributeMatchesRegex.ToArray(), "recipientAttributeMatches");
			}
			if (string.Equals(bifInfo.ManagementRelationship, "Manager", StringComparison.InvariantCultureIgnoreCase) && this.context.UserComparer.Equals(TransportUtils.GetManagerAddress(this.context, recipient), sender))
			{
				return true;
			}
			if (string.Equals(bifInfo.ManagementRelationship, "DirectReport", StringComparison.InvariantCultureIgnoreCase) && this.context.UserComparer.Equals(TransportUtils.GetManagerAddress(this.context, sender), recipient))
			{
				return true;
			}
			bool flag3 = false;
			if (bifInfo.InternalRecipients)
			{
				if (IsInternalPredicate.IsInternal(server, recipient, orgId))
				{
					return true;
				}
				flag3 = true;
			}
			if (bifInfo.ExternalRecipients && (flag3 || !IsInternalPredicate.IsInternal(server, recipient, orgId)))
			{
				return true;
			}
			bool flag4 = false;
			if (bifInfo.ExternalPartnerRecipients)
			{
				if (IsExternalPartnerPredicate.IsExternalPartner(recipient))
				{
					return true;
				}
				flag4 = true;
			}
			return bifInfo.ExternalNonPartnerRecipients && (flag3 || !IsInternalPredicate.IsInternal(server, recipient, orgId)) && (flag4 || !IsExternalPartnerPredicate.IsExternalPartner(recipient));
		}

		private bool AreSomeAttributeValuesAreEqual(string input1, string input2)
		{
			bool flag = input1.Contains(";");
			bool flag2 = input2.Contains(";");
			if (!flag && !flag2)
			{
				return string.Equals(input1.Trim(), input2.Trim(), StringComparison.InvariantCultureIgnoreCase);
			}
			if (flag && flag2)
			{
				string[] array = input1.Split(TransportRulesEvaluator.ADAttributeDelimiterArray, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = input2.Split(TransportRulesEvaluator.ADAttributeDelimiterArray, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array2.Length; j++)
					{
						if (string.Equals(array[i].Trim(), array2[j].Trim(), StringComparison.InvariantCultureIgnoreCase))
						{
							return true;
						}
					}
				}
				return false;
			}
			string a;
			string[] array3;
			if (!flag)
			{
				a = input1.Trim();
				array3 = input2.Split(TransportRulesEvaluator.ADAttributeDelimiterArray, StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				a = input2.Trim();
				array3 = input1.Split(TransportRulesEvaluator.ADAttributeDelimiterArray, StringSplitOptions.RemoveEmptyEntries);
			}
			for (int k = 0; k < array3.Length; k++)
			{
				if (string.Equals(a, array3[k].Trim(), StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private void LogToAgentLog(TransportRulesEvaluationContext context, SmtpResponse response, string logEntryValue)
		{
			if (context.Session != null)
			{
				context.TheAgentLog.LogRejectMessage("Edge Rule", "OnEndOfData", null, context.Session, context.MailItem, response, new LogEntry(logEntryValue, context.CurrentRule.Name));
			}
		}

		private bool CanConvertToBetweewnMemberOfCheck(RuleBifurcationInfo bifInfo1, RuleBifurcationInfo bifInfo2)
		{
			if (bifInfo1 == null || bifInfo2 == null)
			{
				return false;
			}
			if (bifInfo1.Exception != bifInfo2.Exception)
			{
				return false;
			}
			if (bifInfo1.Lists.Count != bifInfo2.FromLists.Count || bifInfo2.Lists.Count != bifInfo1.FromLists.Count || bifInfo1.Lists.Count == 0 || bifInfo1.FromLists.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < bifInfo1.Lists.Count; i++)
			{
				if (bifInfo1.Lists[i] != bifInfo2.FromLists[i])
				{
					return false;
				}
			}
			for (int j = 0; j < bifInfo2.Lists.Count; j++)
			{
				if (bifInfo2.Lists[j] != bifInfo1.FromLists[j])
				{
					return false;
				}
			}
			return true;
		}

		private void AuditClientInformation()
		{
			if (this.context.MailItem != null)
			{
				object obj;
				if (this.context.MailItem.Properties.TryGetValue("CIAudited", out obj))
				{
					return;
				}
				this.context.MailItem.Properties["CIAudited"] = true;
			}
			if (!this.context.ShouldAuditRules || TransportRulesErrorHandler.IsDeferredOrDeleted(this.context.ExecutionStatus))
			{
				return;
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			string value = null;
			if (SenderNotify.GetSenderOverrideHeader(this.context, out value))
			{
				TransportRulesEvaluationContext.AddRuleData(list, "sndOverride", "or");
				TransportRulesEvaluationContext.AddRuleData(list, "just", value);
			}
			if (SenderNotify.IsFpHeaderSet(this.context))
			{
				TransportRulesEvaluationContext.AddRuleData(list, "sndOverride", "fp");
			}
			if (list.Count<KeyValuePair<string, string>>() == 0)
			{
				return;
			}
			try
			{
				this.context.EventSource.TrackAgentInfo(TrackAgentInfoAgentName.TRA.ToString("G"), TrackAgentInfoGroupName.CI.ToString("G"), list);
			}
			catch (InvalidOperationException)
			{
				Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.ExTraceGlobals.TransportRulesEngineTracer.TraceWarning(0L, "InvalidOperationException thrown while attempting to audit client information. Expected when data size to Audit is high.");
			}
		}

		private void AuditRuleActions()
		{
			if (!this.context.ShouldAuditRules || !this.context.ExecutedActions.Any<Action>() || this.context.CurrentAuditSeverityLevel == AuditSeverityLevel.DoNotAudit || TransportRulesErrorHandler.IsDeferredOrDeleted(this.context.ExecutionStatus))
			{
				return;
			}
			Rule currentRule = this.context.CurrentRule;
			List<KeyValuePair<string, string>> ruleData = this.GetRuleData(currentRule);
			try
			{
				this.context.EventSource.TrackAgentInfo(TrackAgentInfoAgentName.TRA.ToString("G"), TrackAgentInfoGroupName.ETR.ToString("G"), ruleData);
			}
			catch (InvalidOperationException)
			{
				Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.ExTraceGlobals.TransportRulesEngineTracer.TraceWarning(0L, "InvalidOperationException thrown while attempting to audit rule hit information. Expected when data size to Audit is high.");
			}
		}

		private void AppendRuleAddedRecipients()
		{
			if (!this.context.RecipientsToAdd.Any<TransportRulesEvaluationContext.AddedRecipient>())
			{
				return;
			}
			IMailRecipientCollectionFacade recipients = TransportUtils.GetTransportMailItemFacade(this.context.MailItem).Recipients;
			foreach (TransportRulesEvaluationContext.AddedRecipient addedRecipient in this.context.RecipientsToAdd)
			{
				if (this.context.MailItem.Recipients.Contains((RoutingAddress)addedRecipient.Address))
				{
					TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("ExitRule: Skip adding recipient {0} to the message because it already exists", addedRecipient.Address));
				}
				else
				{
					TransportRulesEvaluator.Trace(this.context.TransportRulesTracer, this.context.MailItem, string.Format("ExitRule: Adding recipient {0} to the message", addedRecipient.Address));
					switch (addedRecipient.RecipientP2Type)
					{
					case RecipientP2Type.To:
						this.context.MailItem.Message.To.Add(new EmailRecipient(addedRecipient.DisplayName, addedRecipient.Address));
						recipients.Add(addedRecipient.Address);
						break;
					case RecipientP2Type.Cc:
						this.context.MailItem.Message.Cc.Add(new EmailRecipient(addedRecipient.DisplayName, addedRecipient.Address));
						recipients.Add(addedRecipient.Address);
						break;
					case RecipientP2Type.Bcc:
						recipients.AddWithoutDsnRequested(addedRecipient.Address);
						break;
					default:
						recipients.Add(addedRecipient.Address);
						break;
					}
					EnvelopeRecipient recipient = this.context.MailItem.Recipients.Last<EnvelopeRecipient>();
					TransportRulesEvaluator.SetAddedByTransportRuleProperty(recipient, this.context);
					if (this.context.Server != null && this.context.Server.AssociatedAgent != null)
					{
						TransportFacades.TrackRecipientAddByAgent(TransportUtils.GetTransportMailItemFacade(this.context.MailItem), addedRecipient.Address, addedRecipient.RecipientP2Type, this.context.Server.AssociatedAgent.Name);
					}
					this.context.RecipientState = RecipientState.HasUnresolved;
				}
			}
			this.context.RecipientsToAdd.Clear();
		}

		private const string QuarantineMessageString = "Message quarantined by edge rule";

		private const string ADAttributeDelimiterString = ";";

		private const string ExecutionHistoryHeaderSeparator = "%%%";

		private const string RejectMessageString = "Message rejected by edge rule";

		internal static readonly string ActiveMonitoringComponentName = "ExchangeTransportRules";

		private static readonly SmtpResponse QuarantineResponse = new SmtpResponse("550", "5.2.1", new string[]
		{
			"Quarantined by rule agent"
		});

		private static readonly string[] ADAttributeDelimiterArray = new string[]
		{
			";"
		};

		private readonly TransportRulesEvaluationContext context;
	}
}

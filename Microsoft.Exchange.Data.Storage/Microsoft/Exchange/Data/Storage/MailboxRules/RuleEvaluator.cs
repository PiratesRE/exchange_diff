using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RuleEvaluator
	{
		public RuleEvaluator(IRuleEvaluationContext context)
		{
			this.context = context;
		}

		public MailboxEvaluationResult Evaluate()
		{
			HashSet<StoreId> hashSet = new HashSet<StoreId>();
			MailboxEvaluationResult mailboxEvaluationResult = new MailboxEvaluationResult(this.context);
			for (;;)
			{
				Folder currentFolder = this.context.CurrentFolder;
				this.context.TraceDebug<string>("Evaluating Rules in Folder {0}", this.context.CurrentFolderDisplayName);
				this.context.AddCurrentFolderIdTo(hashSet);
				FolderEvaluationResult folderEvaluationResult = this.EvaluateRulesOnCurrentFolder();
				if (folderEvaluationResult == null)
				{
					break;
				}
				mailboxEvaluationResult.AddFolderResult(folderEvaluationResult);
				if (mailboxEvaluationResult.TargetFolder == null)
				{
					goto Block_2;
				}
				if (currentFolder.Id.Equals(mailboxEvaluationResult.TargetFolder.Id))
				{
					goto Block_3;
				}
				if (hashSet.Contains(mailboxEvaluationResult.TargetFolder.Id))
				{
					goto Block_4;
				}
				this.context.CurrentFolder = mailboxEvaluationResult.TargetFolder;
			}
			this.context.TraceError("Stop rules processing because folder result is empty");
			return mailboxEvaluationResult;
			Block_2:
			this.context.TraceDebug("Stop rules processing because the message is deleted");
			return mailboxEvaluationResult;
			Block_3:
			this.context.TraceDebug("Target folder is not changed after evaluating rules on a folder. Complete rules processing");
			return mailboxEvaluationResult;
			Block_4:
			this.context.TraceDebug<string>("Loop detected on folder {0}. Stop rules processing", mailboxEvaluationResult.TargetFolder.DisplayName);
			return mailboxEvaluationResult;
		}

		private void CreateWorkItemForAction(FolderEvaluationResult result, RuleAction action, int actionIndex)
		{
			if (action == null)
			{
				this.context.TraceError("Action is empty");
				return;
			}
			this.context.TraceDebug<Type>("Create work item for action {0}", action.GetType());
			if (action is RuleAction.InMailboxMove)
			{
				this.CreateWorkItemForInMailboxMove(result, (RuleAction.InMailboxMove)action, actionIndex);
				return;
			}
			if (action is RuleAction.ExternalMove)
			{
				this.CreateWorkItemForExternalMove(result, (RuleAction.ExternalMove)action, actionIndex);
				return;
			}
			if (action is RuleAction.InMailboxCopy)
			{
				this.CreateWorkItemForInMailboxCopy(result, (RuleAction.InMailboxCopy)action, actionIndex);
				return;
			}
			if (action is RuleAction.ExternalCopy)
			{
				this.CreateWorkItemForExternalCopy(result, (RuleAction.ExternalCopy)action, actionIndex);
				return;
			}
			if (action is RuleAction.Bounce)
			{
				this.CreateWorkItemForBounce(result, (RuleAction.Bounce)action);
				return;
			}
			if (action is RuleAction.Reply)
			{
				this.CreateWorkItemForReply(result, (RuleAction.Reply)action, actionIndex);
				return;
			}
			if (action is RuleAction.OOFReply)
			{
				this.CreateWorkItemForOofReply(result, (RuleAction.OOFReply)action, actionIndex);
				return;
			}
			if (action is RuleAction.Defer)
			{
				this.CreateWorkItemForDefer(result, (RuleAction.Defer)action, actionIndex);
				return;
			}
			if (action is RuleAction.Forward)
			{
				this.CreateWorkItemForForward(result, (RuleAction.Forward)action, actionIndex);
				return;
			}
			if (action is RuleAction.Delegate)
			{
				this.CreateWorkItemForDelegate(result, (RuleAction.Delegate)action, actionIndex);
				return;
			}
			if (action is RuleAction.Tag)
			{
				this.CreateWorkItemForTag(result, (RuleAction.Tag)action, actionIndex);
				return;
			}
			if (action is RuleAction.Delete)
			{
				this.CreateWorkItemForDelete(result, (RuleAction.Delete)action, actionIndex);
				return;
			}
			if (action is RuleAction.MarkAsRead)
			{
				this.CreateWorkItemForMarkAsRead(result, (RuleAction.MarkAsRead)action, actionIndex);
				return;
			}
			this.context.TraceError<RuleAction.Type>("Unsupported action type {0}", action.ActionType);
			this.context.MarkRuleInError(this.context.CurrentRule, action.ActionType, actionIndex, DeferredError.RuleError.Unknown);
		}

		private void CreateWorkItemForBounce(FolderEvaluationResult result, RuleAction.Bounce action)
		{
			if (ObjectClass.IsDsn(this.context.Message.ClassName))
			{
				this.context.TraceDebug("DSN message can not be bounced");
				return;
			}
			result.ClearWorkItems();
			result.TargetFolder = null;
			result.ExitExecution = true;
			result.BounceCode = new RuleAction.Bounce.BounceCode?(action.Code);
			this.context.TraceDebug<RuleAction.Bounce.BounceCode?>("Message will be bounced with code {0}", result.BounceCode);
		}

		private void CreateWorkItemForInMailboxCopy(FolderEvaluationResult result, RuleAction.InMailboxCopy action, int actionIndex)
		{
			Folder folder;
			if (!this.context.TryOpenLocalStore(action.FolderEntryID, out folder))
			{
				this.context.TraceError<byte[]>("Could not bind to local folder {0}", action.FolderEntryID);
				this.context.MarkRuleInError(this.context.CurrentRule, action.ActionType, actionIndex, DeferredError.RuleError.NoFolder);
				return;
			}
			this.context.TraceDebug<string>("Copy to local folder {0}", folder.DisplayName);
			if (!result.HasCopyWorkItemTo(folder.Id))
			{
				result.AddWorkItem(new CopyWorkItem(this.context, folder, actionIndex));
				return;
			}
			this.context.TraceDebug("Duplicate copy to a folder will be ignored");
		}

		private void CreateWorkItemForExternalCopy(FolderEvaluationResult result, RuleAction.ExternalCopy action, int actionIndex)
		{
			if (this.context.CurrentRule.IsExtended)
			{
				this.context.TraceError("Extended Rule is configured to copy to an out-of-mailbox folder. Marking the rule in error");
				this.context.MarkRuleInError(this.context.CurrentRule, action.ActionType, actionIndex, DeferredError.RuleError.Execution);
			}
			this.context.TraceDebug("Copy to an external store");
			this.CreateWorkItemForDefer(result, action, actionIndex);
		}

		private DeferredActionWorkItem CreateWorkItemForDefer(FolderEvaluationResult result, RuleAction action, int actionIndex)
		{
			string provider = this.context.CurrentRule.Provider;
			DeferredActionWorkItem deferredActionWorkItem = result.GetDeferredActionWorkItem(provider);
			if (deferredActionWorkItem == null)
			{
				this.context.TraceDebug<string>("Create Deferred Action Message for provider {0}", provider);
				deferredActionWorkItem = new DeferredActionWorkItem(this.context, this.context.CurrentRule.Provider, actionIndex);
				result.AddWorkItem(deferredActionWorkItem);
			}
			deferredActionWorkItem.AddAction(action);
			return deferredActionWorkItem;
		}

		private void CreateWorkItemForDelegate(FolderEvaluationResult result, RuleAction.Delegate action, int actionIndex)
		{
			if (RuleUtil.IsNullOrEmpty(action.Recipients))
			{
				this.context.TraceError("Delegate recipient list is empty");
				return;
			}
			result.AddWorkItem(new DelegateWorkItem(this.context, action.Recipients, actionIndex));
		}

		private void CreateWorkItemForDelete(FolderEvaluationResult result, RuleAction.Delete action, int actionIndex)
		{
			result.ExitExecution = true;
			if (result.HasDeferredMoveOrCopy)
			{
				this.context.TraceDebug("Softly delete the message since it is moved/copied to external store");
				this.CreateWorkItemForDefer(result, action, actionIndex);
				result.TargetFolder = this.context.GetDeletedItemsFolder();
				return;
			}
			this.context.TraceDebug("Delete the message");
			result.TargetFolder = null;
			if (result.IsMessageDelegated)
			{
				this.context.TraceDebug("Message was delegated. No need to generate NRN.");
				return;
			}
			if (!this.context.Message.IsReadReceiptRequested)
			{
				this.context.TraceDebug("Read Receipt is not requested on the message.");
				return;
			}
			if (null == this.context.Message.Sender && null == this.context.Message.From && null == this.context.Message.ReadReceiptAddressee)
			{
				this.context.TraceDebug("No NRN recipient data, generation skipped.");
				return;
			}
			if (this.context.IsInternetMdnDisabled)
			{
				this.context.TraceDebug("MDNs are disabled on the mailbox, message deleted without NRN to sender.");
				return;
			}
			result.AddWorkItem(new DeleteWorkItem(this.context, actionIndex));
		}

		private void CreateWorkItemForForward(FolderEvaluationResult result, RuleAction.Forward action, int actionIndex)
		{
			if (ObjectClass.IsDsn(this.context.Message.ClassName) || ObjectClass.IsMdn(this.context.Message.ClassName))
			{
				this.context.TraceDebug("Do not forward for Dsn or Mdn messages");
				return;
			}
			if (this.context.Message.Sensitivity == Sensitivity.Private)
			{
				this.context.TraceDebug("Do not forward private message");
				return;
			}
			object obj = this.context.Message.TryGetProperty(MessageItemSchema.RecipientReassignmentProhibited);
			if (obj is bool && (bool)obj)
			{
				this.context.TraceDebug("Do not forward message is recipient reassignment is prohibited");
				return;
			}
			if ((this.context.Message.AutoResponseSuppress & AutoResponseSuppress.AutoReply) != AutoResponseSuppress.None && RuleLoader.IsOofRule(this.context.CurrentRule))
			{
				this.context.TraceDebug("Do not forward message is auto reply is suppressed");
				return;
			}
			if (RuleUtil.IsNullOrEmpty(action.Recipients))
			{
				this.context.TraceError("Forward recipient list is empty");
				return;
			}
			if (!this.context.LimitChecker.CheckAndIncrementForwardeeCount(action.Recipients.Length))
			{
				this.context.TraceDebug(string.Concat(new object[]
				{
					"Rule ",
					this.context.CurrentRule.Name,
					" is forwarding to ",
					action.Recipients.Length,
					" additional recipients, which reached the accumulated forwardee limit, evaluation skipped."
				}));
				return;
			}
			result.AddWorkItem(new ForwardWorkItem(this.context, action.Recipients, action.Flags, actionIndex));
		}

		private void CreateWorkItemForMarkAsRead(FolderEvaluationResult result, RuleAction.MarkAsRead action, int actionIndex)
		{
			result.AddWorkItem(new MarkAsReadWorkItem(this.context, actionIndex));
		}

		private void CreateWorkItemForInMailboxMove(FolderEvaluationResult result, RuleAction.InMailboxMove action, int actionIndex)
		{
			if (this.context.ShouldSkipMoveRule)
			{
				return;
			}
			if (result.IsMessageMoved)
			{
				this.context.TraceDebug("Message is moved by a previous rule/action. Treat move as copy");
				this.CreateWorkItemForInMailboxCopy(result, new RuleAction.InMailboxCopy(action.FolderEntryID), actionIndex);
				return;
			}
			Folder folder;
			if (this.context.TryOpenLocalStore(action.FolderEntryID, out folder))
			{
				this.context.TraceDebug<string>("Move to local folder {0}", folder.DisplayName);
				if (this.context.CurrentFolder.Id.Equals(folder.Id))
				{
					this.context.TraceDebug("Move to current folder will be ignored");
					result.IsMessageMoved = true;
					return;
				}
				result.TargetFolder = folder;
				result.IsMessageMoved = true;
				return;
			}
			else
			{
				if ("JunkEmailRule".Equals(this.context.CurrentRule.Provider, StringComparison.OrdinalIgnoreCase))
				{
					this.context.TraceDebug("Unable to open junk email folder, skipping the move.");
					return;
				}
				this.context.TraceError<byte[]>("Could not bind to local folder {0}", action.FolderEntryID);
				this.context.MarkRuleInError(this.context.CurrentRule, action.ActionType, actionIndex, DeferredError.RuleError.NoFolder);
				return;
			}
		}

		private void CreateWorkItemForExternalMove(FolderEvaluationResult result, RuleAction.ExternalMove action, int actionIndex)
		{
			if (this.context.CurrentRule.IsExtended)
			{
				this.context.TraceError("Extended Rule is configured to move to an out-of-mailbox folder. Marking the rule in error");
				this.context.MarkRuleInError(this.context.CurrentRule, action.ActionType, actionIndex, DeferredError.RuleError.Execution);
			}
			if (this.context.ShouldSkipMoveRule)
			{
				return;
			}
			if (result.IsMessageMoved)
			{
				this.context.TraceDebug("Message is moved by a previous rule/action. Treat move as copy");
				this.CreateWorkItemForExternalCopy(result, new RuleAction.ExternalCopy(action.StoreEntryID, action.FolderEntryID), actionIndex);
				return;
			}
			this.context.TraceDebug("Move to an external store");
			DeferredActionWorkItem deferredActionWorkItem = this.CreateWorkItemForDefer(result, action, actionIndex);
			deferredActionWorkItem.MoveToStoreEntryId = action.StoreEntryID;
			deferredActionWorkItem.MoveToFolderEntryId = action.FolderEntryID;
			result.IsMessageMoved = true;
		}

		private void CreateWorkItemForOofReply(FolderEvaluationResult result, RuleAction.OOFReply action, int actionIndex)
		{
			string className = this.context.Message.ClassName;
			if (ObjectClass.IsDsn(className) || ObjectClass.IsMdn(className))
			{
				this.context.TraceDebug("Do not generate OOF reply for Dsn or Mdn messages");
				return;
			}
			if ((this.context.Message.AutoResponseSuppress & AutoResponseSuppress.OOF) != AutoResponseSuppress.None)
			{
				this.context.TraceDebug("Do not generate OOF reply when Oof Reply is being suppressed");
				return;
			}
			if (ObjectClass.IsOfClass(className, "IPM.Note.Rules.ExternalOofTemplate.Microsoft") || ObjectClass.IsOfClass(className, "IPM.Note.Rules.OofTemplate.Microsoft"))
			{
				this.context.TraceDebug("Do not generate OOF reply for Oof Message");
				return;
			}
			if (ObjectClass.IsOutlookRecall(className) || ObjectClass.IsOfClass(className, "IPM.Recall.Report.Failure") || ObjectClass.IsOfClass(className, "IPM.Recall.Report.Success"))
			{
				this.context.TraceDebug("Do not generate OOF reply for outlook recall Message");
				return;
			}
			if (null == this.context.Message.Sender)
			{
				this.context.TraceDebug("Do not generate OOF reply for an unknown sender");
				return;
			}
			string routingType = this.context.Message.Sender.RoutingType;
			if (string.IsNullOrEmpty(routingType) || (!(routingType == "SMTP") && !(routingType == "EX") && !routingType.Equals("X400")))
			{
				this.context.TraceDebug<string>("Do not generate OOF reply for address type {0}", routingType ?? "<null>");
				return;
			}
			if (RuleUtil.IsNullOrEmpty(action.ReplyTemplateMessageEntryID))
			{
				this.context.TraceError("Replay template message id is empty");
				return;
			}
			bool? valueAsNullable = this.context.Message.GetValueAsNullable<bool>(ItemSchema.DelegatedByRule);
			if (valueAsNullable != null && valueAsNullable.Value)
			{
				this.context.TraceDebug("Do not generate OOF reply for messages delegated by rule.");
				return;
			}
			result.AddWorkItem(new OofReplyWorkItem(this.context, action.ReplyTemplateMessageEntryID, actionIndex));
		}

		private void CreateWorkItemForReply(FolderEvaluationResult result, RuleAction.Reply action, int actionIndex)
		{
			if (RuleUtil.IsSameUser(this.context, this.context.RecipientCache, this.context.Sender, this.context.Recipient))
			{
				this.context.TraceDebug("Do not generate reply to self.");
				return;
			}
			if (ObjectClass.IsDsn(this.context.Message.ClassName) || ObjectClass.IsMdn(this.context.Message.ClassName))
			{
				this.context.TraceDebug("Do not generate reply for Dsn or Mdn messages");
				return;
			}
			if ((this.context.Message.AutoResponseSuppress & AutoResponseSuppress.AutoReply) != AutoResponseSuppress.None)
			{
				this.context.TraceDebug("Do not generate reply since Auto-Reply is being suppressed on current message");
				return;
			}
			if (ObjectClass.IsApprovalMessage(this.context.Message.ClassName))
			{
				this.context.TraceDebug("Do not generate reply to approval message.");
				return;
			}
			if (this.context.LimitChecker.DoesExceedAutoReplyLimit())
			{
				return;
			}
			result.AddWorkItem(new ReplyWorkItem(this.context, action.ReplyTemplateMessageEntryID, action.ReplyTemplateGuid, action.Flags, actionIndex));
		}

		private void CreateWorkItemForTag(FolderEvaluationResult result, RuleAction.Tag action, int actionIndex)
		{
			result.AddWorkItem(new TagWorkItem(this.context, action.Value, actionIndex));
		}

		private bool EvaluateRule(Rule rule, FolderEvaluationResult result)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			if (rule.Actions == null)
			{
				throw new ArgumentException("Rule.Actions must not be null.");
			}
			this.context.CurrentRule = rule;
			this.context.NestedLevel = 0;
			this.context.TraceDebug<string>("Evaluate rule {0}", rule.Name);
			int i = 0;
			bool result2;
			try
			{
				RuleUtil.FaultInjection((FaultInjectionLid)3102092605U);
				if (!RestrictionEvaluator.Evaluate(rule.Condition, this.context))
				{
					result2 = false;
				}
				else
				{
					this.context.TraceDebug<string>("Rule {0} evaluated to true", rule.Name);
					if (this.context.HasRuleFiredBefore(rule))
					{
						this.context.TraceDebug<string>("Rule history loop detected on rule {0}", rule.Name);
						result2 = false;
					}
					else
					{
						while (i < rule.Actions.Length)
						{
							RuleAction ruleAction = rule.Actions[i];
							if (ruleAction == null)
							{
								this.context.TraceDebug<string, int>("For rule \"{0}\", action at index {1} is null.", rule.Name, i);
							}
							else
							{
								this.CreateWorkItemForAction(result, ruleAction, i++);
								if (!result.ShouldContinue)
								{
									this.context.TraceDebug<Type>("Stop evaluation process after action {0}", ruleAction.GetType());
									break;
								}
							}
						}
						result2 = true;
					}
				}
			}
			catch (InvalidRuleException exception)
			{
				this.context.DisableAndMarkRuleInError(rule, rule.Actions[i].ActionType, i, DeferredError.RuleError.Parsing);
				this.context.RecordError(exception, ServerStrings.FolderRuleStageEvaluation);
				result2 = false;
			}
			return result2;
		}

		private FolderEvaluationResult EvaluateRulesOnCurrentFolder()
		{
			FolderEvaluationResult folderEvaluationResult = null;
			string stage = ServerStrings.FolderRuleStageLoading;
			try
			{
				RuleUtil.FaultInjection((FaultInjectionLid)3353750845U);
				List<Rule> list = this.context.LoadRules();
				if (list == null)
				{
					return null;
				}
				stage = ServerStrings.FolderRuleStageEvaluation;
				folderEvaluationResult = new FolderEvaluationResult(this.context, list);
				if (this.context.SharedPropertiesBetweenAgents == null)
				{
					this.context.SharedPropertiesBetweenAgents = new Dictionary<PropertyDefinition, object>();
				}
				foreach (Rule rule in folderEvaluationResult.Rules)
				{
					this.context.CurrentRule = rule;
					if (this.EvaluateRule(rule, folderEvaluationResult))
					{
						if ((rule.StateFlags & RuleStateFlags.ExitAfterExecution) != (RuleStateFlags)0)
						{
							this.context.TraceDebug<string>("Encountered exit level rule {0} ", rule.Name);
							folderEvaluationResult.ExitExecution = true;
							this.context.SharedPropertiesBetweenAgents[ItemSchema.IsStopProcessingRuleApplicable] = true;
						}
						if (RuleLoader.IsJunkEmailRule(rule))
						{
							this.context.TraceDebug<string>("Processed junk email rule {0} ", rule.Name);
							this.context.SharedPropertiesBetweenAgents[SharedProperties.ItemMovedByJunkMailRule] = folderEvaluationResult.IsMessageMoved;
							break;
						}
						if (RuleLoader.IsNeverClutterOverrideRule(rule))
						{
							this.context.SharedPropertiesBetweenAgents[ItemSchema.InferenceNeverClutterOverrideApplied] = true;
						}
						this.context.SharedPropertiesBetweenAgents[ItemSchema.ItemMovedByConversationAction] = this.context.ShouldSkipMoveRule;
						this.context.SharedPropertiesBetweenAgents[ItemSchema.ItemMovedByRule] = folderEvaluationResult.IsMessageMoved;
						if (!folderEvaluationResult.ShouldContinue)
						{
							this.context.TraceDebug("Terminate rules processing on current folder");
							break;
						}
					}
				}
				this.context.CurrentRule = null;
			}
			catch (StoragePermanentException exception)
			{
				this.context.RecordError(exception, stage);
			}
			catch (ExchangeDataException exception2)
			{
				this.context.RecordError(exception2, stage);
			}
			catch (MapiPermanentException exception3)
			{
				this.context.RecordError(exception3, stage);
			}
			catch (DataValidationException exception4)
			{
				this.context.RecordError(exception4, stage);
			}
			finally
			{
				this.context.CurrentRule = null;
			}
			return folderEvaluationResult;
		}

		public const string ExternalOofTemplate = "IPM.Note.Rules.ExternalOofTemplate.Microsoft";

		public const string InternalOofTemplate = "IPM.Note.Rules.OofTemplate.Microsoft";

		public const string RecallFailureReport = "IPM.Recall.Report.Failure";

		public const string RecallSuccessReport = "IPM.Recall.Report.Success";

		private const string JunkEmailRuleProvider = "JunkEmailRule";

		public const string X400 = "X400";

		private IRuleEvaluationContext context;
	}
}

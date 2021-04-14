using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplyWorkItem : WorkItem
	{
		public ReplyWorkItem(IRuleEvaluationContext context, byte[] messageTemplateEntryId, Guid replyTemplateGuid, RuleAction.Reply.ActionFlags flags, int actionIndex) : base(context, actionIndex)
		{
			this.messageTemplateEntryId = messageTemplateEntryId;
			this.replyTemplateGuid = replyTemplateGuid;
			this.flags = flags;
		}

		public override bool ShouldExecuteOnThisStage
		{
			get
			{
				return base.ShouldExecuteOnThisStage || ExecutionStage.OnPublicFolderAfter == base.Context.ExecutionStage;
			}
		}

		public override ExecutionStage Stage
		{
			get
			{
				return ExecutionStage.OnPromotedMessage | ExecutionStage.OnDeliveredMessage | ExecutionStage.OnPublicFolderAfter;
			}
		}

		public override void Execute()
		{
			if (!this.ShouldExecuteOnThisStage)
			{
				return;
			}
			if (base.Context.DetectLoop())
			{
				return;
			}
			bool isPublicFolderSession = base.Context.StoreSession.IsPublicFolderSession;
			if (isPublicFolderSession)
			{
				MessageItem messageItem = null;
				try
				{
					messageItem = base.OpenMessage(this.messageTemplateEntryId);
				}
				catch (ObjectNotFoundException innerException)
				{
					if (!(this.replyTemplateGuid != Guid.Empty))
					{
						string message = string.Format("Unable to find the reply template for the rule {0} on the folder {1}. The ReplyTemplateGuid is missing.", base.Context.CurrentRule.Name, base.Context.CurrentFolderDisplayName);
						base.Context.TraceDebug(message);
						throw new InvalidRuleException(message, innerException);
					}
					if (!this.TryFixReplyTemplateId())
					{
						string message = string.Format("Failed to find the reply template for the rule {0} on the folder {1}. ReplyTemplateGuid value: {2}", base.Context.CurrentRule.Name, base.Context.CurrentFolderDisplayName, this.replyTemplateGuid);
						base.Context.TraceDebug(message);
						throw new InvalidRuleException(message, new ObjectNotFoundException(ServerStrings.NoTemplateMessage));
					}
					messageItem = base.OpenMessage(this.messageTemplateEntryId);
				}
				using (messageItem)
				{
					if (this.replyTemplateGuid == Guid.Empty)
					{
						this.StampReplyTemplateGuid(messageItem);
					}
					this.InternalCreateReplyAndSubmit(messageItem);
					return;
				}
			}
			using (MessageItem messageItem3 = base.OpenMessage(this.messageTemplateEntryId))
			{
				this.InternalCreateReplyAndSubmit(messageItem3);
			}
		}

		private void InternalCreateReplyAndSubmit(MessageItem template)
		{
			if (template.Recipients != null && !base.Context.LimitChecker.CheckAndIncrementForwardeeCount(template.Recipients.Count))
			{
				base.Context.TraceDebug<string>("Skipping reply rule {0} due to forwardee limit exceeded.", base.Context.CurrentRule.Name);
				return;
			}
			using (MessageItem messageItem = this.CreateReply(template))
			{
				base.Context.SetMailboxOwnerAsSender(messageItem);
				this.CopyRecipientsFromTemplate(template, messageItem);
				base.SubmitMessage(messageItem);
			}
		}

		private bool TryFixReplyTemplateId()
		{
			base.Context.TraceDebug<string, string>("Fixing ReplyTemplateId for Rule {0} in Folder {1}", base.Context.CurrentRule.Name, base.Context.CurrentFolderDisplayName);
			bool flag = false;
			using (QueryResult queryResult = base.Context.CurrentFolder.ItemQuery(ItemQueryType.Associated, ReplyWorkItem.TemplateMessageQueryFilter, null, ReplyWorkItem.ReplyTemplateRelatedProperties))
			{
				Guid g = Guid.Empty;
				bool flag2 = false;
				while (!flag2 && !flag)
				{
					object[][] rows = queryResult.GetRows(10000);
					flag2 = (rows.Length == 0);
					foreach (object[] array2 in rows)
					{
						byte[] array3 = array2[1] as byte[];
						if (array3 != null)
						{
							g = new Guid(array3);
							if (this.replyTemplateGuid.Equals(g))
							{
								VersionedId versionedId = array2[0] as VersionedId;
								if (versionedId != null)
								{
									flag = true;
									this.messageTemplateEntryId = versionedId.ObjectId.ProviderLevelItemId;
									this.UpdateRuleAction(versionedId.ObjectId);
									break;
								}
								break;
							}
						}
					}
				}
			}
			return flag;
		}

		private void StampReplyTemplateGuid(MessageItem template)
		{
			this.replyTemplateGuid = Guid.NewGuid();
			template.OpenAsReadWrite();
			template.SafeSetProperty(ItemSchema.ReplyTemplateId, this.replyTemplateGuid.ToByteArray());
			ConflictResolutionResult conflictResolutionResult = template.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult != ConflictResolutionResult.Success)
			{
				base.Context.TraceDebug<string, string>("Failed to stamp the Guid on the reply template message for the rule {0} on the folder {1}. Skipping updating the rule action with the Guid.", base.Context.CurrentRule.Name, base.Context.CurrentFolderDisplayName);
			}
			else
			{
				base.Context.TraceDebug("Successfully stamped the Guid on the template message, now updating the rule action with the guid.");
				this.UpdateRuleAction(RuleActionConverter.GetReplyTemplateStoreObjectId(this.messageTemplateEntryId));
			}
			template.Load(StoreObjectSchema.ContentConversionProperties);
		}

		private void UpdateRuleAction(StoreObjectId replytemplateMessageId)
		{
			using (IModifyTable ruleTable = base.Context.CurrentFolder.GetRuleTable(null))
			{
				RuleAction ruleAction = base.Context.CurrentRule.Actions[base.ActionIndex];
				RuleAction.Reply reply = (RuleAction.Reply)ruleAction;
				RuleAction[] value = new RuleAction[]
				{
					new RuleAction.ReplyAction(reply.UserFlags, RuleActionConverter.MapiReplyFlagsToReplyFlags(reply.Flags), replytemplateMessageId, this.replyTemplateGuid)
				};
				PropValue[] propValues = new PropValue[]
				{
					new PropValue(InternalSchema.RuleId, base.Context.CurrentRule.ID),
					new PropValue(InternalSchema.RuleActions, value)
				};
				ruleTable.ModifyRow(propValues);
				ruleTable.ApplyPendingChanges();
			}
			base.Context.TraceDebug("Successfully updated the rule action with TemplateMessageId and/or Guid");
		}

		private MessageItem CreateReply(MessageItem template)
		{
			MessageItem messageItem;
			if ((this.flags & RuleAction.Reply.ActionFlags.UseStockReplyTemplate) != RuleAction.Reply.ActionFlags.None)
			{
				base.Context.TraceDebug("Reply action: creating stock reply.");
				CultureInfo preferedCulture = base.Context.StoreSession.PreferedCulture;
				string body = ServerStrings.StockReplyTemplate.ToString(preferedCulture);
				messageItem = RuleMessageUtils.CreateStockReply(base.Context.Message, body, preferedCulture, base.Context.XLoopValue);
			}
			else
			{
				base.Context.TraceDebug("Reply action: creating reply.");
				messageItem = RuleMessageUtils.CreateReply(base.Context.Message, template, base.Context.StoreSession.PreferedCulture, base.Context.XLoopValue, base.Context);
			}
			string subject = template.Subject;
			if (!string.IsNullOrEmpty(subject))
			{
				base.Context.TraceDebug<string>("Reply action: setting subject: {0}", subject);
				messageItem.Subject = subject;
			}
			messageItem[ItemSchema.SpamConfidenceLevel] = -1;
			messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
			return messageItem;
		}

		private void CopyRecipientsFromTemplate(MessageItem template, MessageItem newMessage)
		{
			if ((this.flags & RuleAction.Reply.ActionFlags.DoNotSendToOriginator) != RuleAction.Reply.ActionFlags.None)
			{
				newMessage.Recipients.Clear();
				if (template.Recipients.Count == 0)
				{
					base.Context.TraceError("Reply action: Template Message has no recipients");
				}
			}
			foreach (Recipient recipient in template.Recipients)
			{
				base.Context.TraceDebug<Recipient>("Reply action: Adding recipient {0}", recipient);
				newMessage.Recipients.Add(recipient);
			}
			base.SetRecipientsResponsibility(newMessage);
		}

		private static PropertyDefinition[] ReplyTemplateRelatedProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ReplyTemplateId
		};

		private static QueryFilter TemplateMessageQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Rules.ReplyTemplate.Microsoft");

		private RuleAction.Reply.ActionFlags flags;

		private byte[] messageTemplateEntryId;

		private Guid replyTemplateGuid;

		private enum ReplyTemplatePropertiesIndex
		{
			ItemIdIndex,
			ReplyTemplateGuidIndex
		}
	}
}

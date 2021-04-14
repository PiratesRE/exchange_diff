using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CopyWorkItem : WorkItem
	{
		public CopyWorkItem(IRuleEvaluationContext context, Folder folder, int actionIndex) : base(context, actionIndex)
		{
			this.targetFolder = folder;
		}

		public static Action<MailboxSession, object, object, object, object, object, object, MessageItem, bool> ApplyPolicy
		{
			get
			{
				if (CopyWorkItem.applyPolicy == null)
				{
					CopyWorkItem.applyPolicy = (Action<MailboxSession, object, object, object, object, object, object, MessageItem, bool>)Delegate.CreateDelegate(typeof(Action<MailboxSession, object, object, object, object, object, object, MessageItem, bool>), Type.GetType("Microsoft.Exchange.InfoWorker.Common.ELC.RetentionTagHelper, Microsoft.Exchange.InfoWorker.Common").GetMethod("ApplyPolicy", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(MailboxSession),
						typeof(object),
						typeof(object),
						typeof(object),
						typeof(object),
						typeof(object),
						typeof(object),
						typeof(MessageItem),
						typeof(bool)
					}, null));
				}
				return CopyWorkItem.applyPolicy;
			}
		}

		public override ExecutionStage Stage
		{
			get
			{
				return ExecutionStage.OnPromotedMessage | ExecutionStage.OnDeliveredMessage;
			}
		}

		public StoreId TargetFolderId
		{
			get
			{
				return this.targetFolder.Id;
			}
		}

		public override void Execute()
		{
			if (!this.ShouldExecuteOnThisStage)
			{
				return;
			}
			if (base.Context.DeliveryFolder != null && this.TargetFolderId.Equals(base.Context.DeliveryFolder.Id))
			{
				base.Context.TraceDebug("Copy action: Ignoring copy to same folder.");
				return;
			}
			bool flag = false;
			MailboxSession mailboxSession = base.Context.StoreSession as MailboxSession;
			if (mailboxSession != null && CopyWorkItem.IsRententionPolicyEnabled(base.Context.RecipientCache, base.Context.Recipient))
			{
				base.Context.TraceDebug("Copy action: Retention policy is enabled.");
				flag = true;
			}
			if (ExecutionStage.OnPromotedMessage == base.Context.ExecutionStage)
			{
				base.Context.TraceDebug("Copy action: Creating message.");
				using (MessageItem messageItem = MessageItem.Create(base.Context.StoreSession, this.TargetFolderId))
				{
					base.Context.TraceDebug("Copy action: Copying content from original message.");
					Item.CopyItemContent(base.Context.Message, messageItem);
					if (base.Context.PropertiesForAllMessageCopies != null)
					{
						foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in base.Context.PropertiesForAllMessageCopies)
						{
							if (keyValuePair.Value != null)
							{
								base.Context.TraceDebug<string, object>("Copy action: Setting property {0} to {1}", keyValuePair.Key.Name, keyValuePair.Value);
							}
							else
							{
								base.Context.TraceDebug<string>("Copy action: Removing property {0}", keyValuePair.Key.Name);
							}
							messageItem.SetOrDeleteProperty(keyValuePair.Key, keyValuePair.Value);
						}
					}
					MessageFlags messageFlags = messageItem.GetValueOrDefault<MessageFlags>(MessageItemSchema.Flags);
					messageFlags |= MessageFlags.IsUnmodified;
					messageFlags &= ~(MessageFlags.IsRead | MessageFlags.IsDraft);
					messageItem[MessageItemSchema.Flags] = messageFlags;
					if (flag)
					{
						this.TagItem(mailboxSession, messageItem);
					}
					else
					{
						base.Context.TraceDebug("Copy action: Retention policy is not enabled. Skip tagging.");
					}
					base.Context.TraceDebug("Copy action: Saving message.");
					messageItem.Save(SaveMode.ResolveConflicts);
					base.Context.TraceDebug("Copy action: Message saved.");
					return;
				}
			}
			if (ExecutionStage.OnDeliveredMessage == base.Context.ExecutionStage)
			{
				base.Context.TraceDebug("Copy action: Loading original message.");
				base.Context.DeliveredMessage.Load(new PropertyDefinition[]
				{
					ItemSchema.Id
				});
				if (base.Context.DeliveredMessage.Id == null)
				{
					base.Context.TraceError("Copy action: Cannot copy message since the message is not deliverred to store.");
					return;
				}
				if (!flag)
				{
					if (!base.Context.FinalDeliveryFolderId.Equals(base.Context.DeliveryFolder.Id))
					{
						base.Context.TraceDebug("Copy action: Binding to final delivery folder.");
						using (Folder folder = Folder.Bind(base.Context.StoreSession, base.Context.FinalDeliveryFolderId))
						{
							base.Context.TraceDebug("Copy action: Copying from final delivery folder.");
							folder.CopyItems(this.TargetFolderId, new StoreId[]
							{
								base.Context.DeliveredMessage.Id
							});
							return;
						}
					}
					base.Context.TraceDebug("Copy action: Copying from initial delivery folder.");
					base.Context.DeliveryFolder.CopyItems(this.TargetFolderId, new StoreId[]
					{
						base.Context.DeliveredMessage.Id
					});
					return;
				}
				base.Context.TraceDebug("Copy action: Copying from delivery folder and fixing RetentionPolicies on Item.");
				AggregateOperationResult aggregateOperationResult = mailboxSession.Copy(mailboxSession, this.TargetFolderId, true, new StoreId[]
				{
					base.Context.DeliveredMessage.Id
				});
				if (aggregateOperationResult.OperationResult == OperationResult.Succeeded && aggregateOperationResult.GroupOperationResults.Length == 1 && aggregateOperationResult.GroupOperationResults[0].ResultObjectIds.Count == 1)
				{
					PropertyDefinition[] array = new PropertyDefinition[]
					{
						ItemSchema.Id,
						StoreObjectSchema.PolicyTag,
						StoreObjectSchema.RetentionFlags,
						StoreObjectSchema.RetentionPeriod,
						StoreObjectSchema.ArchiveTag,
						StoreObjectSchema.ArchivePeriod,
						FolderSchema.RetentionTagEntryId
					};
					using (MessageItem messageItem2 = MessageItem.Bind(base.Context.StoreSession, aggregateOperationResult.GroupOperationResults[0].ResultObjectIds[0], (ICollection<PropertyDefinition>)array))
					{
						messageItem2.OpenAsReadWrite();
						this.TagItem(mailboxSession, messageItem2);
						base.Context.TraceDebug("Copy action: Saving message.");
						messageItem2.Save(SaveMode.ResolveConflicts);
						base.Context.TraceDebug("Copy action: Message saved.");
					}
				}
			}
		}

		private void TagItem(MailboxSession mailboxSession, MessageItem message)
		{
			if (!CopyWorkItem.IsMessageExplicitlyTagged(message))
			{
				base.Context.TraceDebug("Copy action: Message is not explicitly tagged. Applying Retention and Archive Tag.");
				CopyWorkItem.ApplyPolicy(mailboxSession, this.targetFolder.GetValueOrDefault<object>(StoreObjectSchema.PolicyTag), this.targetFolder.GetValueOrDefault<object>(StoreObjectSchema.RetentionFlags), this.targetFolder.GetValueOrDefault<object>(StoreObjectSchema.RetentionPeriod), this.targetFolder.GetValueOrDefault<object>(StoreObjectSchema.ArchiveTag), this.targetFolder.GetValueOrDefault<object>(StoreObjectSchema.ArchivePeriod), this.targetFolder.GetValueOrDefault<object>(FolderSchema.RetentionTagEntryId), message, false);
				return;
			}
			base.Context.TraceDebug("Copy action: Message is explicitly tagged by Retention tag. Only Applying Archive Tag.");
			CopyWorkItem.ApplyPolicy(mailboxSession, null, null, null, this.targetFolder.GetValueOrDefault<object>(StoreObjectSchema.ArchiveTag), this.targetFolder.GetValueOrDefault<object>(StoreObjectSchema.ArchivePeriod), this.targetFolder.GetValueOrDefault<object>(FolderSchema.RetentionTagEntryId), message, true);
		}

		private static bool IsRententionPolicyEnabled(IADRecipientCache cache, ProxyAddress address)
		{
			ADRawEntry data = cache.FindAndCacheRecipient(address).Data;
			if (data == null)
			{
				return false;
			}
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)data[ADUserSchema.ElcMailboxFlags];
			ADObjectId adobjectId = (ADObjectId)data[ADUserSchema.ElcPolicyTemplate];
			return ((elcMailboxFlags & ElcMailboxFlags.ElcV2) != ElcMailboxFlags.None && adobjectId != null) || ((elcMailboxFlags & ElcMailboxFlags.ShouldUseDefaultRetentionPolicy) != ElcMailboxFlags.None && adobjectId == null);
		}

		private static bool IsMessageExplicitlyTagged(MessageItem message)
		{
			return message.TryGetProperty(StoreObjectSchema.RetentionPeriod) is int?;
		}

		private static Action<MailboxSession, object, object, object, object, object, object, MessageItem, bool> applyPolicy;

		private Folder targetFolder;
	}
}

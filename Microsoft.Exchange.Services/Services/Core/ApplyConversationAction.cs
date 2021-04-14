using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ApplyConversationAction : MultiStepServiceCommand<ApplyConversationActionRequest, ApplyConversationActionResponseMessage>
	{
		public ApplyConversationAction(CallContext callContext, ApplyConversationActionRequest request) : base(callContext, request)
		{
		}

		internal override ServiceResult<ApplyConversationActionResponseMessage> Execute()
		{
			return this.ApplyConversationActions(base.CurrentStep);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ApplyConversationActionResponse applyConversationActionResponse = new ApplyConversationActionResponse();
			applyConversationActionResponse.AddResponses(base.Results);
			return applyConversationActionResponse;
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.ConversationActions.Length;
			}
		}

		private ServiceResult<ApplyConversationActionResponseMessage> ApplyConversationActions(int index)
		{
			ConversationAction conversationAction = base.Request.ConversationActions[index];
			ItemId conversationId = conversationAction.ConversationId;
			AggregateOperationResult aggregateOperationResult = null;
			TargetFolderId destinationFolderId = conversationAction.DestinationFolderId;
			ConversationActionType action = conversationAction.Action;
			bool processRightAway = conversationAction.ProcessRightAway;
			DateTime? utcDateTime = ServiceCommandBase.GetUtcDateTime(ServiceCommandBase.ParseExDateTimeString(conversationAction.ConversationLastSyncTime));
			ServiceCommandBase.ThrowIfNull(conversationId, "conversationId", "ApplyConversationAction::Execute");
			IdAndSession sessionFromConversationId = this.GetSessionFromConversationId(conversationId);
			StoreObjectId contextFolderId = null;
			if (conversationAction.ContextFolderId != null)
			{
				IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(conversationAction.ContextFolderId.BaseFolderId);
				contextFolderId = ((idAndSession == null) ? null : StoreId.GetStoreObjectId(idAndSession.Id));
			}
			MailboxSession mailboxSession = sessionFromConversationId.Session as MailboxSession;
			ApplyConversationActionResponseMessage applyConversationActionResponseMessage = new ApplyConversationActionResponseMessage();
			PropertyDefinition[] propertyDefinitions = null;
			if (action == ConversationActionType.UpdateAlwaysCategorizeRule && (conversationAction.CategoriesToRemove != null || conversationAction.ClearCategories))
			{
				propertyDefinitions = ApplyConversationAction.conversationCategoryProperties;
			}
			else if (action == ConversationActionType.Flag)
			{
				propertyDefinitions = ApplyConversationAction.conversationFlaggingProperties;
			}
			Conversation conversation = Conversation.Load(mailboxSession, sessionFromConversationId.Id as ConversationId, propertyDefinitions);
			if (conversation.ConversationTree.Count == 0)
			{
				return new ServiceResult<ApplyConversationActionResponseMessage>(applyConversationActionResponseMessage);
			}
			if (action == ConversationActionType.AlwaysCategorize)
			{
				aggregateOperationResult = conversation.AlwaysCategorize(conversationAction.Categories, processRightAway);
			}
			else if (action == ConversationActionType.AlwaysDelete)
			{
				aggregateOperationResult = conversation.AlwaysDelete(conversationAction.EnableAlwaysDelete, processRightAway);
			}
			else if (action == ConversationActionType.AlwaysMove)
			{
				if (destinationFolderId != null)
				{
					IdAndSession sessionFromMoveFolderXml = this.GetSessionFromMoveFolderXml(destinationFolderId);
					StoreObjectId asStoreObjectId = sessionFromMoveFolderXml.GetAsStoreObjectId();
					try
					{
						aggregateOperationResult = conversation.AlwaysMove(asStoreObjectId, processRightAway);
						goto IL_4C4;
					}
					catch (InvalidFolderTypeException)
					{
						throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationActionInvalidFolderType);
					}
				}
				aggregateOperationResult = conversation.AlwaysMove(null, processRightAway);
			}
			else if (action == ConversationActionType.Copy)
			{
				if (destinationFolderId == null)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationActionNeedDestinationFolderForCopyAction);
				}
				IdAndSession sessionFromMoveFolderXml2 = this.GetSessionFromMoveFolderXml(destinationFolderId);
				aggregateOperationResult = conversation.Copy(contextFolderId, utcDateTime, sessionFromMoveFolderXml2.Session, StoreId.GetStoreObjectId(sessionFromMoveFolderXml2.Id));
			}
			else if (action == ConversationActionType.Move)
			{
				if (destinationFolderId == null)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationActionNeedDestinationFolderForMoveAction);
				}
				IdAndSession sessionFromMoveFolderXml3 = this.GetSessionFromMoveFolderXml(destinationFolderId);
				MailboxTarget mailboxTarget = base.GetMailboxTarget(sessionFromMoveFolderXml3.Session);
				if (mailboxTarget == MailboxTarget.SharedFolder)
				{
					aggregateOperationResult = conversation.Copy(contextFolderId, utcDateTime, sessionFromMoveFolderXml3.Session, StoreId.GetStoreObjectId(sessionFromMoveFolderXml3.Id));
					if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
					{
						DeleteItemFlags deleteFlags = DeleteItemFlags.HardDelete | DeleteItemFlags.SuppressReadReceipt;
						conversation.Delete(contextFolderId, utcDateTime, deleteFlags);
					}
				}
				else
				{
					bool returnMovedItemIds = base.Request.ReturnMovedItemIds;
					aggregateOperationResult = conversation.Move(contextFolderId, utcDateTime, sessionFromMoveFolderXml3.Session, StoreId.GetStoreObjectId(sessionFromMoveFolderXml3.Id), returnMovedItemIds);
					if (returnMovedItemIds)
					{
						applyConversationActionResponseMessage.MovedItemIds = this.GetMovedItemIds(aggregateOperationResult, mailboxSession);
					}
				}
			}
			else if (action == ConversationActionType.SetReadState)
			{
				if (conversationAction.IsRead == null)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3601113588U);
				}
				bool suppressReadReceiptsSetting = this.GetSuppressReadReceiptsSetting(conversationAction, contextFolderId, mailboxSession);
				aggregateOperationResult = conversation.SetIsReadState(contextFolderId, utcDateTime, conversationAction.IsRead.Value, suppressReadReceiptsSetting);
			}
			else if (action == ConversationActionType.Delete)
			{
				if (conversationAction.DeleteType == null)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationActionNeedDeleteTypeForSetDeleteAction);
				}
				DeleteItemFlags deleteItemFlags = (DeleteItemFlags)conversationAction.DeleteType.Value;
				bool suppressReadReceiptsSetting2 = this.GetSuppressReadReceiptsSetting(conversationAction, contextFolderId, mailboxSession);
				if (suppressReadReceiptsSetting2)
				{
					deleteItemFlags |= DeleteItemFlags.SuppressReadReceipt;
				}
				bool returnMovedItemIds2 = base.Request.ReturnMovedItemIds;
				aggregateOperationResult = conversation.Delete(contextFolderId, utcDateTime, deleteItemFlags, returnMovedItemIds2);
				if (returnMovedItemIds2)
				{
					applyConversationActionResponseMessage.MovedItemIds = this.GetMovedItemIds(aggregateOperationResult, mailboxSession);
				}
			}
			else if (action == ConversationActionType.SetRetentionPolicy)
			{
				RetentionType? retentionPolicyType = conversationAction.RetentionPolicyType;
				string retentionPolicyTagId = conversationAction.RetentionPolicyTagId;
				if (retentionPolicyType == null)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3967405104U);
				}
				bool isArchiveAction = retentionPolicyType == RetentionType.Archive;
				PolicyTag andValidatePolicyTag = this.GetAndValidatePolicyTag(mailboxSession, conversation, retentionPolicyTagId, isArchiveAction);
				aggregateOperationResult = conversation.SetRetentionPolicy(contextFolderId, utcDateTime, andValidatePolicyTag, isArchiveAction);
			}
			else if (action == ConversationActionType.UpdateAlwaysCategorizeRule)
			{
				string[] categories = conversationAction.Categories;
				string[] categoriesToRemove = conversationAction.CategoriesToRemove;
				bool clearCategories = conversationAction.ClearCategories;
				if (clearCategories)
				{
					aggregateOperationResult = conversation.AlwaysCategorize(null, processRightAway);
				}
				else
				{
					aggregateOperationResult = this.UpdateAlwaysCategorizeRule(mailboxSession, conversation, sessionFromConversationId.Id as ConversationId, categories, categoriesToRemove, processRightAway);
				}
				if (clearCategories)
				{
					this.ClearCategories(conversation, mailboxSession);
				}
				else if (categoriesToRemove != null)
				{
					this.RemoveCategories(conversation, mailboxSession, categoriesToRemove);
				}
			}
			else if (action == ConversationActionType.Flag)
			{
				FlagType flag = conversationAction.Flag;
				if (flag == null || !flag.IsValid())
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationActionNeedFlagForFlagAction);
				}
				aggregateOperationResult = this.FlagConversationItems(contextFolderId, conversation, utcDateTime, flag);
			}
			else if (action == ConversationActionType.SetClutterState)
			{
				if (conversationAction.IsClutter == null)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3601113588U);
				}
				aggregateOperationResult = conversation.AlwaysClutterOrUnclutter(conversationAction.IsClutter, true);
			}
			IL_4C4:
			if (aggregateOperationResult != null && aggregateOperationResult.OperationResult != OperationResult.Succeeded)
			{
				throw new ApplyConversationActionException
				{
					ConstantValues = 
					{
						{
							"FailedOperation",
							Enum.GetName(typeof(ConversationActionType), action)
						}
					}
				};
			}
			return new ServiceResult<ApplyConversationActionResponseMessage>(applyConversationActionResponseMessage);
		}

		private bool GetSuppressReadReceiptsSetting(ConversationAction action, StoreObjectId contextFolderId, MailboxSession session)
		{
			return (contextFolderId != null && session.IsDefaultFolderType(contextFolderId) == DefaultFolderType.JunkEmail) || action.SuppressReadReceipts == null || action.SuppressReadReceipts.Value;
		}

		private ItemId[] GetMovedItemIds(AggregateOperationResult result, MailboxSession session)
		{
			ItemId[] array = null;
			if (result != null && result.GroupOperationResults != null && result.GroupOperationResults.Length > 0 && result.GroupOperationResults[0].OperationResult == OperationResult.Succeeded && result.GroupOperationResults[0].ResultObjectIds != null && result.GroupOperationResults[0].ResultObjectIds.Count > 0)
			{
				array = new ItemId[result.GroupOperationResults[0].ResultObjectIds.Count];
				for (int i = 0; i < result.GroupOperationResults[0].ResultObjectIds.Count; i++)
				{
					StoreId storeItemId = IdConverter.CombineStoreObjectIdWithChangeKey(result.GroupOperationResults[0].ResultObjectIds[i], result.GroupOperationResults[0].ResultChangeKeys[i]);
					array[i] = IdConverter.ConvertStoreItemIdToItemId(storeItemId, session);
				}
			}
			return array;
		}

		private AggregateOperationResult FlagConversationItems(StoreObjectId contextFolderId, Conversation conversation, DateTime? conversationLastSyncTime, FlagType flagAction)
		{
			AggregateOperationResult result = null;
			switch (flagAction.FlagStatus)
			{
			case FlagStatus.NotFlagged:
				result = conversation.ClearFlags(contextFolderId, conversationLastSyncTime);
				break;
			case FlagStatus.Complete:
			{
				ExDateTime? completeDate = ServiceCommandBase.ParseExDateTimeString(flagAction.CompleteDate);
				result = conversation.CompleteFlags(contextFolderId, conversationLastSyncTime, completeDate);
				break;
			}
			case FlagStatus.Flagged:
			{
				ExDateTime? startDate = ServiceCommandBase.ParseExDateTimeString(flagAction.StartDate);
				ExDateTime? dueDate = ServiceCommandBase.ParseExDateTimeString(flagAction.DueDate);
				result = conversation.SetFlags(contextFolderId, conversationLastSyncTime, CoreResources.FlagForFollowUp, startDate, dueDate);
				break;
			}
			}
			return result;
		}

		private IdAndSession GetSessionFromConversationId(BaseItemId conversationId)
		{
			IdAndSession idAndSession = base.IdConverter.ConvertConversationIdToIdAndSession(conversationId);
			if (!(idAndSession.Session is MailboxSession))
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationSupportedOnlyForMailboxSession);
			}
			return idAndSession;
		}

		private IdAndSession GetSessionFromMoveFolderXml(TargetFolderId moveFolderId)
		{
			IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(moveFolderId.BaseFolderId);
			if (!(idAndSession.Session is MailboxSession))
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationActionAlwaysMoveNoPublicFolder);
			}
			return idAndSession;
		}

		private PolicyTag GetAndValidatePolicyTag(MailboxSession session, Conversation conversation, string retentionId, bool isArchiveAction)
		{
			PolicyTag policyTag = null;
			if (!string.IsNullOrEmpty(retentionId))
			{
				Guid guid;
				if (!Guid.TryParse(retentionId, out guid) || !(guid != Guid.Empty))
				{
					ExTraceGlobals.ELCTracer.TraceError<Guid, ConversationId>((long)this.GetHashCode(), "[ApplyConversationAction::ApplyConversationActions] Tag {0} is not a valid guid for {1}", guid, conversation.ConversationId);
					throw new InvalidRetentionTagException(CoreResources.IDs.ErrorInvalidRetentionTagIdGuid);
				}
				Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType type = isArchiveAction ? Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType.MoveToArchive : Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType.DeleteAndAllowRecovery;
				PolicyTagList policyTagList = session.GetPolicyTagList(type);
				if (!policyTagList.TryGetValue(guid, out policyTag))
				{
					ExTraceGlobals.ELCTracer.TraceError<Guid, ConversationId>((long)this.GetHashCode(), "[ApplyConversationAction::ApplyConversationActions] Tag {0} has incorrect intended action type for {1}", guid, conversation.ConversationId);
					throw new InvalidRetentionTagException(CoreResources.IDs.ErrorInvalidRetentionTagTypeMismatch);
				}
				if (!policyTag.IsVisible)
				{
					ExTraceGlobals.ELCTracer.TraceError<Guid, ConversationId>((long)this.GetHashCode(), "[ApplyConversationAction::ApplyConversationActions] Tag {0} is an invisible tag for {1}", guid, conversation.ConversationId);
					throw new InvalidRetentionTagException((CoreResources.IDs)4105318492U);
				}
			}
			return policyTag;
		}

		private AggregateOperationResult UpdateAlwaysCategorizeRule(MailboxSession mailboxSession, Conversation conversation, ConversationId conversationId, string[] categoriesToAdd, string[] categoriesToRemove, bool processRightAway)
		{
			AggregateOperationResult result = null;
			string[] array = null;
			using (ConversationActionItem conversationActionItem = this.GetConversationActionItem(conversationId, mailboxSession))
			{
				if (conversationActionItem != null)
				{
					array = conversationActionItem.AlwaysCategorizeValue;
				}
			}
			List<string> list = new List<string>();
			if (array != null)
			{
				list.AddRange(array);
			}
			bool flag = false;
			if (categoriesToRemove != null && list.Count > 0)
			{
				foreach (string item in categoriesToRemove)
				{
					if (list.Contains(item))
					{
						list.Remove(item);
						flag = true;
					}
				}
			}
			if (categoriesToAdd != null)
			{
				foreach (string item2 in categoriesToAdd)
				{
					if (!list.Contains(item2))
					{
						list.Add(item2);
						flag = true;
					}
				}
			}
			if (flag)
			{
				result = conversation.AlwaysCategorize(list.ToArray(), processRightAway);
			}
			return result;
		}

		public void ClearCategories(Conversation conversation, MailboxSession mailboxSession)
		{
			this.UpdateAllConversationNodes(conversation, mailboxSession, delegate(IStorePropertyBag propertyBag)
			{
				string[] property = this.GetProperty<string[]>(propertyBag, ItemSchema.Categories, null);
				return property == null;
			}, delegate(Item item)
			{
				item.Categories.Clear();
			});
		}

		public void RemoveCategories(Conversation conversation, MailboxSession mailboxSession, string[] categoriesToRemove)
		{
			foreach (IConversationTreeNode conversationTreeNode in conversation.ConversationTree)
			{
				ConversationTreeNode conversationTreeNode2 = (ConversationTreeNode)conversationTreeNode;
				foreach (IStorePropertyBag storePropertyBag in conversationTreeNode2.StorePropertyBags)
				{
					string[] property = this.GetProperty<string[]>(storePropertyBag, ItemSchema.Categories, null);
					if (property != null)
					{
						foreach (string value in property)
						{
							if (Array.IndexOf<string>(categoriesToRemove, value) != -1)
							{
								VersionedId versionedId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
								if (versionedId != null)
								{
									using (Item item = Item.Bind(mailboxSession, versionedId.ObjectId))
									{
										item.OpenAsReadWrite();
										for (int j = 0; j < categoriesToRemove.Length; j++)
										{
											item.Categories.Remove(categoriesToRemove[j]);
										}
										item.Save(SaveMode.ResolveConflicts);
										break;
									}
								}
								ApplyConversationAction.Tracer.TraceDebug((long)this.GetHashCode(), "[ApplyConversationAction::RemoveCategories] skipping item categories update because versionedId == null");
								break;
							}
						}
					}
				}
			}
		}

		private void UpdateAllConversationNodes(Conversation conversation, MailboxSession mailboxSession, Func<IStorePropertyBag, bool> skipNodeFunc, Action<Item> updateAction)
		{
			foreach (IConversationTreeNode conversationTreeNode in conversation.ConversationTree)
			{
				ConversationTreeNode conversationTreeNode2 = (ConversationTreeNode)conversationTreeNode;
				foreach (IStorePropertyBag storePropertyBag in conversationTreeNode2.StorePropertyBags)
				{
					if (!skipNodeFunc(storePropertyBag))
					{
						VersionedId versionedId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
						if (versionedId != null)
						{
							using (Item item = Item.Bind(mailboxSession, versionedId.ObjectId))
							{
								item.OpenAsReadWrite();
								updateAction(item);
								item.Save(SaveMode.ResolveConflicts);
								continue;
							}
						}
						ApplyConversationAction.Tracer.TraceDebug((long)this.GetHashCode(), "[ApplyConversationAction::UpdateAllConversationNodes] skipping set because versionedId == null");
					}
				}
			}
		}

		private T GetProperty<T>(IStorePropertyBag propertyBag, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = propertyBag.TryGetProperty(propertyDefinition);
			if (obj is PropertyError || obj == null)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private ConversationActionItem GetConversationActionItem(ConversationId conversationId, MailboxSession mailboxSession)
		{
			StoreId storeId = ConversationActionItem.QueryConversationActionsFolder(mailboxSession, conversationId);
			ConversationActionItem result = null;
			if (storeId != null)
			{
				result = ConversationActionItem.Bind(mailboxSession, storeId);
			}
			return result;
		}

		private IdAndSession GetSessionFromConversationId(XmlElement conversationIdNode)
		{
			IdAndSession idAndSession = base.IdConverter.ConvertConversationIdXmlToIdAndSession(conversationIdNode);
			if (!(idAndSession.Session is MailboxSession))
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ConversationSupportedOnlyForMailboxSession);
			}
			return idAndSession;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ApplyConversationActionCallTracer;

		private static PropertyDefinition[] conversationCategoryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ParentItemId,
			ItemSchema.Categories
		};

		private static PropertyDefinition[] conversationFlaggingProperties = new PropertyDefinition[]
		{
			ItemSchema.FlagStatus
		};
	}
}

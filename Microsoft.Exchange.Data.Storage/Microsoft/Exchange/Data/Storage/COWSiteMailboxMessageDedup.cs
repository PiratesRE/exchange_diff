using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class COWSiteMailboxMessageDedup : ICOWNotification
	{
		internal COWSiteMailboxMessageDedup()
		{
		}

		public bool SkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(session, "session");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null || !Utils.IsTeamMailbox(session) || !ClientInfo.MOMT.IsMatch(session.ClientInfoString))
			{
				return true;
			}
			if (callbackContext.SiteMailboxMessageDedupState == COWProcessorState.DoNotProcess)
			{
				COWSiteMailboxMessageDedup.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.SkipItemOperation: skipping notification for item {0} because it should not be processed.", itemId);
				return true;
			}
			if (callbackContext.SiteMailboxMessageDedupState == COWProcessorState.Processed)
			{
				COWSiteMailboxMessageDedup.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.SkipItemOperation: skipping notification for item {0} because it was already processed.", itemId);
				return true;
			}
			if (callbackContext.SiteMailboxMessageDedupState == COWProcessorState.ProcessAfterSave)
			{
				return onBeforeNotification;
			}
			if (onDumpster || item == null || operation != COWTriggerAction.Create)
			{
				callbackContext.SiteMailboxMessageDedupState = COWProcessorState.DoNotProcess;
				return true;
			}
			if (!settings.IsSiteMailboxMessageDedupEnabled())
			{
				callbackContext.SiteMailboxMessageDedupState = COWProcessorState.DoNotProcess;
				COWSiteMailboxMessageDedup.Tracer.Information<string>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.SkipItemOperation: skipping message dedup for site mailbox {0} because the feature is disabled for the mailbox.", mailboxSession.DisplayName);
				return true;
			}
			ExDateTime now = ExDateTime.Now;
			bool flag = false;
			try
			{
				flag = this.IsDuplicateMessageBySiteMailboxDrop(mailboxSession, item);
			}
			catch (StoragePermanentException arg)
			{
				COWSiteMailboxMessageDedup.Tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.SkipItemOperation: got store permanent exception: {0}", arg);
			}
			catch (StorageTransientException arg2)
			{
				COWSiteMailboxMessageDedup.Tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.SkipItemOperation: got store transient exception: {0}", arg2);
			}
			if (flag)
			{
				callbackContext.SiteMailboxMessageDedupState = COWProcessorState.ProcessAfterSave;
			}
			else
			{
				callbackContext.SiteMailboxMessageDedupState = COWProcessorState.DoNotProcess;
			}
			COWSiteMailboxMessageDedup.Tracer.TraceDebug((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.SkipItemOperation: inspecting message {0} dragged & dropped to site mailbox {1} costed {2} milliseconds, and the result is {3}.", new object[]
			{
				itemId,
				mailboxSession.DisplayName,
				(ExDateTime.Now - now).TotalMilliseconds,
				callbackContext.SiteMailboxMessageDedupState
			});
			return true;
		}

		public void ItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, CoreFolder folder, bool onBeforeNotification, OperationResult result, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(session, "session");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			Util.ThrowOnNullArgument(item, "item");
			MailboxSession mailboxSession = session as MailboxSession;
			ExDateTime now = ExDateTime.Now;
			try
			{
				item.PropertyBag.Load(COWSiteMailboxMessageDedup.PropsForMessageRemoval);
				if (item.Id != null && item.Id.ObjectId != null)
				{
					StoreObjectId storeObjectId = item.PropertyBag.TryGetProperty(StoreObjectSchema.ParentItemId) as StoreObjectId;
					if (storeObjectId != null)
					{
						using (CoreFolder coreFolder = CoreFolder.Bind(session, storeObjectId))
						{
							DeleteItemFlags deleteItemFlags = DeleteItemFlags.HardDelete | DeleteItemFlags.SuppressReadReceipt;
							coreFolder.DeleteItems(deleteItemFlags, new StoreObjectId[]
							{
								item.Id.ObjectId
							});
						}
						COWSiteMailboxMessageDedup.Tracer.TraceDebug<StoreObjectId, string, double>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.ItemOperation: deleting message {0} from site mailbox {1} used {2} milliseconds", itemId, mailboxSession.DisplayName, (ExDateTime.Now - now).TotalMilliseconds);
					}
				}
			}
			catch (StoragePermanentException arg)
			{
				COWSiteMailboxMessageDedup.Tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.ItemOperation: got store permanent exception: {0}", arg);
			}
			catch (StorageTransientException arg2)
			{
				COWSiteMailboxMessageDedup.Tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.ItemOperation: got store transient exception: {0}", arg2);
			}
			callbackContext.SiteMailboxMessageDedupState = COWProcessorState.Processed;
		}

		public CowClientOperationSensitivity SkipGroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, bool onBeforeNotification, bool onDumpster, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			return CowClientOperationSensitivity.Skip;
		}

		public void GroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, bool onBeforeNotification, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
		}

		private bool IsDuplicateMessageBySiteMailboxDrop(MailboxSession session, CoreItem item)
		{
			if (item.Origin != Origin.New)
			{
				return false;
			}
			item.PropertyBag.Load(COWSiteMailboxMessageDedup.PropItemClass);
			if (item.ClassName() != "IPM.Note")
			{
				return false;
			}
			byte[] array = item.PropertyBag.TryGetProperty(StoreObjectSchema.ParentEntryId) as byte[];
			string text = item.PropertyBag.TryGetProperty(ItemSchema.InternetMessageId) as string;
			string text2 = item.PropertyBag.TryGetProperty(MessageItemSchema.SenderSmtpAddress) as string;
			ExDateTime? exDateTime = item.PropertyBag.TryGetProperty(ItemSchema.SentTime) as ExDateTime?;
			string a = item.PropertyBag.TryGetProperty(ItemSchema.Subject) as string;
			if (array == null || string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2) || exDateTime == null)
			{
				COWSiteMailboxMessageDedup.Tracer.TraceError((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.IsDuplicateMessageBySiteMailboxDrop: required poperties are not complete, rawId: {0}, internetMessageId: {1}, senderSmtpAddress: {2}, sentTime: {3}", new object[]
				{
					array,
					text,
					text2,
					exDateTime
				});
				return false;
			}
			int hashValue = (int)AllItemsFolderHelper.GetHashValue(text);
			StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Message);
			using (Folder folder = Folder.Bind(session, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, COWSiteMailboxMessageDedup.DedupSortBy, COWSiteMailboxMessageDedup.DedupProperties))
				{
					if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InternetMessageIdHash, hashValue)))
					{
						return false;
					}
					IStorePropertyBag[] array2 = AllItemsFolderHelper.ProcessQueryResult(queryResult, text, hashValue);
					if (array2 == null)
					{
						COWSiteMailboxMessageDedup.Tracer.TraceDebug((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.IsDuplicateMessageBySiteMailboxDrop: propertyBags is null for ProcessQueryResult.");
						return false;
					}
					foreach (IStorePropertyBag storePropertyBag in array2)
					{
						string text3 = storePropertyBag.TryGetProperty(ItemSchema.InternetMessageId) as string;
						string text4 = storePropertyBag.TryGetProperty(MessageItemSchema.SenderSmtpAddress) as string;
						string b = storePropertyBag.TryGetProperty(ItemSchema.Subject) as string;
						ExDateTime? exDateTime2 = storePropertyBag.TryGetProperty(ItemSchema.SentTime) as ExDateTime?;
						VersionedId versionedId = storePropertyBag.TryGetProperty(ItemSchema.Id) as VersionedId;
						if (!string.IsNullOrEmpty(text3) && string.Equals(text, text3, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(text4) && string.Equals(text2, text4, StringComparison.OrdinalIgnoreCase) && string.Equals(a, b) && exDateTime2 != null && exDateTime2 == exDateTime)
						{
							if (item.Id == null || item.Id.ObjectId == null || versionedId == null || versionedId.ObjectId == null)
							{
								COWSiteMailboxMessageDedup.Tracer.TraceError((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.IsDuplicateMessageBySiteMailboxDrop: some following values are unavailable: item.Id, item.Id.ObjectId, matchedId, matchedId.ObjectId");
							}
							else
							{
								COWSiteMailboxMessageDedup.Tracer.TraceDebug<StoreObjectId, StoreObjectId, string>((long)this.GetHashCode(), "COWSiteMailboxMessageDedup.IsDuplicateMessageBySiteMailboxDrop: found duplicate {0} of message {1} in site mailbox {2}.", versionedId.ObjectId, item.Id.ObjectId, session.DisplayName);
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		internal static readonly Trace Tracer = ExTraceGlobals.SiteMailboxMessageDedupTracer;

		private static readonly PropertyDefinition[] DedupProperties = new PropertyDefinition[]
		{
			ItemSchema.InternetMessageIdHash,
			ItemSchema.InternetMessageId,
			MessageItemSchema.SenderSmtpAddress,
			ItemSchema.Subject,
			ItemSchema.SentTime,
			ItemSchema.Id
		};

		private static readonly SortBy[] DedupSortBy = new SortBy[]
		{
			new SortBy(ItemSchema.InternetMessageIdHash, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] PropsForMessageRemoval = new PropertyDefinition[]
		{
			StoreObjectSchema.EntryId,
			StoreObjectSchema.ParentItemId
		};

		private static readonly PropertyDefinition[] PropItemClass = new PropertyDefinition[]
		{
			InternalSchema.ItemClass
		};
	}
}

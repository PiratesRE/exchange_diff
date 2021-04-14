using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.WorkingSet.Publisher;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Groups;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class COWGroupMessageWSPublishing : ICOWNotification
	{
		internal COWGroupMessageWSPublishing()
		{
			this.groupWSPublisher = this.CreateWorkingSetPublisher();
		}

		protected virtual WorkingSetPublisher CreateWorkingSetPublisher()
		{
			return new WorkingSetPublisher();
		}

		public bool SkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(session, "session");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			Util.ThrowOnNullArgument(callbackContext, "callbackContext");
			if (!WorkingSetPublisher.IsGroupWSPublishingEnabled())
			{
				COWGroupMessageWSPublishing.Tracer.Information((long)this.GetHashCode(), "COWGroupMessageWSPublishing.SkipItemOperation: skipping group message working set publishing as the feature is disabled.");
				return true;
			}
			switch (callbackContext.COWGroupMessageWSPublishingState)
			{
			case COWProcessorState.Unknown:
				callbackContext.COWGroupMessageWSPublishingState = this.InspectNotification(operation, session, item, onBeforeNotification, onDumpster);
				COWGroupMessageWSPublishing.Tracer.TraceDebug<StoreObjectId, COWProcessorState>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.SkipItemOperation: inspected item {0} and result is {1}.", itemId, callbackContext.COWGroupMessageWSPublishingState);
				return true;
			case COWProcessorState.DoNotProcess:
				COWGroupMessageWSPublishing.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.SkipItemOperation: skipping notification for item {0} because it should not be processed.", itemId);
				return true;
			case COWProcessorState.ProcessAfterSave:
				return onBeforeNotification;
			case COWProcessorState.Processed:
				COWGroupMessageWSPublishing.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.SkipItemOperation: skipping notification for item {0} because it has already been processed.", itemId);
				return true;
			default:
				return true;
			}
		}

		public void ItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, CoreFolder folder, bool onBeforeNotification, OperationResult result, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(session, "session");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			Util.ThrowOnNullArgument(item, "item");
			Util.ThrowOnNullArgument(callbackContext, "callbackContext");
			if (callbackContext.COWGroupMessageWSPublishingState != COWProcessorState.ProcessAfterSave)
			{
				COWGroupMessageWSPublishing.Tracer.TraceError<COWProcessorState>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.ItemOperation: Skipping working set publishing because the state doesn't indicate processing is needed: {0}", callbackContext.COWGroupMessageWSPublishingState);
				return;
			}
			if (onBeforeNotification)
			{
				COWGroupMessageWSPublishing.Tracer.TraceError((long)this.GetHashCode(), "COWGroupMessageWSPublishing.ItemOperation: Skipping working set publishing because we should only publish after saving");
				return;
			}
			if (result != OperationResult.Succeeded)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug<OperationResult>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.ItemOperation: Skipping working set publishing of message because the operation wasn't successful: {0}", result);
				return;
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceError<OperationResult>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.ItemOperation: Skipping working set publishing the session is not a mailbox session", result);
				return;
			}
			if (mailboxSession.MailboxOwner == null || mailboxSession.MailboxOwner.MailboxInfo == null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceError<OperationResult>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.ItemOperation: Skipping working set publishing, the session does not contain the mailbox info", result);
				return;
			}
			string displayName = mailboxSession.DisplayName;
			string groupId = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			Exception ex = this.groupWSPublisher.PublishGroupPost(item, displayName, groupId);
			if (ex == null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.ItemOperation: working set publishing of message {0} from group mailbox successful", itemId);
			}
			else
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug<StoreObjectId, string>((long)this.GetHashCode(), "COWGroupMessageWSPublishing.ItemOperation: working set publishing of message {0} from group mailbox failed. Error: {1}", itemId, ex.ToString());
			}
			callbackContext.COWGroupMessageWSPublishingState = COWProcessorState.Processed;
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

		private COWProcessorState InspectNotification(COWTriggerAction operation, StoreSession session, CoreItem item, bool onBeforeNotification, bool onDumpster)
		{
			if (onDumpster)
			{
				return COWProcessorState.DoNotProcess;
			}
			if (operation != COWTriggerAction.Create)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: not an Create operation.");
				return COWProcessorState.DoNotProcess;
			}
			if (!onBeforeNotification)
			{
				return COWProcessorState.DoNotProcess;
			}
			if (item == null)
			{
				return COWProcessorState.Unknown;
			}
			if (item.Id != null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: ItemId is non-null, so this is not a new item. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: not a mailbox session. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			int? num = mailboxSession.Mailbox.TryGetProperty(MailboxSchema.MailboxTypeDetail) as int?;
			if (num == null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: mailbox type not found. Try to inspect later.");
				return COWProcessorState.Unknown;
			}
			if (!StoreSession.IsGroupMailbox(num.Value))
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: Mailbox isn't a GroupMailbox. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			string valueOrDefault = item.PropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: item class not found. Try to inspect later.");
				return COWProcessorState.Unknown;
			}
			if (!ObjectClass.IsMessage(valueOrDefault, false))
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: item class is not a message. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			if (ObjectClass.IsMeetingForwardNotification(valueOrDefault))
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: item class is meeting forward notification. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			if (ObjectClass.IsMeetingResponse(valueOrDefault))
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: item class is meeting response. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			bool flag = ClientInfo.OWA.IsMatch(mailboxSession.ClientInfoString);
			bool flag2 = ClientInfo.HubTransport.IsMatch(mailboxSession.ClientInfoString);
			if (!flag && !flag2)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: This isn't either a post or a delivery. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			StoreObjectId storeObjectId = item.PropertyBag.TryGetProperty(StoreObjectSchema.ParentItemId) as StoreObjectId;
			if (storeObjectId == null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: parent folder id not found. Try to inspect later.");
				return COWProcessorState.Unknown;
			}
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			if (defaultFolderId == null)
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: inbox folder id not found. Try to inspect later.");
				return COWProcessorState.Unknown;
			}
			if (!storeObjectId.Equals(defaultFolderId))
			{
				COWGroupMessageWSPublishing.Tracer.TraceDebug((long)this.GetHashCode(), "COWGroupMessageWSPublishing.InspectNotification: This message isn't located on the inbox folder. Do not process.");
				return COWProcessorState.DoNotProcess;
			}
			return COWProcessorState.ProcessAfterSave;
		}

		internal static readonly Trace Tracer = ExTraceGlobals.COWGroupMessageWSPublishingTracer;

		protected readonly WorkingSetPublisher groupWSPublisher;
	}
}

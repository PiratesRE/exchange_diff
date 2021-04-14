using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class COWHardDelete : ICOWNotification
	{
		public bool SkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			return true;
		}

		public void ItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, CoreFolder folder, bool onBeforeNotification, OperationResult result, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
		}

		public CowClientOperationSensitivity SkipGroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, bool onBeforeNotification, bool onDumpster, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			Util.ThrowOnNullArgument(settings, "settings");
			Util.ThrowOnNullArgument(sourceSession, "sourceSession");
			if (onDumpster)
			{
				return CowClientOperationSensitivity.Skip;
			}
			if (!onBeforeNotification)
			{
				return CowClientOperationSensitivity.Skip;
			}
			bool flag;
			if (!settings.HoldEnabled())
			{
				flag = true;
			}
			else
			{
				switch (operation)
				{
				case COWTriggerAction.Move:
				case COWTriggerAction.MoveToDeletedItems:
				{
					Util.ThrowOnNullArgument(sourceSession, "sourceSession");
					if (destinationSession == null)
					{
						flag = true;
						ExTraceGlobals.SessionTracer.TraceDebug((long)dumpster.StoreSession.GetHashCode(), "Destination session is null, meaning item moving to same mailbox, so don't keep a copy.");
						goto IL_187;
					}
					Guid mailboxGuid = destinationSession.MailboxGuid;
					Guid mailboxGuid2 = sourceSession.MailboxGuid;
					if (destinationSession.MailboxGuid == sourceSession.MailboxGuid)
					{
						flag = true;
						ExTraceGlobals.SessionTracer.TraceDebug((long)dumpster.StoreSession.GetHashCode(), "Mailbox guids are the same, meaning item moving to same mailbox, so don't keep a copy.");
						goto IL_187;
					}
					if (destinationSession is MailboxSession && sourceSession is MailboxSession && string.Compare(((MailboxSession)sourceSession).MailboxOwner.LegacyDn, ((MailboxSession)destinationSession).MailboxOwner.LegacyDn, StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						ExTraceGlobals.SessionTracer.TraceDebug((long)dumpster.StoreSession.GetHashCode(), "MailboxOwner.LegacyDistinguishedName is the same, meaning same person owns both mailboxes (eg primary and archive), so don't keep a copy.");
						goto IL_187;
					}
					flag = false;
					ExTraceGlobals.SessionTracer.TraceDebug((long)dumpster.StoreSession.GetHashCode(), "Item moving to a different mailbox owned by someone else. Keep a copy.");
					goto IL_187;
				}
				case COWTriggerAction.HardDelete:
					flag = (settings.IsMrmAction() && DumpsterFolderHelper.IsDumpsterFolder(callbackContext.SessionWithBestAccess, sourceFolderId));
					goto IL_187;
				case COWTriggerAction.DoneWithMessageDelete:
					flag = false;
					goto IL_187;
				}
				flag = true;
			}
			IL_187:
			if (flag)
			{
				return CowClientOperationSensitivity.Skip;
			}
			if (COWSession.IsDelegateSession(sourceSession))
			{
				return CowClientOperationSensitivity.Capture;
			}
			return CowClientOperationSensitivity.CaptureAndPerformOperation;
		}

		public void GroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, bool onBeforeNotification, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			if (settings.CurrentFolderId.Equals(dumpster.RecoverableItemsPurgesFolderId))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)dumpster.StoreSession.GetHashCode(), "Attempt to hard delete items in the dumpster purges folder");
				throw new RecoverableItemsAccessDeniedException("Purges");
			}
			if (settings.CurrentFolderId.Equals(dumpster.RecoverableItemsDiscoveryHoldsFolderId))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)dumpster.StoreSession.GetHashCode(), "Attempt to hard delete items in the dumpster discoveryholds folder");
				throw new RecoverableItemsAccessDeniedException("DiscoveryHolds");
			}
			if (COWTriggerAction.HardDelete == operation)
			{
				try
				{
					StoreObjectId storeObjectId;
					if (settings.IsOnlyInPlaceHoldEnabled())
					{
						settings.Session.CowSession.CheckAndCreateDiscoveryHoldsFolder(callbackContext.SessionWithBestAccess);
						storeObjectId = dumpster.RecoverableItemsDiscoveryHoldsFolderId;
					}
					else
					{
						storeObjectId = dumpster.RecoverableItemsPurgesFolderId;
					}
					if (!settings.CurrentFolderId.Equals(storeObjectId))
					{
						dumpster.MoveItemsToDumpster(callbackContext.SessionWithBestAccess, storeObjectId, itemIds);
					}
					return;
				}
				catch (DumpsterOperationException)
				{
					if (dumpster.Results.AnyPartialResultFailure())
					{
						throw;
					}
					List<GroupOperationResult> partialResults = dumpster.Results.GetPartialResults();
					ExTraceGlobals.SessionTracer.TraceWarning<GroupOperationResult>((long)dumpster.StoreSession.GetHashCode(), "DumpsterOperationException during HardDelete and Partial success: leave the current results {0}", partialResults[partialResults.Count - 1]);
					return;
				}
			}
			dumpster.CopyItemsToDumpster(callbackContext.SessionWithBestAccess, dumpster.RecoverableItemsPurgesFolderId, itemIds, COWTriggerAction.DoneWithMessageDelete == operation);
		}
	}
}

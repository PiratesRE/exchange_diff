using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class COWCalendarLogging : ICOWNotification
	{
		public bool SkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(settings, "settings");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(dumpster, "dumpster");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			if (onDumpster)
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)session.GetHashCode(), "Skipping calendar COW item operation for user {0}, since the operation is on dumpster", session.UserLegacyDN);
				return true;
			}
			if (!CalendarLoggingHelper.ShouldLog(operation))
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string, COWTriggerAction>((long)session.GetHashCode(), "Skipping calendar COW item operation for user {0}, since the trigger action {1} is not interesting", session.UserLegacyDN, operation);
				return true;
			}
			if (!onBeforeNotification && operation != COWTriggerAction.Update)
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)session.GetHashCode(), "Skipping calendar COW item operation for user {0}, since we are only interested in on-before notifications and update trigger actions", session.UserLegacyDN);
				return true;
			}
			if (COWSettings.IsImapPoisonMessage(onBeforeNotification, operation, session, item))
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)session.GetHashCode(), "Skipping calendar COW item operation for user {0}, since the items is marked as IMAP poison message", session.UserLegacyDN);
				return true;
			}
			if (!settings.IsCalendarLoggingEnabled())
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)session.GetHashCode(), "Skipping calendar COW item operation for user {0}, since calendar logging is disabled for this user", session.UserLegacyDN);
				return true;
			}
			return false;
		}

		public void ItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, CoreFolder folder, bool onBeforeNotification, OperationResult result, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(dumpster, "dumpster");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			MailboxSession sessionWithBestAccess = callbackContext.SessionWithBestAccess;
			if (onBeforeNotification)
			{
				if (CalendarLoggingHelper.ShouldLog(item, operation))
				{
					StoreObjectId storeObjectId = ((ICoreObject)item).StoreObjectId;
					CalendarLoggingHelper.AddMetadata(item, operation, null);
					if (operation == COWTriggerAction.Update && state == COWTriggerActionState.Save && CalendarLoggingHelper.ShouldBeCopiedOnWrite(storeObjectId))
					{
						if (settings.HoldEnabled() && item.IsLegallyDirty)
						{
							return;
						}
						if (!settings.IsCurrentFolderItemEnabled(sessionWithBestAccess, item) && !COWCalendarLogging.IsParkedMessagesFolder(settings, sessionWithBestAccess))
						{
							return;
						}
						if (dumpster.IsDumpsterOverCalendarLoggingQuota(sessionWithBestAccess, settings))
						{
							ExTraceGlobals.CalendarLoggingTracer.Information<string, string>((long)session.GetHashCode(), "User {0} has exceeded the calendar logging quota of {1}", session.UserLegacyDN, settings.CalendarLoggingQuota.Value.ToString("A"));
							StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_COWCalendarLoggingStopped, session.UserLegacyDN, new object[]
							{
								session.UserLegacyDN
							});
						}
						else if (dumpster.IsDumpsterOverWarningQuota(settings))
						{
							ExTraceGlobals.CalendarLoggingTracer.Information<string, string>((long)session.GetHashCode(), "Disabling calendar logging for user {0}, since it has exceeded the dumpster warning quota of {1}", session.UserLegacyDN, settings.DumpsterWarningQuota.Value.ToString("A"));
							dumpster.DisableCalendarLogging();
						}
						else
						{
							StoreObjectId calendarLogGeneratedId = this.PerformCopyOnWrite(sessionWithBestAccess, dumpster, storeObjectId);
							dumpster.Results.CalendarLogGeneratedId = calendarLogGeneratedId;
						}
					}
					COWSettings.AddMetadata(settings, item, operation);
					return;
				}
			}
			else if (operation == COWTriggerAction.Update && state == COWTriggerActionState.Save && result == OperationResult.Failed)
			{
				dumpster.RollbackItemVersion(sessionWithBestAccess, item, dumpster.Results.CalendarLogGeneratedId);
				dumpster.Results.CalendarLogGeneratedId = null;
			}
		}

		public CowClientOperationSensitivity SkipGroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, bool onBeforeNotification, bool onDumpster, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			Util.ThrowOnNullArgument(settings, "settings");
			Util.ThrowOnNullArgument(sourceSession, "sourceSession");
			Util.ThrowOnNullArgument(dumpster, "dumpster");
			if (onDumpster)
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)sourceSession.GetHashCode(), "Skipping calendar COW group operation for user {0}, since the operation is on dumpster", sourceSession.UserLegacyDN);
				return CowClientOperationSensitivity.Skip;
			}
			if (!onBeforeNotification)
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)sourceSession.GetHashCode(), "Skipping calendar COW group operation for user {0}, since we are only interested in on-before notifications", sourceSession.UserLegacyDN);
				return CowClientOperationSensitivity.Skip;
			}
			if (!CalendarLoggingHelper.ShouldLog(operation))
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string, COWTriggerAction>((long)sourceSession.GetHashCode(), "Skipping calendar COW group operation for user {0}, since the trigger action {1} is not interesting", sourceSession.UserLegacyDN, operation);
				return CowClientOperationSensitivity.Skip;
			}
			if (DumpsterFolderHelper.IsAuditFolder(callbackContext.SessionWithBestAccess, sourceFolderId))
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)sourceSession.GetHashCode(), "Skipping calendar COW group operation for user {0}, since the operation is on audit folder", sourceSession.UserLegacyDN);
				return CowClientOperationSensitivity.Skip;
			}
			if (!settings.IsCalendarLoggingEnabled())
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)sourceSession.GetHashCode(), "Skipping calendar COW group operation for user {0}, since calendar logging is disabled for this user", sourceSession.UserLegacyDN);
				return CowClientOperationSensitivity.Skip;
			}
			if (!DumpsterFolderHelper.IsDumpsterFolder(callbackContext.SessionWithBestAccess, sourceFolderId))
			{
				return CowClientOperationSensitivity.Capture;
			}
			if (operation == COWTriggerAction.HardDelete && !sourceFolderId.Equals(dumpster.CalendarLoggingFolderId))
			{
				return CowClientOperationSensitivity.Capture;
			}
			ExTraceGlobals.CalendarLoggingTracer.Information<string>((long)sourceSession.GetHashCode(), "Skipping calendar COW group operation for user {0}, since we are not interested in hard deletes in calendar logging folder", sourceSession.UserLegacyDN);
			return CowClientOperationSensitivity.Skip;
		}

		public void GroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, bool onBeforeNotification, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			Util.ThrowOnNullArgument(dumpster, "dumpster");
			if (itemIds == null)
			{
				return;
			}
			if (dumpster.IsDumpsterOverCalendarLoggingQuota(callbackContext.SessionWithBestAccess, settings))
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string, string>((long)sourceSession.GetHashCode(), "User {0} has exceeded the calendar logging quota of {1}", sourceSession.UserLegacyDN, settings.CalendarLoggingQuota.Value.ToString("A"));
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_COWCalendarLoggingStopped, sourceSession.UserLegacyDN, new object[]
				{
					sourceSession.UserLegacyDN
				});
				return;
			}
			if (dumpster.IsDumpsterOverWarningQuota(settings))
			{
				ExTraceGlobals.CalendarLoggingTracer.Information<string, string>((long)sourceSession.GetHashCode(), "Disabling calendar logging for user {0}, since it has exceeded the dumpster warning quota of {1}", sourceSession.UserLegacyDN, settings.DumpsterWarningQuota.Value.ToString("A"));
				dumpster.DisableCalendarLogging();
				return;
			}
			foreach (StoreObjectId storeObjectId in itemIds)
			{
				ICoreItem coreItem = null;
				StoragePermanentException ex = null;
				try
				{
					if (CalendarLoggingHelper.ShouldBeCopiedOnWrite(storeObjectId))
					{
						if (CalendarLoggingHelper.ShouldLogInitialCheck(storeObjectId, operation))
						{
							try
							{
								coreItem = CoreItem.Bind(callbackContext.SessionWithBestAccess, storeObjectId, CalendarLoggingHelper.RequiredOriginalProperties);
								if (!CalendarLoggingHelper.ShouldLog(coreItem, operation))
								{
									goto IL_25A;
								}
								if (coreItem.PropertyBag.GetValueOrDefault<bool>(InternalSchema.HasBeenSubmitted))
								{
									ExTraceGlobals.CalendarLoggingTracer.TraceWarning<ICoreItem, COWTriggerAction>((long)callbackContext.SessionWithBestAccess.GetHashCode(), "Save Item for Calendar Logging skipped as the item.HasBeenSubmitted is true (item {0}, operation {1}.", coreItem, operation);
									goto IL_25A;
								}
								switch (operation)
								{
								case COWTriggerAction.Move:
								case COWTriggerAction.MoveToDeletedItems:
								case COWTriggerAction.SoftDelete:
									if (!this.PerformFolderCopyOnWrite(settings, dumpster, coreItem, callbackContext.SessionWithBestAccess, operation, flags, false))
									{
										goto IL_25A;
									}
									break;
								case COWTriggerAction.HardDelete:
								{
									StoreObjectId parentIdFromMessageId = IdConverter.GetParentIdFromMessageId(storeObjectId);
									if (DumpsterFolderHelper.IsAuditFolder(callbackContext.SessionWithBestAccess, parentIdFromMessageId))
									{
										goto IL_25A;
									}
									if (DumpsterFolderHelper.IsDumpsterFolder(callbackContext.SessionWithBestAccess, parentIdFromMessageId))
									{
										this.PerformCopyOnWrite(callbackContext.SessionWithBestAccess, dumpster, storeObjectId);
										goto IL_25A;
									}
									if (!this.PerformFolderCopyOnWrite(settings, dumpster, coreItem, callbackContext.SessionWithBestAccess, operation, flags, !settings.HoldEnabled()))
									{
										goto IL_25A;
									}
									break;
								}
								}
							}
							catch (ObjectNotFoundException ex2)
							{
								ex = ex2;
							}
							catch (VirusDetectedException ex3)
							{
								ex = ex3;
							}
							catch (VirusMessageDeletedException ex4)
							{
								ex = ex4;
							}
							catch (VirusException ex5)
							{
								ex = ex5;
							}
							if (ex != null)
							{
								ExTraceGlobals.CalendarLoggingTracer.TraceWarning<StoreObjectId, StoragePermanentException, COWTriggerAction>((long)callbackContext.SessionWithBestAccess.GetHashCode(), "Item ({0}) processing for Calendar Logging failure {1} (operation {2}).", storeObjectId, ex, operation);
							}
						}
					}
				}
				finally
				{
					if (coreItem != null)
					{
						coreItem.Dispose();
					}
				}
				IL_25A:;
			}
		}

		private static bool IsParkedMessagesFolder(COWSettings settings, MailboxSession sessionWithBestAccess)
		{
			return settings.CurrentFolderId.Equals(sessionWithBestAccess.GetDefaultFolderId(DefaultFolderType.ParkedMessages));
		}

		private void SaveItem(ICoreItem item, StoreSession session, COWTriggerAction operation)
		{
			item.SetEnableFullValidation(false);
			try
			{
				ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.FailOnAnyConflict);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					ExTraceGlobals.CalendarLoggingTracer.TraceWarning<int, ICoreItem, COWTriggerAction>((long)session.GetHashCode(), "Save Item for Calendar Logging failure ({0} conflicts, item {1}, operation {2}).", (conflictResolutionResult.PropertyConflicts == null) ? 0 : conflictResolutionResult.PropertyConflicts.Length, item, operation);
				}
			}
			catch (QuotaExceededException arg)
			{
				ExTraceGlobals.CalendarLoggingTracer.TraceWarning<QuotaExceededException, ICoreItem, COWTriggerAction>((long)session.GetHashCode(), "Save Item for Calendar Logging failure due to exceeded quota (exception {0}, item {1}, operation {2}).", arg, item, operation);
			}
		}

		private bool PerformFolderCopyOnWrite(COWSettings settings, IDumpsterItemOperations dumpster, ICoreItem item, MailboxSession sessionWithBestAccess, COWTriggerAction operation, FolderChangeOperationFlags folderChangeOperationFlags, bool copyAfterSave)
		{
			bool result = false;
			if (settings.IsCurrentFolderEnabled(sessionWithBestAccess) || COWCalendarLogging.IsParkedMessagesFolder(settings, sessionWithBestAccess))
			{
				StoreObjectId storeObjectId = item.StoreObjectId;
				this.PerformCopyOnWrite(sessionWithBestAccess, dumpster, storeObjectId);
				item.OpenAsReadWrite();
				CalendarLoggingHelper.AddMetadata(item, operation, new FolderChangeOperationFlags?(folderChangeOperationFlags));
				COWSettings.AddMetadata(settings, item, operation);
				this.SaveItem(item, sessionWithBestAccess, operation);
				if (copyAfterSave)
				{
					this.PerformCopyOnWrite(sessionWithBestAccess, dumpster, storeObjectId);
				}
				result = true;
			}
			return result;
		}

		private StoreObjectId PerformCopyOnWrite(MailboxSession sessionWithBestAccess, IDumpsterItemOperations dumpster, StoreObjectId storeObjectId)
		{
			return CoreCalendarItemVersion.CreateVersion(sessionWithBestAccess, storeObjectId, dumpster.CalendarLoggingFolderId);
		}
	}
}

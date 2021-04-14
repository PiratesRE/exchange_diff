using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class COWLogProtector : ICOWNotification
	{
		public bool SkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			if (onBeforeNotification && COWTriggerAction.Update == operation)
			{
				if (settings.CurrentFolderId != null)
				{
					if (settings.CurrentFolderId.Equals(dumpster.AuditsFolderId))
					{
						throw new AccessDeniedException(ServerStrings.ExAuditsUpdateDenied);
					}
					if (settings.CurrentFolderId.Equals(dumpster.AdminAuditLogsFolderId))
					{
						throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsUpdateDenied);
					}
					if (dumpster.IsAuditFolder(settings.CurrentFolderId))
					{
						throw new AccessDeniedException((dumpster.AuditsFolderId != null) ? ServerStrings.ExAuditsUpdateDenied : ServerStrings.ExAdminAuditLogsUpdateDenied);
					}
				}
				else if (itemId != null)
				{
					StoreObjectId parentIdFromMessageId = IdConverter.GetParentIdFromMessageId(itemId);
					if (parentIdFromMessageId.Equals(dumpster.AuditsFolderId))
					{
						throw new AccessDeniedException(ServerStrings.ExAuditsUpdateDenied);
					}
					if (parentIdFromMessageId.Equals(dumpster.AdminAuditLogsFolderId))
					{
						throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsUpdateDenied);
					}
					if (dumpster.IsAuditFolder(settings.CurrentFolderId))
					{
						throw new AccessDeniedException((dumpster.AuditsFolderId != null) ? ServerStrings.ExAuditsUpdateDenied : ServerStrings.ExAdminAuditLogsUpdateDenied);
					}
				}
			}
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
			MailboxSession mailboxSession = sourceSession as MailboxSession;
			if (mailboxSession == null)
			{
				return CowClientOperationSensitivity.Skip;
			}
			if (onBeforeNotification && (COWTriggerAction.Copy == operation || COWTriggerAction.HardDelete == operation || COWTriggerAction.Move == operation || COWTriggerAction.MoveToDeletedItems == operation || COWTriggerAction.SoftDelete == operation))
			{
				StoreObjectId auditsFolderId = dumpster.AuditsFolderId;
				StoreObjectId adminAuditLogsFolderId = dumpster.AdminAuditLogsFolderId;
				if (settings.CurrentFolderId != null && (COWTriggerAction.HardDelete != operation || LogonType.SystemService != sourceSession.LogonType || !settings.IsMrmAction()))
				{
					this.CheckAccessOnAuditFolders(mailboxSession, settings.CurrentFolderId, dumpster, false);
				}
				if (itemIds != null)
				{
					foreach (StoreObjectId storeObjectId in itemIds)
					{
						if (storeObjectId != null)
						{
							if (storeObjectId.IsMessageId)
							{
								if (settings.CurrentFolderId == null && (COWTriggerAction.HardDelete != operation || LogonType.SystemService != sourceSession.LogonType || !settings.IsMrmAction()))
								{
									StoreObjectId parentIdFromMessageId = IdConverter.GetParentIdFromMessageId(storeObjectId);
									if (parentIdFromMessageId.Equals(auditsFolderId))
									{
										throw new AccessDeniedException(ServerStrings.ExAuditsUpdateDenied);
									}
									if (parentIdFromMessageId.Equals(adminAuditLogsFolderId))
									{
										throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsUpdateDenied);
									}
									if (dumpster.IsAuditFolder(parentIdFromMessageId))
									{
										throw new AccessDeniedException((auditsFolderId != null) ? ServerStrings.ExAuditsUpdateDenied : ServerStrings.ExAdminAuditLogsUpdateDenied);
									}
								}
							}
							else if (storeObjectId.IsFolderId)
							{
								this.CheckAccessOnAuditFolders(mailboxSession, storeObjectId, dumpster, true);
							}
						}
					}
				}
			}
			return CowClientOperationSensitivity.Skip;
		}

		private void CheckAccessOnAuditFolders(MailboxSession mailboxSession, StoreObjectId folderId, IDumpsterItemOperations dumpster, bool checkAncestorFolders)
		{
			StoreObjectId auditsFolderId = dumpster.AuditsFolderId;
			StoreObjectId adminAuditLogsFolderId = dumpster.AdminAuditLogsFolderId;
			if (folderId.Equals(auditsFolderId))
			{
				throw new AccessDeniedException(ServerStrings.ExAuditsUpdateDenied);
			}
			if (folderId.Equals(adminAuditLogsFolderId))
			{
				throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsUpdateDenied);
			}
			if (dumpster.IsAuditFolder(folderId))
			{
				throw new AccessDeniedException((auditsFolderId != null) ? ServerStrings.ExAuditsUpdateDenied : ServerStrings.ExAdminAuditLogsUpdateDenied);
			}
			if (checkAncestorFolders && (folderId.Equals(dumpster.RecoverableItemsRootFolderId) || folderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration))))
			{
				if (auditsFolderId != null)
				{
					throw new AccessDeniedException(ServerStrings.ExAuditsUpdateDenied);
				}
				if (adminAuditLogsFolderId != null)
				{
					throw new AccessDeniedException(ServerStrings.ExAdminAuditLogsUpdateDenied);
				}
			}
		}

		public void GroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, bool onBeforeNotification, CallbackContext callbackContext)
		{
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
		}
	}
}

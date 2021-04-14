using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class COWAudit : ICOWNotification
	{
		public bool SkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(settings, "settings");
			Util.ThrowOnNullArgument(callbackContext, "callbackContext");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			bool skipItemOperation = true;
			this.HandleExceptionsAtEntries(delegate
			{
				skipItemOperation = this.InternalSkipItemOperation(settings, dumpster, operation, state, session, itemId, item, onBeforeNotification, onDumpster, success, callbackContext);
			}, session, callbackContext, onBeforeNotification, "SkipItemOperation");
			return skipItemOperation;
		}

		internal bool InternalSkipItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, bool onBeforeNotification, bool onDumpster, bool success, CallbackContext callbackContext)
		{
			ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation operation = {0}, session = {1}, itemId = {2}, item = {3}, onBeforeNotification = {4}", new object[]
			{
				operation,
				session,
				itemId,
				item,
				onBeforeNotification
			});
			if (!onBeforeNotification && callbackContext.AuditSkippedOnBefore != null)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<bool>((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns {0} from AuditSkippedOnBefore value", callbackContext.AuditSkippedOnBefore.Value);
				return callbackContext.AuditSkippedOnBefore.Value;
			}
			if (state == COWTriggerActionState.Flush && (operation == COWTriggerAction.Create || operation == COWTriggerAction.Update))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since COWTriggerActionState is Flush and operation is Create or Update");
				if (onBeforeNotification)
				{
					callbackContext.AuditSkippedOnBefore = new bool?(true);
				}
				return true;
			}
			if (itemId is OccurrenceStoreObjectId)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since it is an occurence item, all operations on the master copy (i.e. recurrence series) are audited.");
				return true;
			}
			if (onBeforeNotification)
			{
				callbackContext.AuditSkippedOnBefore = new bool?(true);
				if (operation == COWTriggerAction.Create || COWTriggerAction.Update == operation || COWTriggerAction.Submit == operation)
				{
					callbackContext.ItemOperationAuditInfo = new ItemAuditInfo((item.Id == null) ? null : item.Id.ObjectId, settings.CurrentFolderId, null, item.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string, item.PropertyBag.TryGetProperty(ItemSchema.From) as Participant, item.PropertyBag.GetValueOrDefault<bool>(InternalSchema.IsAssociated), item.GetLegallyDirtyProperties());
				}
				else if (COWTriggerAction.ItemBind == operation)
				{
					callbackContext.ItemOperationAuditInfo = new ItemAuditInfo(null, settings.CurrentFolderId, null, null);
				}
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since session is not MailboxSession.");
				return true;
			}
			if ((COWTriggerAction.Submit == operation || operation == COWTriggerAction.Create || COWTriggerAction.Update == operation) && callbackContext.ItemOperationAuditInfo != null && callbackContext.ItemOperationAuditInfo.IsAssociated)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since it is FAI message.");
				return true;
			}
			if (!onBeforeNotification && COWTriggerAction.ItemBind == operation && item != null && item.PropertyBag.GetValueOrDefault<bool>(InternalSchema.IsAssociated))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since it is FAI message.");
				return true;
			}
			if (operation == COWTriggerAction.Update && !item.IsLegallyDirty)
			{
				return true;
			}
			IExchangePrincipal exchangePrincipal = mailboxSession.MailboxOwner;
			if (onBeforeNotification && (COWTriggerAction.Submit == operation || COWTriggerAction.ItemBind == operation || operation == COWTriggerAction.Create))
			{
				callbackContext.AuditSkippedOnBefore = null;
				return true;
			}
			if (!onBeforeNotification && operation == COWTriggerAction.Submit && !success)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since it is a failed Submit operation.");
				return true;
			}
			if (operation == COWTriggerAction.ItemBind && itemId.ObjectType != StoreObjectType.Message)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since it is binding a non-message item.");
				return true;
			}
			if (LogonType.SystemService == mailboxSession.LogonType || LogonType.Transport == mailboxSession.LogonType)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<LogonType>((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since mailbox session logon type is not being audited. LogonType = {0}", mailboxSession.LogonType);
				return true;
			}
			if (!onBeforeNotification && COWTriggerAction.Submit == operation)
			{
				exchangePrincipal = COWAudit.GetSubmitEffectiveMailboxOwner(mailboxSession, callbackContext);
				if (exchangePrincipal == null)
				{
					ExTraceGlobals.SessionTracer.TraceDebug<string>((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since resolving effective mailbox owner has failed. From = {0}", (callbackContext.ItemOperationAuditInfo == null || null == callbackContext.ItemOperationAuditInfo.From) ? string.Empty : callbackContext.ItemOperationAuditInfo.From.EmailAddress);
					return true;
				}
			}
			if (!COWAudit.GetEffectiveAuditEnabled(mailboxSession, exchangePrincipal))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since auditing is disabled on the effective mailbox owner.effectiveMailboxOwner = {0}", exchangePrincipal);
				return true;
			}
			string clientInfoString = COWAudit.GetClientInfoString(mailboxSession);
			if (COWAudit.BypassAuditApplicationType(mailboxSession.LogonType, clientInfoString))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<LogonType, string>((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since the client application is bypassing auditing. LogonType = {0}, ClientInfoString = {1}", mailboxSession.LogonType, clientInfoString);
				return true;
			}
			if (COWAudit.UnsupportedMailboxVersion(exchangePrincipal))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal, int>((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since the version of the effective mailbox is not supported for auditing. effectiveMailboxOwner = {0}, ServerVersion = {1}", exchangePrincipal, exchangePrincipal.MailboxInfo.Location.ServerVersion);
				return true;
			}
			if (!onBeforeNotification && COWTriggerAction.Submit == operation)
			{
				item.PropertyBag.Load(COWAudit.ItemPropertiesToLoad);
				COWAudit.SetSubmitAuditOperation(item, callbackContext);
				if (callbackContext.SubmitAuditOperation == MailboxAuditOperations.None)
				{
					ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since the Submit operation is neither SendAs nor SendOnBehalf.");
					return true;
				}
			}
			LogonType logonType = COWAudit.ResolveEffectiveLogonType(mailboxSession, new COWTriggerAction?(operation), callbackContext);
			MailboxAuditOperations mailboxAuditOperations = COWAudit.AuditOperationFromCOWAction(operation, callbackContext.SubmitAuditOperation);
			if (COWAudit.FilterOnMailboxAuditOperation(logonType, exchangePrincipal, mailboxSession, mailboxAuditOperations))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since the operation is not being audited. effectiveLogonType = {0}, effectiveMailboxOwner = {1}, auditOperation = {2}, AuditAdminOperations = {3}, AuditDelegateOperations = {4}, AuditDelegateAdminOperations = {5}, AuditOwnerOperations = {6}", new object[]
				{
					logonType,
					exchangePrincipal,
					mailboxAuditOperations,
					COWAudit.GetEffectiveAuditAdminOperations(mailboxSession, exchangePrincipal),
					COWAudit.GetEffectiveAuditDelegateOperations(mailboxSession, exchangePrincipal),
					exchangePrincipal.MailboxInfo.Configuration.AuditDelegateAdminOperations,
					COWAudit.GetEffectiveAuditOwnerOperations(mailboxSession, exchangePrincipal)
				});
				return true;
			}
			if (!onBeforeNotification && operation == COWTriggerAction.Create && success)
			{
				item.PropertyBag.Load(COWAudit.CreateOperationLoadProperties);
				if (item.Id != null)
				{
					callbackContext.ItemOperationAuditInfo.Id = item.Id.ObjectId;
					if (settings.CurrentFolderId == null && IdConverter.IsMessageId(item.Id.ObjectId.ProviderLevelItemId))
					{
						settings.CurrentFolderId = IdConverter.GetParentIdFromMessageId(item.Id.ObjectId);
					}
				}
			}
			if (mailboxSession.CowSession.SkipAuditingFolderOperations(mailboxAuditOperations, settings.CurrentFolderId))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since the operation on the current folder is excluded from auditing.");
				return true;
			}
			if (COWAudit.FilterBypassAudit(mailboxSession, exchangePrincipal.MailboxInfo.OrganizationId))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since the logon user is bypassing auditing.");
				return true;
			}
			if (settings.IsCurrentFolderExcludedFromAuditing(callbackContext.SessionWithBestAccess))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns true since the current folder is excluded from auditing.");
				return true;
			}
			if (onBeforeNotification)
			{
				callbackContext.AuditSkippedOnBefore = new bool?(false);
			}
			ExTraceGlobals.SessionTracer.TraceDebug((long)session.GetHashCode(), "COWAudit::InternalSkipItemOperation returns false");
			return false;
		}

		public void ItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, COWTriggerActionState state, StoreSession session, StoreObjectId itemId, CoreItem item, CoreFolder folder, bool onBeforeNotification, OperationResult result, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(callbackContext, "callbackContext");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			EnumValidator.ThrowIfInvalid<COWTriggerActionState>(state, "state");
			this.HandleExceptionsAtEntries(delegate
			{
				this.InternalItemOperation(settings, dumpster, operation, session, itemId, item, folder, onBeforeNotification, result, callbackContext);
			}, session, callbackContext, onBeforeNotification, "ItemOperation");
		}

		internal void InternalItemOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, StoreSession session, StoreObjectId itemId, CoreItem item, CoreFolder folder, bool onBeforeNotification, OperationResult result, CallbackContext callbackContext)
		{
			if (onBeforeNotification)
			{
				if (callbackContext.ItemOperationAuditInfo != null && callbackContext.ItemOperationAuditInfo.ParentFolderPathName == null)
				{
					string text = null;
					if (settings.CurrentFolderId != null && settings.GetCurrentFolder(callbackContext.SessionWithBestAccess) != null)
					{
						Exception ex = null;
						try
						{
							Folder currentFolder = settings.GetCurrentFolder(callbackContext.SessionWithBestAccess);
							if (currentFolder != null)
							{
								try
								{
									text = (currentFolder.TryGetProperty(FolderSchema.FolderPathName) as string);
								}
								catch (NotInBagPropertyErrorException)
								{
									ExTraceGlobals.SessionTracer.TraceError<COWTriggerAction>((long)session.GetHashCode(), "[COWAudit::InternalItemOperation] failed to get FolderPathName property of the current folder for operation {0}", operation);
								}
								if (text != null)
								{
									text = text.Replace(COWSettings.StoreIdSeparator, '\\');
								}
							}
						}
						catch (StoragePermanentException ex2)
						{
							ex = ex2;
						}
						catch (StorageTransientException ex3)
						{
							ex = ex3;
						}
						if (ex != null)
						{
							ExTraceGlobals.SessionTracer.TraceError<COWTriggerAction>((long)session.GetHashCode(), "[COWAudit::InternalItemOperation] failed to bind to the current folder for operation {0}", operation);
						}
					}
					callbackContext.ItemOperationAuditInfo.ParentFolderPathName = text;
				}
				return;
			}
			if (operation == COWTriggerAction.FolderBind && folder == null)
			{
				return;
			}
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				return;
			}
			FolderBindHistoryManager folderBindHistoryManager = null;
			if (COWTriggerAction.FolderBind == operation && folder != null)
			{
				LogonType logonType = COWAudit.ResolveEffectiveLogonType(mailboxSession, new COWTriggerAction?(operation), callbackContext);
				if (logonType != LogonType.Owner)
				{
					folderBindHistoryManager = new FolderBindHistoryManager(folder);
					if (folderBindHistoryManager.ShouldSkipAudit)
					{
						return;
					}
				}
			}
			MailboxSession mailboxSession2;
			if (COWTriggerAction.Submit == operation)
			{
				mailboxSession2 = COWAudit.GetSubmitEffectiveMailboxSession(mailboxSession, callbackContext);
				if (mailboxSession2 == null)
				{
					return;
				}
				if (mailboxSession2 == mailboxSession)
				{
					mailboxSession2 = settings.Session;
				}
			}
			else
			{
				mailboxSession2 = callbackContext.SessionWithBestAccess;
			}
			if (result != OperationResult.Failed)
			{
				LogonType effectiveLogonType = COWAudit.ResolveEffectiveLogonType(mailboxSession, new COWTriggerAction?(operation), callbackContext);
				MailboxAuditOperations auditOperation = COWAudit.AuditOperationFromCOWAction(operation, callbackContext.SubmitAuditOperation);
				bool externalAccess = COWAudit.IsExternalAccess(mailboxSession.MailboxOwner, mailboxSession, effectiveLogonType);
				this.InternalAuditOperation(operation, mailboxSession2, mailboxSession, auditOperation, settings, result, effectiveLogonType, externalAccess, itemId, item, callbackContext);
				COWAudit.CheckAndUpdateLastAccessTimestamps(effectiveLogonType, externalAccess, mailboxSession2);
				if (folderBindHistoryManager != null)
				{
					folderBindHistoryManager.UpdateHistory(callbackContext);
				}
			}
		}

		public CowClientOperationSensitivity SkipGroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, bool onBeforeNotification, bool onDumpster, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(settings, "settings");
			Util.ThrowOnNullArgument(sourceSession, "sourceSession");
			Util.ThrowOnNullArgument(callbackContext, "callbackContext");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			CowClientOperationSensitivity result = CowClientOperationSensitivity.Skip;
			this.HandleExceptionsAtEntries(delegate
			{
				result = this.InternalSkipGroupOperation(settings, dumpster, operation, flags, sourceSession, destinationSession, sourceFolderId, destinationFolderId, itemIds, onBeforeNotification, onDumpster, callbackContext);
			}, sourceSession, callbackContext, onBeforeNotification, "SkipGroupOperation");
			return result;
		}

		internal CowClientOperationSensitivity InternalSkipGroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, bool onBeforeNotification, bool onDumpster, CallbackContext callbackContext)
		{
			ExTraceGlobals.SessionTracer.TraceDebug((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation operation = {0}, sourceSession = {1}, destinationSession = {2}, sourceFolderId = {3}, destinationFolderId = {4}, onBeforeNotification = {5}", new object[]
			{
				operation,
				sourceSession,
				destinationSession,
				sourceFolderId,
				destinationFolderId,
				onBeforeNotification
			});
			if (!onBeforeNotification && callbackContext.AuditSkippedOnBefore != null)
			{
				CowClientOperationSensitivity cowClientOperationSensitivity = callbackContext.AuditSkippedOnBefore.Value ? CowClientOperationSensitivity.Skip : CowClientOperationSensitivity.Capture;
				ExTraceGlobals.SessionTracer.TraceDebug<CowClientOperationSensitivity>((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns {0} from AuditSkippedOnBefore value", cowClientOperationSensitivity);
				return cowClientOperationSensitivity;
			}
			if (onBeforeNotification)
			{
				callbackContext.AuditSkippedOnBefore = new bool?(true);
			}
			MailboxSession mailboxSession = sourceSession as MailboxSession;
			if (mailboxSession == null)
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since session is not MailboxSession.");
				return CowClientOperationSensitivity.Skip;
			}
			if (LogonType.SystemService == mailboxSession.LogonType || LogonType.Transport == mailboxSession.LogonType)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<LogonType>((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since mailbox session logon type is not being audited. LogonType = {0}", mailboxSession.LogonType);
				return CowClientOperationSensitivity.Skip;
			}
			if (!COWAudit.GetEffectiveAuditEnabled(mailboxSession, mailboxSession.MailboxOwner))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal>((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since auditing is disabled on the mailbox owner. MailboxOwner = {0}", mailboxSession.MailboxOwner);
				return CowClientOperationSensitivity.Skip;
			}
			string clientInfoString = COWAudit.GetClientInfoString(mailboxSession);
			if (COWAudit.BypassAuditApplicationType(mailboxSession.LogonType, clientInfoString))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<LogonType, string>((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since the client application is bypassing auditing. LogonType = {0}, ClientInfoString = {1}", mailboxSession.LogonType, clientInfoString);
				return CowClientOperationSensitivity.Skip;
			}
			if (COWAudit.UnsupportedMailboxVersion(mailboxSession.MailboxOwner))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<IExchangePrincipal, int>((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since the version of the effective mailbox is not supported for auditing. MailboxOwner = {0}, ServerVersion = {1}", mailboxSession.MailboxOwner, mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion);
				return CowClientOperationSensitivity.Skip;
			}
			operation = COWAudit.TransformOperationIfNecessary(operation, mailboxSession, destinationSession, destinationFolderId);
			LogonType logonType = COWAudit.ResolveEffectiveLogonType(mailboxSession, new COWTriggerAction?(operation), callbackContext);
			MailboxAuditOperations mailboxAuditOperations = COWAudit.AuditOperationFromCOWAction(operation, callbackContext.SubmitAuditOperation);
			if (COWAudit.FilterOnMailboxAuditOperation(logonType, mailboxSession.MailboxOwner, mailboxSession, mailboxAuditOperations))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since the operation is not being audited. effectiveLogonType = {0}, MailboxOwner = {1}, auditOperation = {2}, AuditAdminOperations = {3}, AuditDelegateOperations = {4}, AuditDelegateAdminOperations = {5}, AuditOwnerOperations = {6}", new object[]
				{
					logonType,
					mailboxSession.MailboxOwner,
					mailboxAuditOperations,
					COWAudit.GetEffectiveAuditAdminOperations(mailboxSession, mailboxSession.MailboxOwner),
					COWAudit.GetEffectiveAuditDelegateOperations(mailboxSession, mailboxSession.MailboxOwner),
					mailboxSession.MailboxOwner.MailboxInfo.Configuration.AuditDelegateAdminOperations,
					COWAudit.GetEffectiveAuditOwnerOperations(mailboxSession, mailboxSession.MailboxOwner)
				});
				return CowClientOperationSensitivity.Skip;
			}
			if (operation == COWTriggerAction.Move && sourceSession == destinationSession && sourceFolderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox)) && destinationFolderId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems)))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since the operation is move from Outbox to SentItems.");
				return CowClientOperationSensitivity.Skip;
			}
			if (COWAudit.FilterBypassAudit(mailboxSession, mailboxSession.MailboxOwner.MailboxInfo.OrganizationId))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since the logon user is bypassing auditing.");
				return CowClientOperationSensitivity.Skip;
			}
			if (settings.IsCurrentFolderExcludedFromAuditing(callbackContext.SessionWithBestAccess))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Skip since the current folder is excluded from auditing.");
				return CowClientOperationSensitivity.Skip;
			}
			if (onBeforeNotification)
			{
				callbackContext.AuditSkippedOnBefore = new bool?(false);
			}
			ExTraceGlobals.SessionTracer.TraceDebug((long)sourceSession.GetHashCode(), "COWAudit::InternalSkipGroupOperation returns Capture.");
			return CowClientOperationSensitivity.Capture;
		}

		public void GroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, bool onBeforeNotification, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(settings, "settings");
			Util.ThrowOnNullArgument(dumpster, "dumpster");
			Util.ThrowOnNullArgument(callbackContext, "callbackContext");
			EnumValidator.ThrowIfInvalid<COWTriggerAction>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			this.HandleExceptionsAtEntries(delegate
			{
				this.InternalGroupOperation(settings, dumpster, operation, flags, sourceSession, destinationSession, destinationFolderId, itemIds, result, onBeforeNotification, callbackContext);
			}, sourceSession, callbackContext, onBeforeNotification, "GroupOperation");
		}

		internal void InternalGroupOperation(COWSettings settings, IDumpsterItemOperations dumpster, COWTriggerAction operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, bool onBeforeNotification, CallbackContext callbackContext)
		{
			if (onBeforeNotification)
			{
				string text = null;
				if (settings.CurrentFolderId != null && settings.GetCurrentFolder(callbackContext.SessionWithBestAccess) != null)
				{
					try
					{
						text = (settings.GetCurrentFolder(callbackContext.SessionWithBestAccess).TryGetProperty(FolderSchema.FolderPathName) as string);
					}
					catch (NotInBagPropertyErrorException)
					{
						ExTraceGlobals.SessionTracer.TraceError<COWTriggerAction>((long)sourceSession.GetHashCode(), "[COWAudit::InternalGroupOperation] failed to get FolderPathName property of the current folder for operation {0}", operation);
					}
					if (text != null)
					{
						text = text.Replace('￾', '\\');
					}
					callbackContext.FolderAuditInfo[settings.CurrentFolderId] = new FolderAuditInfo(settings.CurrentFolderId, text);
				}
				if (itemIds != null)
				{
					foreach (StoreObjectId storeObjectId in itemIds)
					{
						if (!callbackContext.ItemAuditInfo.ContainsKey(storeObjectId))
						{
							Exception ex = null;
							string subject = null;
							try
							{
								using (CoreItem coreItem = CoreItem.Bind(sourceSession, storeObjectId, COWAudit.GroupOperationAuditItemProperties))
								{
									try
									{
										subject = (coreItem.PropertyBag.TryGetProperty(ItemSchema.Subject) as string);
									}
									catch (NotInBagPropertyErrorException)
									{
										ExTraceGlobals.SessionTracer.TraceWarning<StoreObjectId, COWTriggerAction>((long)sourceSession.GetHashCode(), "[COWAudit::InternalGroupOperation] item ({0}) has no subject property for auditing (operation {1})", storeObjectId, operation);
									}
								}
							}
							catch (StoragePermanentException ex2)
							{
								ex = ex2;
							}
							catch (StorageTransientException ex3)
							{
								ex = ex3;
							}
							finally
							{
								callbackContext.ItemAuditInfo[storeObjectId] = new ItemAuditInfo(storeObjectId, settings.CurrentFolderId, text, subject);
							}
							if (ex != null)
							{
								ExTraceGlobals.SessionTracer.TraceWarning<StoreObjectId, Exception, COWTriggerAction>((long)sourceSession.GetHashCode(), "Item ({0}) processing for collecting audit information has failure {1} (operation {2}).", storeObjectId, ex, operation);
								ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorBindingMessageItem, storeObjectId.ToString(), new object[]
								{
									storeObjectId,
									((MailboxSession)sourceSession).MailboxOwner.MailboxInfo.PrimarySmtpAddress,
									((MailboxSession)sourceSession).MailboxGuid,
									sourceSession.LogonType,
									IdentityHelper.SidFromLogonIdentity(sourceSession.Identity),
									operation,
									ex
								});
							}
						}
					}
					return;
				}
			}
			else
			{
				MailboxSession mailboxSession = sourceSession as MailboxSession;
				if (mailboxSession != null && result != null && result.OperationResult != OperationResult.Failed)
				{
					operation = COWAudit.TransformOperationIfNecessary(operation, mailboxSession, destinationSession, destinationFolderId);
					MailboxAuditOperations auditOperation = COWAudit.AuditOperationFromCOWAction(operation, callbackContext.SubmitAuditOperation);
					LogonType effectiveLogonType = COWAudit.ResolveEffectiveLogonType(mailboxSession, new COWTriggerAction?(operation), callbackContext);
					bool externalAccess = COWAudit.IsExternalAccess(mailboxSession.MailboxOwner, mailboxSession, effectiveLogonType);
					this.InternalAuditGroupOperation(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, callbackContext);
					COWAudit.CheckAndUpdateLastAccessTimestamps(effectiveLogonType, externalAccess, mailboxSession);
				}
			}
		}

		private static bool IsSharedOwnerAccess(MailboxSession mailboxSession, LogonType effectiveLogonType)
		{
			if (effectiveLogonType != LogonType.Owner)
			{
				return false;
			}
			SecurityIdentifier securityIdentifier = IdentityHelper.SidFromLogonIdentity(mailboxSession.Identity);
			return !securityIdentifier.Equals(mailboxSession.MailboxOwner.Sid) && (null == mailboxSession.MailboxOwner.MasterAccountSid || !securityIdentifier.Equals(mailboxSession.MailboxOwner.MasterAccountSid));
		}

		private static MailboxAuditOperations AuditOperationFromCOWAction(COWTriggerAction operation, MailboxAuditOperations cowSubmitOperation)
		{
			switch (operation)
			{
			case COWTriggerAction.Create:
				return MailboxAuditOperations.Create;
			case COWTriggerAction.Update:
				return MailboxAuditOperations.Update;
			case COWTriggerAction.ItemBind:
				return MailboxAuditOperations.MessageBind;
			case COWTriggerAction.Submit:
				return cowSubmitOperation;
			case COWTriggerAction.Copy:
				return MailboxAuditOperations.Copy;
			case COWTriggerAction.Move:
				return MailboxAuditOperations.Move;
			case COWTriggerAction.MoveToDeletedItems:
				return MailboxAuditOperations.MoveToDeletedItems;
			case COWTriggerAction.SoftDelete:
				return MailboxAuditOperations.SoftDelete;
			case COWTriggerAction.HardDelete:
				return MailboxAuditOperations.HardDelete;
			case COWTriggerAction.DoneWithMessageDelete:
				return MailboxAuditOperations.None;
			case COWTriggerAction.FolderBind:
				return MailboxAuditOperations.FolderBind;
			default:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The specified operation type ('{0}') is not supported by mailbox auditing.", new object[]
				{
					operation
				}));
			}
		}

		private static bool FilterOnMailboxAuditOperation(LogonType logonType, IExchangePrincipal mailboxOwner, MailboxSession mailboxSession, MailboxAuditOperations auditOperation)
		{
			if (auditOperation == MailboxAuditOperations.None)
			{
				return true;
			}
			switch (logonType)
			{
			case LogonType.Owner:
				return (COWAudit.GetEffectiveAuditOwnerOperations(mailboxSession, mailboxOwner) & auditOperation) != auditOperation;
			case LogonType.Admin:
				return (COWAudit.GetEffectiveAuditAdminOperations(mailboxSession, mailboxOwner) & auditOperation) != auditOperation;
			case LogonType.Delegated:
				return (COWAudit.GetEffectiveAuditDelegateOperations(mailboxSession, mailboxOwner) & auditOperation) != auditOperation;
			case LogonType.Transport:
			case LogonType.SystemService:
				return true;
			case LogonType.DelegatedAdmin:
				return (mailboxOwner.MailboxInfo.Configuration.AuditDelegateAdminOperations & auditOperation) != auditOperation;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The specified mailbox session logon type ('{0}') is not supported by mailbox auditing.", new object[]
			{
				logonType
			}));
		}

		private static bool FilterBypassAudit(MailboxSession session, OrganizationId organizationId)
		{
			switch (session.LogonType)
			{
			case LogonType.Owner:
			case LogonType.Admin:
			case LogonType.Delegated:
			case LogonType.DelegatedAdmin:
			{
				SecurityIdentifier effectiveLogonSid = IdentityHelper.GetEffectiveLogonSid(session);
				bool flag = COWAudit.InternalIsUserBypassingAudit(organizationId, effectiveLogonSid);
				if (flag)
				{
					ExTraceGlobals.SessionTracer.TraceDebug<SecurityIdentifier, OrganizationId>((long)session.GetHashCode(), "COWAudit::FilterBypassAudit returns true for user sid {0} in organization {1}", effectiveLogonSid, organizationId);
				}
				return flag;
			}
			case LogonType.Transport:
			case LogonType.SystemService:
				return true;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The specified mailbox session logon type ('{0}') is not supported by mailbox auditing.", new object[]
			{
				session.LogonType
			}));
		}

		private static bool UnsupportedMailboxVersion(IExchangePrincipal mailboxOwner)
		{
			if (mailboxOwner.MailboxInfo.Location.ServerVersion != 0)
			{
				ServerVersion serverVersion = new ServerVersion(mailboxOwner.MailboxInfo.Location.ServerVersion);
				if (serverVersion.Major < StoreSession.CurrentServerMajorVersion)
				{
					return true;
				}
			}
			return false;
		}

		private static string GetNextClientStringSegment(string prefix, string clientInfo)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(clientInfo))
			{
				int length = prefix.Length;
				if (clientInfo.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					int num = clientInfo.IndexOf(';', length);
					if (num == -1)
					{
						num = clientInfo.Length;
					}
					result = clientInfo.Substring(length, num - length);
				}
			}
			return result;
		}

		private static string GetClientApplicationType(string clientInfo)
		{
			return COWAudit.GetNextClientStringSegment("Client=", clientInfo);
		}

		private static string GetManagementActionType(string clientInfo)
		{
			return COWAudit.GetNextClientStringSegment("Client=Management;Action=", clientInfo);
		}

		private static bool BypassManagementClient(string clientInfo)
		{
			string text = COWAudit.GetManagementActionType(clientInfo).ToUpperInvariant();
			string a;
			return (a = text) == null || !(a == "E-DISCOVERY");
		}

		private static bool BypassAuditApplicationType(LogonType logonType, string clientInfo)
		{
			if (logonType == LogonType.Admin || logonType == LogonType.DelegatedAdmin)
			{
				string clientApplicationType = COWAudit.GetClientApplicationType(clientInfo);
				if (!string.IsNullOrEmpty(clientApplicationType))
				{
					string key;
					switch (key = clientApplicationType.ToUpperInvariant())
					{
					case "AS":
					case "EXSEARCHSERVICE":
					case "CI":
					case "ELC":
					case "HUB":
					case "MONITORING":
					case "MSEXCHANGEMAILBOXASSISTANTS":
					case "TBA":
					case "EBA":
					case "TIMEBASED MSEXCHANGEMAILBOXASSISTANTS":
					case "CONVERSATION ASSISTANT":
					case "TIMEBASED":
					case "FREEBUSYPUBLISHINGASSISTANT":
					case "UMPARTNERMESSAGEAGENT":
					case "SYSTEM MANAGEMENT":
					case "NEW-EXCHANGENOTIFICATION":
					case "NOTIFICATIONDATAPROVIDER":
					case "UMPUBLISHINGMAILBOX":
					case "AVAILABILITYSERVICE":
					case "TRANSPORTSYNC":
					case "EVENTBASED MSEXCHANGEMAILBOXASSISTANTS":
					case "MEETING VALIDATOR":
					case "FIXIMAPID":
					case "UM":
						return true;
					case "MANAGEMENT":
						return COWAudit.BypassManagementClient(clientInfo);
					}
					return false;
				}
			}
			return false;
		}

		private static COWTriggerAction TransformOperationIfNecessary(COWTriggerAction originalOperation, MailboxSession sourceSession, StoreSession destinationSession, StoreObjectId destinationFolderId)
		{
			COWTriggerAction resultOperation = originalOperation;
			if (sourceSession.LogonType == LogonType.Delegated && originalOperation == COWTriggerAction.Move && destinationFolderId != null)
			{
				MailboxSession destMailboxSession = destinationSession as MailboxSession;
				if (destMailboxSession != null)
				{
					destMailboxSession.BypassAuditing(delegate
					{
						StoreObjectId defaultFolderId = destMailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
						if (destinationFolderId.Equals(defaultFolderId))
						{
							resultOperation = COWTriggerAction.MoveToDeletedItems;
						}
					});
				}
			}
			return resultOperation;
		}

		private static MailboxSession InternalOpenAdminAccess(MailboxSession masterMailboxSession, ExchangePrincipal principal)
		{
			MailboxSession mailboxSession = null;
			Exception ex = null;
			try
			{
				mailboxSession = MailboxSession.OpenAsAdmin(principal, masterMailboxSession.InternalPreferedCulture, masterMailboxSession.ClientInfoString + ";COW=Audit");
				mailboxSession.ExTimeZone = masterMailboxSession.ExTimeZone;
				mailboxSession.SetClientIPEndpoints(masterMailboxSession.ClientIPAddress, masterMailboxSession.ServerIPAddress);
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.SessionTracer.TraceWarning<ADObjectId, Exception>((long)masterMailboxSession.GetHashCode(), "Failed to open mailbox session for {0} with Admin logon. Error: {1}", principal.ObjectId, ex);
				ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorOpeningParticipantSession, principal.ObjectId.ToString(), new object[]
				{
					principal.MailboxInfo.PrimarySmtpAddress,
					principal.MailboxInfo.MailboxGuid,
					masterMailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress,
					masterMailboxSession.MailboxGuid,
					masterMailboxSession.LogonType,
					IdentityHelper.SidFromLogonIdentity(masterMailboxSession.Identity),
					ex
				});
				if (mailboxSession != null)
				{
					mailboxSession.Dispose();
					mailboxSession = null;
				}
			}
			return mailboxSession;
		}

		private static IExchangePrincipal GetSubmitEffectiveMailboxOwner(MailboxSession session, CallbackContext callbackContext)
		{
			if (callbackContext.SubmitEffectiveMailboxOwner != null)
			{
				return callbackContext.SubmitEffectiveMailboxOwner;
			}
			if (callbackContext.ItemOperationAuditInfo == null || null == callbackContext.ItemOperationAuditInfo.From || callbackContext.ItemOperationAuditInfo.From.EmailAddress == null)
			{
				return session.MailboxOwner;
			}
			Participant from = callbackContext.ItemOperationAuditInfo.From;
			ExchangePrincipal exchangePrincipal = null;
			Exception ex = null;
			try
			{
				if (COWAudit.ExchangePrincipalHasEmailAddress(session.MailboxOwner, from.EmailAddress))
				{
					return session.MailboxOwner;
				}
				try
				{
					exchangePrincipal = ExchangePrincipal.FromLegacyDN(session.GetADSessionSettings(), from.EmailAddress, RemotingOptions.AllowCrossSite);
				}
				catch (ObjectNotFoundException)
				{
					exchangePrincipal = ExchangePrincipal.FromProxyAddress(session.GetADSessionSettings(), from.EmailAddress, RemotingOptions.AllowCrossSite);
				}
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			catch (StorageTransientException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.SessionTracer.TraceWarning<string, Exception>((long)session.GetHashCode(), "From address '{0}' cannot be resolved to an existing Exchange Principal. Exception: '{1}'", from.EmailAddress, ex);
				ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorResolvingFromAddress, session.MailboxOwner.ObjectId.ToString(), new object[]
				{
					from.EmailAddress,
					ex
				});
				return null;
			}
			callbackContext.SubmitEffectiveMailboxOwner = exchangePrincipal;
			return exchangePrincipal;
		}

		private static MailboxSession GetSubmitEffectiveMailboxSession(MailboxSession session, CallbackContext callbackContext)
		{
			if (callbackContext.SubmitEffectiveMailboxSession != null)
			{
				return callbackContext.SubmitEffectiveMailboxSession;
			}
			if (callbackContext.SubmitEffectiveMailboxOwner == null)
			{
				return session;
			}
			MailboxSession mailboxSession = COWAudit.InternalOpenAdminAccess(session, callbackContext.SubmitEffectiveMailboxOwner);
			callbackContext.SubmitEffectiveMailboxSession = mailboxSession;
			return mailboxSession;
		}

		private static void SetSubmitAuditOperation(CoreItem item, CallbackContext callbackContext)
		{
			Util.ThrowOnNullArgument(item, "item");
			MailboxSession mailboxSession = item.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new ArgumentException("Item store session is not a MailboxSession object.", "item");
			}
			object obj = item.PropertyBag.TryGetProperty(ItemSchema.From);
			if (PropertyError.IsPropertyNotFound(obj))
			{
				return;
			}
			SecurityIdentifier effectiveLogonSid = IdentityHelper.GetEffectiveLogonSid(mailboxSession);
			if (effectiveLogonSid != mailboxSession.MailboxOwner.Sid && (null == mailboxSession.MailboxOwner.MasterAccountSid || effectiveLogonSid != mailboxSession.MailboxOwner.MasterAccountSid))
			{
				callbackContext.SubmitAuditOperation = MailboxAuditOperations.SendAs;
			}
			Participant participant = (Participant)obj;
			Participant participant2 = item.PropertyBag.TryGetProperty(ItemSchema.Sender) as Participant;
			bool flag = COWAudit.ExchangePrincipalHasEmailAddress(mailboxSession.MailboxOwner, participant.EmailAddress);
			bool flag2 = null == participant2 || participant2 == participant || participant2.EmailAddress.Equals(participant.EmailAddress, StringComparison.OrdinalIgnoreCase) || (flag && COWAudit.ExchangePrincipalHasEmailAddress(mailboxSession.MailboxOwner, participant2.EmailAddress));
			if (flag && flag2)
			{
				return;
			}
			MailboxAuditOperations mailboxAuditOperations = flag2 ? MailboxAuditOperations.SendAs : MailboxAuditOperations.SendOnBehalf;
			if (mailboxAuditOperations == MailboxAuditOperations.SendOnBehalf)
			{
				string itemClass = item.PropertyBag.TryGetProperty(CoreItemSchema.ItemClass) as string;
				if (ObjectClass.IsMeetingRequest(itemClass))
				{
					mailboxAuditOperations = MailboxAuditOperations.None;
				}
			}
			callbackContext.SubmitAuditOperation = mailboxAuditOperations;
		}

		private static bool ExchangePrincipalHasEmailAddress(IExchangePrincipal exchangePrincipal, string emailAddress)
		{
			return exchangePrincipal.LegacyDn.Equals(emailAddress, StringComparison.OrdinalIgnoreCase) || exchangePrincipal.MailboxInfo.EmailAddresses.Any((ProxyAddress element) => element.AddressString == emailAddress);
		}

		private void HandleExceptionsAtEntries(Action tryCode, StoreSession session, CallbackContext callbackContext, bool onBeforeNotification, string methodName)
		{
			Exception ex = null;
			try
			{
				tryCode();
			}
			catch (Exception ex2)
			{
				ex = ex2;
				throw;
			}
			finally
			{
				if (ex != null)
				{
					if (ExTraceGlobals.SessionTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.SessionTracer.TraceError<string, Exception>((long)session.GetHashCode(), "COWAudit::{0}: Exception {1}", methodName, ex);
					}
					if (onBeforeNotification)
					{
						callbackContext.AuditSkippedOnBefore = new bool?(true);
					}
				}
			}
		}

		internal static LogonType ResolveEffectiveLogonType(MailboxSession mailboxSession, COWTriggerAction? operation, CallbackContext callbackContext)
		{
			if (mailboxSession.RemoteClientSessionInfo != null)
			{
				return mailboxSession.RemoteClientSessionInfo.EffectiveLogonType;
			}
			LogonType logonType = mailboxSession.LogonType;
			if (mailboxSession.IsRemote && logonType == LogonType.BestAccess)
			{
				logonType = LogonType.Owner;
			}
			if (COWAudit.InternalIsEwsImpersonation(mailboxSession, logonType))
			{
				return LogonType.Admin;
			}
			if (!mailboxSession.IsRemote && COWTriggerAction.Submit == operation && logonType == LogonType.Owner && callbackContext != null && callbackContext.SubmitAuditOperation != MailboxAuditOperations.None)
			{
				return LogonType.Delegated;
			}
			if (COWAudit.IsSharedOwnerAccess(mailboxSession, logonType))
			{
				return LogonType.Delegated;
			}
			if (logonType == LogonType.DelegatedAdmin)
			{
				return LogonType.Admin;
			}
			return logonType;
		}

		private static void CheckAndUpdateLastAccessTimestamps(LogonType effectiveLogonType, bool externalAccess, MailboxSession mailboxSession)
		{
			List<ADPropertyDefinition> propertiesToBeUpdated = new List<ADPropertyDefinition>(3);
			IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
			if (!externalAccess)
			{
				if (effectiveLogonType == LogonType.Admin && (mailboxOwner.MailboxInfo.Configuration.AuditLastAdminAccess == null || COWAudit.LastAuditAccessRefreshInterval < DateTime.UtcNow - mailboxOwner.MailboxInfo.Configuration.AuditLastAdminAccess.Value.ToUniversalTime()))
				{
					propertiesToBeUpdated.Add(ADRecipientSchema.AuditLastAdminAccess);
				}
				if (effectiveLogonType == LogonType.Delegated && (mailboxOwner.MailboxInfo.Configuration.AuditLastDelegateAccess == null || COWAudit.LastAuditAccessRefreshInterval < DateTime.UtcNow - mailboxOwner.MailboxInfo.Configuration.AuditLastDelegateAccess.Value.ToUniversalTime()))
				{
					propertiesToBeUpdated.Add(ADRecipientSchema.AuditLastDelegateAccess);
				}
			}
			if (externalAccess && (mailboxOwner.MailboxInfo.Configuration.AuditLastExternalAccess == null || COWAudit.LastAuditAccessRefreshInterval < DateTime.UtcNow - mailboxOwner.MailboxInfo.Configuration.AuditLastExternalAccess.Value.ToUniversalTime()))
			{
				propertiesToBeUpdated.Add(ADRecipientSchema.AuditLastExternalAccess);
			}
			using (ActivityContext.SuppressThreadScope())
			{
				IRecipientSession recipientSession = mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent);
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					ExWatson.SendReportOnUnhandledException(delegate()
					{
						Exception exception = null;
						try
						{
							GrayException.MapAndReportGrayExceptions(delegate()
							{
								try
								{
									COWAudit.UpdateLastAuditAccess(propertiesToBeUpdated.ToArray(), mailboxOwner, recipientSession);
								}
								catch (ADTransientException exception2)
								{
									exception = exception2;
								}
							});
						}
						catch (GrayException exception)
						{
							GrayException exception3;
							exception = exception3;
						}
						if (exception != null)
						{
							ExTraceGlobals.SessionTracer.TraceError<ADObjectId, Exception>(0L, "Failed to update last audit access information for user {0}. Error: {1}", mailboxOwner.ObjectId, exception);
							ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorSavingLastAccessTime, mailboxOwner.ObjectId.ToString(), new object[]
							{
								effectiveLogonType,
								mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
								exception
							});
						}
					}, delegate(object ex)
					{
						ExTraceGlobals.SessionTracer.TraceError<ADObjectId, object>(0L, "Failed to update last audit access information for user {0}. Error: {1}", mailboxOwner.ObjectId, ex);
						ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorSavingLastAccessTime, mailboxOwner.ObjectId.ToString(), new object[]
						{
							effectiveLogonType,
							mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(),
							ex
						});
						return !(ex is GrayException);
					});
				});
			}
		}

		private static bool UserSidIsExternal(SecurityIdentifier logonUserSid, SecurityIdentifier ownerSid)
		{
			if (!logonUserSid.Equals(ownerSid))
			{
				SecurityIdentifier securityIdentifier = null;
				try
				{
					securityIdentifier = logonUserSid.AccountDomainSid;
				}
				catch (ArgumentNullException)
				{
					return true;
				}
				SecurityIdentifier securityIdentifier2 = null;
				try
				{
					securityIdentifier2 = ownerSid.AccountDomainSid;
				}
				catch (ArgumentNullException)
				{
				}
				if (null != securityIdentifier && null != securityIdentifier2 && !securityIdentifier.Equals(securityIdentifier2))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private static void UpdateLastAuditAccess(ADPropertyDefinition[] properties, IExchangePrincipal mailboxOwner, IRecipientSession recipientSession)
		{
			try
			{
				ADUser aduser = recipientSession.Read(mailboxOwner.ObjectId) as ADUser;
				if (aduser != null)
				{
					aduser.propertyBag.SetIsReadOnly(false);
					object[] array = new object[properties.Length];
					DateTime utcNow = DateTime.UtcNow;
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = utcNow;
					}
					aduser.SetProperties(properties, array);
					recipientSession.Save(aduser);
				}
			}
			catch (TenantIsLockedDownForRelocationException)
			{
			}
			catch (TenantIsArrivingException)
			{
			}
		}

		private static bool InternalIsEwsImpersonation(MailboxSession mailboxSession, LogonType effectiveLogonType)
		{
			SecurityIdentifier left = IdentityHelper.SidFromAuxiliaryIdentity(mailboxSession.AuxiliaryIdentity);
			return effectiveLogonType == LogonType.Owner && left != null && left != IdentityHelper.SidFromLogonIdentity(mailboxSession.Identity);
		}

		private void InternalAuditOperation(COWTriggerAction operation, MailboxSession auditingMailboxSession, MailboxSession mailboxSession, MailboxAuditOperations auditOperation, COWSettings settings, OperationResult result, LogonType effectiveLogonType, bool externalAccess, StoreObjectId itemId, CoreItem item, CallbackContext callbackContext)
		{
			if (AuditFeatureManager.IsMailboxAuditLocalQueueEnabled(auditingMailboxSession.MailboxOwner))
			{
				ExchangeMailboxAuditRecord record = AuditRecordFactory.CreateMailboxItemRecord(mailboxSession, auditOperation, settings, result, effectiveLogonType, externalAccess, itemId, item, callbackContext.ItemOperationAuditInfo);
				this.logger.Value.WriteAuditRecord(record);
			}
			else
			{
				ItemOperationAuditEvent auditEvent = new ItemOperationAuditEvent(mailboxSession, auditOperation, settings, result, effectiveLogonType, externalAccess, itemId, item, callbackContext.ItemOperationAuditInfo);
				auditingMailboxSession.AuditMailboxAccess(auditEvent, false);
			}
			COWSession.PerfCounters.AuditItemChangeRate.Increment();
			if (operation == COWTriggerAction.FolderBind)
			{
				COWSession.PerfCounters.AuditFolderBindRate.Increment();
			}
		}

		private void InternalAuditGroupOperation(MailboxSession mailboxSession, MailboxAuditOperations auditOperation, COWSettings settings, LogonType effectiveLogonType, bool externalAccess, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, CallbackContext callbackContext)
		{
			try
			{
				IDictionary<StoreObjectId, FolderAuditInfo> folderAuditInfo = callbackContext.FolderAuditInfo;
				IDictionary<StoreObjectId, ItemAuditInfo> itemAuditInfo = callbackContext.ItemAuditInfo;
				bool flag = itemAuditInfo.Count + folderAuditInfo.Count <= 32;
				if (flag)
				{
					if (AuditFeatureManager.IsMailboxAuditLocalQueueEnabled(mailboxSession.MailboxOwner))
					{
						ExchangeMailboxAuditGroupRecord record = AuditRecordFactory.CreateMailboxGroupRecord(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, folderAuditInfo, itemAuditInfo, folderAuditInfo);
						this.logger.Value.WriteAuditRecord(record);
					}
					else
					{
						GroupOperationAuditEvent auditEvent = new GroupOperationAuditEvent(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, folderAuditInfo, itemAuditInfo, folderAuditInfo);
						callbackContext.SessionWithBestAccess.AuditMailboxAccess(auditEvent, false);
					}
				}
				else
				{
					if (AuditFeatureManager.IsMailboxAuditLocalQueueEnabled(mailboxSession.MailboxOwner))
					{
						using (IEnumerator<ExchangeMailboxAuditGroupRecord> enumerator = COWAudit.SplitHugeEvent<ExchangeMailboxAuditGroupRecord>(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, folderAuditInfo, itemAuditInfo, new Func<MailboxSession, MailboxAuditOperations, COWSettings, LogonType, bool, StoreSession, StoreObjectId, StoreObjectId[], GroupOperationResult, IDictionary<StoreObjectId, FolderAuditInfo>, IDictionary<StoreObjectId, ItemAuditInfo>, IDictionary<StoreObjectId, FolderAuditInfo>, ExchangeMailboxAuditGroupRecord>(AuditRecordFactory.CreateMailboxGroupRecord)).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ExchangeMailboxAuditGroupRecord record2 = enumerator.Current;
								this.logger.Value.WriteAuditRecord(record2);
							}
							goto IL_156;
						}
					}
					foreach (GroupOperationAuditEvent auditEvent2 in COWAudit.SplitHugeEvent<GroupOperationAuditEvent>(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, folderAuditInfo, itemAuditInfo, new Func<MailboxSession, MailboxAuditOperations, COWSettings, LogonType, bool, StoreSession, StoreObjectId, StoreObjectId[], GroupOperationResult, IDictionary<StoreObjectId, FolderAuditInfo>, IDictionary<StoreObjectId, ItemAuditInfo>, IDictionary<StoreObjectId, FolderAuditInfo>, GroupOperationAuditEvent>(COWAudit.CreateGroupOperationAuditEvent)))
					{
						callbackContext.SessionWithBestAccess.AuditMailboxAccess(auditEvent2, false);
					}
				}
				IL_156:;
			}
			finally
			{
				COWSession.PerfCounters.AuditGroupChangeRate.Increment();
			}
		}

		internal static IEnumerable<T> SplitHugeEvent<T>(MailboxSession mailboxSession, MailboxAuditOperations auditOperation, COWSettings settings, LogonType effectiveLogonType, bool externalAccess, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, IDictionary<StoreObjectId, FolderAuditInfo> folders, IDictionary<StoreObjectId, ItemAuditInfo> items, Func<MailboxSession, MailboxAuditOperations, COWSettings, LogonType, bool, StoreSession, StoreObjectId, StoreObjectId[], GroupOperationResult, IDictionary<StoreObjectId, FolderAuditInfo>, IDictionary<StoreObjectId, ItemAuditInfo>, IDictionary<StoreObjectId, FolderAuditInfo>, T> createEvent)
		{
			List<KeyValuePair<StoreObjectId, ItemAuditInfo>> nullGroup;
			Dictionary<StoreObjectId, List<KeyValuePair<StoreObjectId, ItemAuditInfo>>> folderGroups = COWAudit.GroupItemsByFolder(items, out nullGroup);
			Dictionary<StoreObjectId, FolderAuditInfo> allFolders = new Dictionary<StoreObjectId, FolderAuditInfo>();
			Dictionary<StoreObjectId, FolderAuditInfo> emptyFolders = new Dictionary<StoreObjectId, FolderAuditInfo>();
			foreach (KeyValuePair<StoreObjectId, FolderAuditInfo> keyValuePair in folders)
			{
				allFolders.Add(keyValuePair.Key, keyValuePair.Value);
				if (!folderGroups.ContainsKey(keyValuePair.Key))
				{
					emptyFolders.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			if (emptyFolders.Count > 0)
			{
				yield return createEvent(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, emptyFolders, new Dictionary<StoreObjectId, ItemAuditInfo>(), allFolders);
			}
			foreach (KeyValuePair<StoreObjectId, List<KeyValuePair<StoreObjectId, ItemAuditInfo>>> folderGroup in folderGroups)
			{
				Dictionary<StoreObjectId, FolderAuditInfo> folderBunch = new Dictionary<StoreObjectId, FolderAuditInfo>(1);
				KeyValuePair<StoreObjectId, List<KeyValuePair<StoreObjectId, ItemAuditInfo>>> keyValuePair2 = folderGroup;
				StoreObjectId folderId = keyValuePair2.Key;
				FolderAuditInfo folder;
				bool isKnownFolder = allFolders.TryGetValue(folderId, out folder);
				if (isKnownFolder)
				{
					folderBunch.Add(folderId, folder);
				}
				KeyValuePair<StoreObjectId, List<KeyValuePair<StoreObjectId, ItemAuditInfo>>> keyValuePair3 = folderGroup;
				List<KeyValuePair<StoreObjectId, ItemAuditInfo>> folderItems = keyValuePair3.Value;
				foreach (IDictionary<StoreObjectId, ItemAuditInfo> dictionary in COWAudit.BreakIntoChunks(folderItems))
				{
					Dictionary<StoreObjectId, ItemAuditInfo> group = (Dictionary<StoreObjectId, ItemAuditInfo>)dictionary;
					yield return createEvent(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, folderBunch, group, allFolders);
					folderBunch = new Dictionary<StoreObjectId, FolderAuditInfo>(1);
				}
			}
			foreach (IDictionary<StoreObjectId, ItemAuditInfo> group2 in COWAudit.BreakIntoChunks(nullGroup))
			{
				yield return createEvent(mailboxSession, auditOperation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, new Dictionary<StoreObjectId, FolderAuditInfo>(1), group2, allFolders);
			}
			yield break;
		}

		private static IEnumerable<IDictionary<StoreObjectId, ItemAuditInfo>> BreakIntoChunks(IEnumerable<KeyValuePair<StoreObjectId, ItemAuditInfo>> items)
		{
			Dictionary<StoreObjectId, ItemAuditInfo> groupItems = new Dictionary<StoreObjectId, ItemAuditInfo>(32);
			foreach (KeyValuePair<StoreObjectId, ItemAuditInfo> item in items)
			{
				Dictionary<StoreObjectId, ItemAuditInfo> dictionary = groupItems;
				KeyValuePair<StoreObjectId, ItemAuditInfo> keyValuePair = item;
				StoreObjectId key = keyValuePair.Key;
				KeyValuePair<StoreObjectId, ItemAuditInfo> keyValuePair2 = item;
				dictionary.Add(key, keyValuePair2.Value);
				if (groupItems.Count >= 32)
				{
					yield return groupItems;
					groupItems = new Dictionary<StoreObjectId, ItemAuditInfo>(32);
				}
			}
			if (groupItems.Count > 0)
			{
				yield return groupItems;
			}
			yield break;
		}

		private static Dictionary<StoreObjectId, List<KeyValuePair<StoreObjectId, ItemAuditInfo>>> GroupItemsByFolder(IEnumerable<KeyValuePair<StoreObjectId, ItemAuditInfo>> items, out List<KeyValuePair<StoreObjectId, ItemAuditInfo>> nullGroup)
		{
			Dictionary<StoreObjectId, List<KeyValuePair<StoreObjectId, ItemAuditInfo>>> dictionary = new Dictionary<StoreObjectId, List<KeyValuePair<StoreObjectId, ItemAuditInfo>>>();
			nullGroup = new List<KeyValuePair<StoreObjectId, ItemAuditInfo>>();
			foreach (KeyValuePair<StoreObjectId, ItemAuditInfo> item in items)
			{
				StoreObjectId parentFolderId = item.Value.ParentFolderId;
				List<KeyValuePair<StoreObjectId, ItemAuditInfo>> list;
				if (parentFolderId == null)
				{
					list = nullGroup;
				}
				else if (!dictionary.TryGetValue(parentFolderId, out list))
				{
					list = new List<KeyValuePair<StoreObjectId, ItemAuditInfo>>();
					dictionary.Add(parentFolderId, list);
				}
				list.Add(item);
			}
			return dictionary;
		}

		private static bool InternalIsExternalAccess(MailboxSession mailboxSession, SecurityIdentifier logonUserSid)
		{
			return ExternalAccessCache.Instance.IsExternalAccess(mailboxSession.MailboxOwner.MailboxInfo.OrganizationId, logonUserSid);
		}

		private static bool InternalIsUserBypassingAudit(OrganizationId organizationId, SecurityIdentifier sid)
		{
			return BypassAuditCache.Instance.IsUserBypassingAudit(organizationId, sid);
		}

		private static bool IsExternalAccess(IExchangePrincipal mailboxOwner, MailboxSession mailboxSession, LogonType effectiveLogonType)
		{
			SecurityIdentifier effectiveLogonSid = IdentityHelper.GetEffectiveLogonSid(mailboxSession);
			bool result = false;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.DataStorage.CheckExternalAccess.Enabled && effectiveLogonType == LogonType.Admin)
			{
				result = (mailboxSession.RemoteClientSessionInfo == null && (effectiveLogonSid.IsWellKnown(WellKnownSidType.LocalServiceSid) || effectiveLogonSid.IsWellKnown(WellKnownSidType.LocalSystemSid) || effectiveLogonSid.IsWellKnown(WellKnownSidType.NetworkServiceSid) || COWAudit.InternalIsExternalAccess(mailboxSession, effectiveLogonSid)));
			}
			return result;
		}

		private static string GetClientInfoString(MailboxSession mailboxSession)
		{
			if (mailboxSession.RemoteClientSessionInfo == null)
			{
				return mailboxSession.ClientInfoString;
			}
			return mailboxSession.RemoteClientSessionInfo.ClientInfoString;
		}

		private static bool IsUCCPolicyEnabledAndExist(MailboxSession mailboxSession, IExchangePrincipal mailboxOwner, out AuditPolicyCacheEntry cacheEntry)
		{
			cacheEntry = null;
			if (COWAudit.IsAuditConfigFromUCCPolicyEnabled(mailboxSession))
			{
				AuditPolicyUtility.RetrieveAuditPolicy(mailboxOwner.MailboxInfo.OrganizationId, out cacheEntry);
				return cacheEntry != null && cacheEntry.IsExist();
			}
			return false;
		}

		private static bool GetEffectiveAuditEnabled(MailboxSession mailboxSession, IExchangePrincipal mailboxOwner)
		{
			AuditPolicyCacheEntry auditPolicyCacheEntry;
			return COWAudit.IsUCCPolicyEnabledAndExist(mailboxSession, mailboxOwner, out auditPolicyCacheEntry) || mailboxOwner.MailboxInfo.Configuration.IsMailboxAuditEnabled;
		}

		private static MailboxAuditOperations GetEffectiveAuditAdminOperations(MailboxSession mailboxSession, IExchangePrincipal mailboxOwner)
		{
			AuditPolicyCacheEntry auditPolicyCacheEntry;
			if (!COWAudit.IsUCCPolicyEnabledAndExist(mailboxSession, mailboxOwner, out auditPolicyCacheEntry))
			{
				return mailboxOwner.MailboxInfo.Configuration.AuditAdminOperations;
			}
			return MailboxAuditOperations.Update | MailboxAuditOperations.Move | MailboxAuditOperations.MoveToDeletedItems | MailboxAuditOperations.SoftDelete | MailboxAuditOperations.HardDelete | MailboxAuditOperations.FolderBind | MailboxAuditOperations.SendAs | MailboxAuditOperations.SendOnBehalf | MailboxAuditOperations.Create;
		}

		private static MailboxAuditOperations GetEffectiveAuditOwnerOperations(MailboxSession mailboxSession, IExchangePrincipal mailboxOwner)
		{
			AuditPolicyCacheEntry auditPolicyCacheEntry;
			if (!COWAudit.IsUCCPolicyEnabledAndExist(mailboxSession, mailboxOwner, out auditPolicyCacheEntry))
			{
				return mailboxOwner.MailboxInfo.Configuration.AuditOwnerOperations;
			}
			return MailboxAuditOperations.None;
		}

		private static MailboxAuditOperations GetEffectiveAuditDelegateOperations(MailboxSession mailboxSession, IExchangePrincipal mailboxOwner)
		{
			AuditPolicyCacheEntry auditPolicyCacheEntry;
			if (!COWAudit.IsUCCPolicyEnabledAndExist(mailboxSession, mailboxOwner, out auditPolicyCacheEntry))
			{
				return mailboxOwner.MailboxInfo.Configuration.AuditDelegateOperations;
			}
			if (auditPolicyCacheEntry.LoadStatus != PolicyLoadStatus.Loaded)
			{
				return MailboxAuditOperations.Update | MailboxAuditOperations.SoftDelete | MailboxAuditOperations.HardDelete | MailboxAuditOperations.SendAs | MailboxAuditOperations.Create;
			}
			return auditPolicyCacheEntry.AuditOperationsDelegate;
		}

		private static bool IsAuditConfigFromUCCPolicyEnabled(MailboxSession mailboxSession)
		{
			return AuditFeatureManager.IsAuditConfigFromUCCPolicyEnabled(mailboxSession, null);
		}

		private static GroupOperationAuditEvent CreateGroupOperationAuditEvent(MailboxSession mailboxSession, MailboxAuditOperations operation, COWSettings settings, LogonType effectiveLogonType, bool externalAccess, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, IDictionary<StoreObjectId, FolderAuditInfo> folders, IDictionary<StoreObjectId, ItemAuditInfo> items, IDictionary<StoreObjectId, FolderAuditInfo> parentFolders)
		{
			return new GroupOperationAuditEvent(mailboxSession, operation, settings, effectiveLogonType, externalAccess, destinationSession, destinationFolderId, itemIds, result, folders, items, parentFolders);
		}

		private const MailboxAuditOperations DefaultAuditAdminOperations = MailboxAuditOperations.Update | MailboxAuditOperations.Move | MailboxAuditOperations.MoveToDeletedItems | MailboxAuditOperations.SoftDelete | MailboxAuditOperations.HardDelete | MailboxAuditOperations.FolderBind | MailboxAuditOperations.SendAs | MailboxAuditOperations.SendOnBehalf | MailboxAuditOperations.Create;

		private const MailboxAuditOperations DefaultAuditOwnerOperations = MailboxAuditOperations.None;

		private const MailboxAuditOperations DefaultAuditDelegateOperations = MailboxAuditOperations.Update | MailboxAuditOperations.SoftDelete | MailboxAuditOperations.HardDelete | MailboxAuditOperations.SendAs | MailboxAuditOperations.Create;

		private const int maxGroupItemCount = 32;

		public static readonly MailboxAuditOperations ItemOperations = MailboxAuditOperations.Update | MailboxAuditOperations.FolderBind | MailboxAuditOperations.SendAs | MailboxAuditOperations.SendOnBehalf | MailboxAuditOperations.MessageBind | MailboxAuditOperations.Create;

		public static readonly MailboxAuditOperations GroupOperations = MailboxAuditOperations.Copy | MailboxAuditOperations.Move | MailboxAuditOperations.MoveToDeletedItems | MailboxAuditOperations.SoftDelete | MailboxAuditOperations.HardDelete;

		private static readonly PropertyDefinition[] ItemPropertiesToLoad = new PropertyDefinition[]
		{
			CoreItemSchema.Id,
			CoreItemSchema.ItemClass,
			CoreItemSchema.Subject,
			ItemSchema.From,
			ItemSchema.Sender
		};

		internal static readonly TimeSpan LastAuditAccessRefreshInterval = new TimeSpan(1, 0, 0, 0);

		private static readonly PropertyDefinition[] GroupOperationAuditItemProperties = new PropertyDefinition[]
		{
			InternalSchema.Subject
		};

		private static readonly PropertyDefinition[] CreateOperationLoadProperties = new PropertyDefinition[]
		{
			CoreItemSchema.Id
		};

		private readonly Lazy<UnifiedAuditLogger> logger = new Lazy<UnifiedAuditLogger>(LazyThreadSafetyMode.PublicationOnly);
	}
}

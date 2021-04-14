using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderOperations
	{
		public static StoreId CreateFolder(PublicLogon logon, string folderName, string folderDescription, StoreId parentFolderId, CreateFolderFlags flags)
		{
			Util.ThrowOnNullArgument(logon, "logon");
			return PublicFolderOperations.InvokeOnPrimaryHierarchyAndSync<StoreId>(false, logon, delegate(out StoreId folderIdToSync, out Guid contentMailboxGuid)
			{
				folderIdToSync = logon.PrimaryHierarchyProvider.CreateFolder(folderName, folderDescription, parentFolderId, flags, out contentMailboxGuid);
				return folderIdToSync;
			});
		}

		public static void DeleteFolder(PublicLogon logon, StoreId parentFolderId, StoreId folderId, DeleteFolderFlags flags)
		{
			Util.ThrowOnNullArgument(logon, "logon");
			if ((byte)(flags & DeleteFolderFlags.DeleteFolders) == 0 || (byte)(flags & DeleteFolderFlags.DeleteMessages) == 0)
			{
				throw new RopExecutionException("In Public Folder, Delete Folder operations should always have DeleteFolders and DeleteMessages flag set", (ErrorCode)2147746050U);
			}
			PublicFolderOperations.InvokeOnPrimaryHierarchyAndSync<bool>(true, logon, delegate(out StoreId folderIdToSync, out Guid contentMailboxGuid)
			{
				contentMailboxGuid = Guid.Empty;
				folderIdToSync = folderId;
				logon.PrimaryHierarchyProvider.DeleteFolder(parentFolderId, folderId, flags);
				return true;
			});
		}

		public static GroupOperationResult MoveFolder(PublicLogon logon, StoreId parentFolderId, StoreId destinationFolderId, StoreId sourceFolderId, string folderName)
		{
			Util.ThrowOnNullArgument(logon, "logon");
			GroupOperationResult result;
			try
			{
				result = PublicFolderOperations.InvokeOnPrimaryHierarchyAndSync<GroupOperationResult>(false, logon, delegate(out StoreId folderIdToSync, out Guid contentMailboxGuid)
				{
					contentMailboxGuid = Guid.Empty;
					folderIdToSync = sourceFolderId;
					logon.PrimaryHierarchyProvider.MoveFolder(parentFolderId, destinationFolderId, sourceFolderId, folderName);
					return new GroupOperationResult(OperationResult.Succeeded, new StoreObjectId[]
					{
						StoreId.GetStoreObjectId(sourceFolderId)
					}, null);
				});
			}
			catch (RopExecutionException ex)
			{
				result = new GroupOperationResult(OperationResult.Failed, new StoreObjectId[]
				{
					StoreId.GetStoreObjectId(sourceFolderId)
				}, ex.InnerException as DataSourceOperationException);
			}
			return result;
		}

		public static PropertyProblem[] SetProperties(PublicLogon logon, StoreId folderId, PropertyValue[] propertyValues, bool trackChanges)
		{
			Util.ThrowOnNullArgument(logon, "logon");
			if (trackChanges)
			{
				return PublicFolderOperations.InvokeOnPrimaryHierarchyAndSync<PropertyProblem[]>(false, logon, delegate(out StoreId folderIdToSync, out Guid contentMailboxGuid)
				{
					folderIdToSync = folderId;
					return logon.PrimaryHierarchyProvider.SetProperties(folderId, propertyValues, out contentMailboxGuid);
				});
			}
			return Array<PropertyProblem>.Empty;
		}

		public static PropertyProblem[] DeleteProperties(PublicLogon logon, StoreId folderId, PropertyTag[] propertyTags, bool trackChanges)
		{
			Util.ThrowOnNullArgument(logon, "logon");
			if (trackChanges)
			{
				return PublicFolderOperations.InvokeOnPrimaryHierarchyAndSync<PropertyProblem[]>(false, logon, delegate(out StoreId folderIdToSync, out Guid contentMailboxGuid)
				{
					folderIdToSync = folderId;
					return logon.PrimaryHierarchyProvider.DeleteProperties(folderId, propertyTags, out contentMailboxGuid);
				});
			}
			return Array<PropertyProblem>.Empty;
		}

		public static void ModifyPermissions(PublicLogon logon, CoreFolder coreFolder, IModifyTable permissionsTable, IEnumerable<ModifyTableRow> modifyTableRows, ModifyTableOptions options, bool replaceRows)
		{
			Util.ThrowOnNullArgument(logon, "logon");
			PublicFolderOperations.InvokeOnPrimaryHierarchyAndSync<bool>(false, logon, delegate(out StoreId folderIdToSync, out Guid contentMailboxGuid)
			{
				contentMailboxGuid = coreFolder.GetContentMailboxInfo().MailboxGuid;
				folderIdToSync = coreFolder.Id;
				logon.PrimaryHierarchyProvider.ModifyPermissions(coreFolder, permissionsTable, modifyTableRows, options, replaceRows);
				return true;
			});
		}

		private static T InvokeOnPrimaryHierarchyAndSync<T>(bool isCompleteSync, PublicLogon publicLogon, PublicFolderOperations.PrimaryHierarchyActionDelegate<T> primaryHierarchyAction)
		{
			if (publicLogon.PublicFolderSession.IsPrimaryHierarchySession || publicLogon.PrimaryHierarchyProvider == null)
			{
				throw new InvalidOperationException("InvokeOnPrimaryHierarchyAndSync operations are valid only for secondary hierarchy logons.");
			}
			StoreId storeId = null;
			T t = default(T);
			T result;
			try
			{
				Guid guid;
				t = primaryHierarchyAction(out storeId, out guid);
				if (storeId != null)
				{
					Guid guid2;
					Guid b;
					ExchangePrincipal contentMailboxPrincipal;
					if (isCompleteSync && PublicFolderSession.TryGetHierarchyMailboxGuidForUser(publicLogon.Connection.MiniRecipient.OrganizationId, publicLogon.Connection.MiniRecipient.ExchangeGuid, publicLogon.Connection.MiniRecipient.DefaultPublicFolderMailbox, out guid2) && PublicFolderSession.TryGetPrimaryHierarchyMailboxGuid(publicLogon.PublicFolderSession.OrganizationId, out b) && guid2 != b && PublicFolderSession.TryGetPublicFolderMailboxPrincipal(publicLogon.PublicFolderSession.OrganizationId, guid2, false, out contentMailboxPrincipal))
					{
						PublicFolderSyncJobRpc.StartSyncHierarchy(contentMailboxPrincipal, false);
					}
					if (guid != publicLogon.PublicFolderSession.MailboxGuid)
					{
						PublicFolderSyncJobRpc.SyncFolder(publicLogon.PublicFolderSession.MailboxPrincipal, StoreId.GetStoreObjectId(storeId).ProviderLevelItemId);
					}
					if (PublicFolderSession.TryGetHierarchyMailboxGuidForUser(publicLogon.Connection.MiniRecipient.OrganizationId, publicLogon.Connection.MiniRecipient.ExchangeGuid, publicLogon.Connection.MiniRecipient.DefaultPublicFolderMailbox, out guid2) && guid2 != guid && guid2 != publicLogon.PublicFolderSession.MailboxGuid && PublicFolderSession.TryGetPrimaryHierarchyMailboxGuid(publicLogon.PublicFolderSession.OrganizationId, out b) && guid2 != b && PublicFolderSession.TryGetPublicFolderMailboxPrincipal(publicLogon.PublicFolderSession.OrganizationId, guid2, false, out contentMailboxPrincipal))
					{
						PublicFolderSyncJobRpc.SyncFolder(contentMailboxPrincipal, StoreId.GetStoreObjectId(storeId).ProviderLevelItemId);
					}
				}
				else
				{
					ExTraceGlobals.FolderTracer.TraceWarning(Activity.TraceId, "Sync operation was not called after hierarchy action as folderIdToSync was null.");
				}
				result = t;
			}
			catch (LimitExceededException ex)
			{
				ExTraceGlobals.FolderTracer.TraceDebug<LimitExceededException>(Activity.TraceId, "EWS Redirection call failed with the following exception - {0}", ex);
				throw new RopExecutionException(ex.Message, ErrorCode.ServerBusy, ex);
			}
			return result;
		}

		private delegate T PrimaryHierarchyActionDelegate<T>(out StoreId folderIdToSync, out Guid contentMailboxGuid);
	}
}

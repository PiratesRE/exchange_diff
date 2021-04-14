using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupOperationAuditEvent : AuditEvent
	{
		public GroupOperationAuditEvent(MailboxSession session, MailboxAuditOperations operation, COWSettings settings, LogonType logonType, bool externalAccess, StoreSession destinationSession, StoreObjectId destinationFolderId, StoreObjectId[] itemIds, GroupOperationResult result, IDictionary<StoreObjectId, FolderAuditInfo> folderAuditInfo, IDictionary<StoreObjectId, ItemAuditInfo> itemAuditInfo, IDictionary<StoreObjectId, FolderAuditInfo> parentFolders) : base(session, operation, settings, (result == null) ? OperationResult.Failed : result.OperationResult, logonType, externalAccess)
		{
			this.destinationSession = destinationSession;
			this.destinationFolderId = destinationFolderId;
			this.folderAuditInfo = folderAuditInfo;
			this.itemAuditInfo = itemAuditInfo;
			this.parentFolders = parentFolders;
		}

		protected override IEnumerable<KeyValuePair<string, string>> InternalGetEventDetails()
		{
			foreach (KeyValuePair<string, string> detail in base.InternalGetEventDetails())
			{
				yield return detail;
			}
			if (base.COWSettings.CurrentFolderId != null)
			{
				yield return new KeyValuePair<string, string>("FolderId", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					base.COWSettings.CurrentFolderId
				}));
				FolderAuditInfo currentFolder;
				if (this.parentFolders.TryGetValue(base.COWSettings.CurrentFolderId, out currentFolder))
				{
					yield return new KeyValuePair<string, string>("FolderPathName", currentFolder.PathName);
				}
				else
				{
					string folderPathName = base.GetCurrentFolderPathName();
					if (folderPathName != null)
					{
						yield return new KeyValuePair<string, string>("FolderPathName", folderPathName);
					}
				}
			}
			bool crossMailbox = this.destinationSession != null && base.MailboxSession != this.destinationSession;
			yield return new KeyValuePair<string, string>("CrossMailboxOperation", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				crossMailbox
			}));
			MailboxSession destinationMailboxSession = base.MailboxSession;
			if (crossMailbox && this.destinationSession is MailboxSession)
			{
				destinationMailboxSession = (this.destinationSession as MailboxSession);
				yield return new KeyValuePair<string, string>("DestMailboxGuid", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					destinationMailboxSession.MailboxOwner.MailboxInfo.MailboxGuid
				}));
				yield return new KeyValuePair<string, string>("DestMailboxOwnerUPN", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					destinationMailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress
				}));
				if (destinationMailboxSession.MailboxOwner.Sid != null)
				{
					yield return new KeyValuePair<string, string>("DestMailboxOwnerSid", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						destinationMailboxSession.MailboxOwner.Sid
					}));
					if (destinationMailboxSession.MailboxOwner.MasterAccountSid != null)
					{
						yield return new KeyValuePair<string, string>("DestMailboxOwnerMasterAccountSid", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
						{
							destinationMailboxSession.MailboxOwner.MasterAccountSid
						}));
					}
				}
			}
			if (this.destinationFolderId != null)
			{
				yield return new KeyValuePair<string, string>("DestFolderId", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					this.destinationFolderId
				}));
				string folderPathName2 = null;
				Exception exception = null;
				try
				{
					using (Folder folder2 = Folder.Bind(destinationMailboxSession, this.destinationFolderId, new PropertyDefinition[]
					{
						FolderSchema.FolderPathName
					}))
					{
						if (folder2 != null)
						{
							folderPathName2 = (folder2.TryGetProperty(FolderSchema.FolderPathName) as string);
							if (folderPathName2 != null)
							{
								folderPathName2 = folderPathName2.Replace(COWSettings.StoreIdSeparator, '\\');
							}
						}
					}
				}
				catch (StorageTransientException ex)
				{
					exception = ex;
				}
				catch (StoragePermanentException ex2)
				{
					exception = ex2;
				}
				if (exception != null)
				{
					ExTraceGlobals.SessionTracer.TraceError<StoreObjectId, Exception>((long)base.MailboxSession.GetHashCode(), "[GroupOperationAuditEvent::ToString] failed to get FolderPathName property of destination folder {0}. Exception: {1}", this.destinationFolderId, exception);
				}
				if (folderPathName2 != null)
				{
					yield return new KeyValuePair<string, string>("DestFolderPathName", folderPathName2);
				}
			}
			int folderCount = 0;
			foreach (KeyValuePair<StoreObjectId, FolderAuditInfo> folder in this.folderAuditInfo)
			{
				KeyValuePair<StoreObjectId, FolderAuditInfo> keyValuePair = folder;
				StoreObjectId folderId = keyValuePair.Key;
				KeyValuePair<StoreObjectId, FolderAuditInfo> keyValuePair2 = folder;
				FolderAuditInfo folderInfo = keyValuePair2.Value;
				if (base.COWSettings.CurrentFolderId == null || !folderId.Equals(base.COWSettings.CurrentFolderId))
				{
					yield return new KeyValuePair<string, string>(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						folderCount,
						".SourceFolderId"
					}), string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						folderId
					}));
					if (folderInfo.PathName != null)
					{
						yield return new KeyValuePair<string, string>(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
						{
							folderCount,
							".SourceFolderPathName"
						}), folderInfo.PathName);
					}
					folderCount++;
				}
			}
			int itemCount = 0;
			foreach (KeyValuePair<StoreObjectId, ItemAuditInfo> item in this.itemAuditInfo)
			{
				KeyValuePair<StoreObjectId, ItemAuditInfo> keyValuePair3 = item;
				StoreObjectId itemId = keyValuePair3.Key;
				KeyValuePair<StoreObjectId, ItemAuditInfo> keyValuePair4 = item;
				ItemAuditInfo itemInfo = keyValuePair4.Value;
				yield return new KeyValuePair<string, string>(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
				{
					itemCount,
					".SourceItemId"
				}), string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					itemId
				}));
				if (itemInfo.Subject != null)
				{
					yield return new KeyValuePair<string, string>(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						itemCount,
						".SourceItemSubject"
					}), itemInfo.Subject);
				}
				FolderAuditInfo folderInfo2;
				if (itemInfo.ParentFolderId != null && this.parentFolders.TryGetValue(itemInfo.ParentFolderId, out folderInfo2))
				{
					yield return new KeyValuePair<string, string>(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						itemCount,
						".SourceItemFolderPathName"
					}), folderInfo2.PathName);
				}
				itemCount++;
			}
			yield break;
		}

		private IDictionary<StoreObjectId, FolderAuditInfo> folderAuditInfo;

		private IDictionary<StoreObjectId, ItemAuditInfo> itemAuditInfo;

		private IDictionary<StoreObjectId, FolderAuditInfo> parentFolders;

		private StoreSession destinationSession;

		private StoreObjectId destinationFolderId;
	}
}

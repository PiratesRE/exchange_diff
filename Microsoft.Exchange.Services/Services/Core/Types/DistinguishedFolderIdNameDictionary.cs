using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DistinguishedFolderIdNameDictionary
	{
		internal DistinguishedFolderIdNameDictionary()
		{
			this.currentMailboxGuid = Guid.Empty;
			this.folderIdToNameMap = new Dictionary<StoreObjectId, string>();
		}

		internal string Get(StoreObjectId folderId, StoreSession session)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession == null)
			{
				return null;
			}
			Guid mailboxGuid = mailboxSession.MailboxGuid;
			if (this.currentMailboxGuid != mailboxGuid)
			{
				this.BuildFolderIdToNameMap(mailboxSession);
				this.currentMailboxGuid = mailboxGuid;
			}
			string result;
			if (this.folderIdToNameMap.TryGetValue(folderId, out result))
			{
				return result;
			}
			return null;
		}

		internal void BuildFolderIdToNameMap(MailboxSession mailboxSession)
		{
			this.folderIdToNameMap.Clear();
			Dictionary<DefaultFolderType, string> dictionary;
			if (mailboxSession.MailboxOwner.MailboxInfo.IsArchive)
			{
				dictionary = IdConverter.GetDefaultFolderTypeToFolderNameMapForArchiveMailbox();
			}
			else
			{
				dictionary = IdConverter.GetDefaultFolderTypeToFolderNameMapForMailbox();
			}
			foreach (DefaultFolderType defaultFolderType in dictionary.Keys)
			{
				StoreObjectId storeObjectId;
				if (defaultFolderType == DefaultFolderType.AdminAuditLogs)
				{
					if (mailboxSession.LogonType != LogonType.SystemService)
					{
						continue;
					}
					storeObjectId = mailboxSession.GetAdminAuditLogsFolderId();
				}
				else
				{
					storeObjectId = mailboxSession.GetDefaultFolderId(defaultFolderType);
				}
				if (storeObjectId != null)
				{
					if (!this.folderIdToNameMap.ContainsKey(storeObjectId))
					{
						this.folderIdToNameMap.Add(storeObjectId, dictionary[defaultFolderType]);
					}
					else
					{
						ExTraceGlobals.UtilAlgorithmTracer.TraceError<string, string>((long)this.GetHashCode(), "FolderId is already mapped in FolderIdToName map to '{0}'. It cannot also be mapped to '{1}'.", this.folderIdToNameMap[storeObjectId], dictionary[defaultFolderType]);
					}
				}
			}
		}

		private Guid currentMailboxGuid;

		private Dictionary<StoreObjectId, string> folderIdToNameMap;
	}
}

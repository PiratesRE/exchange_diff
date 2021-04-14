using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class SearchBacklinksCheckTask : IntegrityCheckTaskBase
	{
		public SearchBacklinksCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "SearchBacklinks";
			}
		}

		public override ErrorCode ExecuteOneFolder(Mailbox mailbox, MailboxEntry mailboxEntry, FolderEntry folderEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			Context currentOperationContext = mailbox.CurrentOperationContext;
			this.currentMailbox = mailboxEntry;
			this.currentFolder = folderEntry;
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "Mailbox {0}: Executing task {1} with detectOnly=={2} on folder {3}", new object[]
				{
					mailboxEntry.MailboxOwnerName,
					this.TaskName,
					detectOnly,
					folderEntry.ToString()
				});
			}
			Folder folder = Folder.OpenFolder(currentOperationContext, mailbox, folderEntry.FolderId);
			if (folder != null)
			{
				this.CheckSearchBacklinks(currentOperationContext, folder, false, detectOnly);
				this.CheckSearchBacklinks(currentOperationContext, folder, true, detectOnly);
			}
			else if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string>(0L, "Skipping folder {0} because it no longer exists.", folderEntry.ToString());
			}
			return ErrorCode.NoError;
		}

		protected internal override bool IgnoreFolder(Context context, Mailbox mailbox, ExchangeId folderId, ExchangeId parentFolderId, bool isSearchFolder, short specialFolderNumber)
		{
			return false;
		}

		private bool CheckSearchBacklinks(Context context, Folder folder, bool recursive, bool detectOnly)
		{
			bool flag = false;
			bool flag2 = false;
			string arg = recursive ? "recursive" : "non-recursive";
			IList<ExchangeId> searchBacklinksNoAsserts = folder.GetSearchBacklinksNoAsserts(context, recursive);
			List<ExchangeId> list = new List<ExchangeId>(0);
			if (!ValueHelper.IsArraySorted<ExchangeId>(searchBacklinksNoAsserts, false))
			{
				base.ReportCorruption(string.Format("The list of {0} search backlinks is not properly sorted or has illegal duplicate entries.", arg), this.currentMailbox, this.currentFolder, null, CorruptionType.SearchBacklinksUnsorted, !detectOnly);
				flag = true;
				flag2 = true;
			}
			foreach (ExchangeId exchangeId in searchBacklinksNoAsserts)
			{
				Folder folder2 = Folder.OpenFolder(context, folder.Mailbox, exchangeId);
				if (folder2 == null)
				{
					base.ReportCorruption(string.Format("There is a {0} search backlink to folder {1}, but the folder does not exist.", arg, exchangeId), this.currentMailbox, this.currentFolder, null, CorruptionType.SearchFolderNotFound, !detectOnly);
					flag = true;
				}
				else if (!(folder2 is SearchFolder))
				{
					base.ReportCorruption(string.Format("There is a {0} search backlink to folder {1}, but the folder is not a search folder.", arg, exchangeId), this.currentMailbox, this.currentFolder, null, CorruptionType.SearchBacklinkNotSearchFolder, !detectOnly);
					flag = true;
				}
				else if (((SearchFolder)folder2).IsStaticSearch(context))
				{
					base.ReportCorruption(string.Format("There is a {0} search backlink to search folder {1}, but the search folder is not for a dynamic search.", arg, exchangeId), this.currentMailbox, this.currentFolder, null, CorruptionType.SearchBacklinkIsNotDynamicSearchFolder, !detectOnly);
					flag = true;
				}
				else if (!((SearchFolder)folder2).GetQueryScope(context).Folders.Contains(folder.GetId(context)))
				{
					base.ReportCorruption(string.Format("There is a {0} search backlink to search folder {1}, but the folder is not in the scope of the search.", arg, exchangeId, recursive ? "non-recursive" : "recursive"), this.currentMailbox, this.currentFolder, null, CorruptionType.FolderOutOfSearchScope, !detectOnly);
					flag = true;
				}
				else if (((SearchFolder)folder2).IsRecursiveSearch(context) != recursive)
				{
					base.ReportCorruption(string.Format("There is a {0} search backlink to search folder {1}, but the search folder performs a {2} search.", arg, exchangeId, recursive ? "non-recursive" : "recursive"), this.currentMailbox, this.currentFolder, null, CorruptionType.SearchBacklinksRecursiveMismatch, !detectOnly);
					flag = true;
				}
				else if (flag2 && list.Contains(exchangeId))
				{
					base.ReportCorruption(string.Format("The {0} search backlink to folder {1} occurs multiple times in the search backlink list.", arg, exchangeId), this.currentMailbox, this.currentFolder, null, CorruptionType.SearchBacklinksDuplicatedFolder, !detectOnly);
					flag = true;
				}
				else
				{
					list.Add(exchangeId);
				}
			}
			if (flag && !detectOnly)
			{
				list.Sort();
				folder.SetSearchBacklinks(context, list, recursive);
			}
			return !flag;
		}

		private MailboxEntry currentMailbox;

		private FolderEntry currentFolder;
	}
}

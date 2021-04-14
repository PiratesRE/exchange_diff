using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class FolderTypeCheckTask : IntegrityCheckTaskBase
	{
		public FolderTypeCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "FolderTypeCheckTask";
			}
		}

		public override ErrorCode ExecuteOneFolder(Mailbox mailbox, MailboxEntry mailboxEntry, FolderEntry folderEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			Context currentOperationContext = mailbox.CurrentOperationContext;
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
			if (folder != null && !(folder is SearchFolder))
			{
				Folder parentFolder = folder.GetParentFolder(currentOperationContext);
				if (parentFolder != null && (folderEntry.NameStartsWith("Restriction-") || folderEntry.NameStartsWith("Restriciton-")) && parentFolder.GetSpecialFolderNumber(currentOperationContext) == SpecialFolders.Finder)
				{
					if (!detectOnly)
					{
						FolderTable folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(currentOperationContext.Database);
						folder.SetColumn(currentOperationContext, folderTable.QueryCriteria, new RestrictionFalse().Serialize());
						folder.Save(currentOperationContext);
					}
					base.ReportCorruption("Incorrect folder type detected", mailboxEntry, folderEntry, null, CorruptionType.WrongFolderTypeOnRestrictionFolder, !detectOnly);
				}
			}
			return ErrorCode.NoError;
		}

		protected internal override bool IgnoreFolder(Context context, Mailbox mailbox, ExchangeId folderId, ExchangeId parentFolderId, bool isSearchFolder, short specialFolderNumber)
		{
			return false;
		}
	}
}

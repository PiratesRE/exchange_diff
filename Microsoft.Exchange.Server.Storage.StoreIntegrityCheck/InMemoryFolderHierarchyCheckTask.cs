using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class InMemoryFolderHierarchyCheckTask : IntegrityCheckTaskBase
	{
		public InMemoryFolderHierarchyCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "InMemoryFolderHierarchy";
			}
		}

		public override ErrorCode Execute(Context context, MailboxEntry mailboxEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, bool>(0L, "InMemoryFolderHierarchyCheckTask.Execute invoked on mailbox {0}, detect only = {1}", mailboxEntry.MailboxGuid, detectOnly);
			}
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)37936U);
			}
			this.currentMailbox = mailboxEntry;
			ErrorCode errorCode = IntegrityCheckTaskBase.LockMailboxForOperation(context, mailboxEntry.MailboxNumber, delegate(MailboxState mailboxState)
			{
				ErrorCode noError;
				try
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
					{
						if (mailbox == null)
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "The mailbox has been removed");
							}
							noError = ErrorCode.NoError;
						}
						else
						{
							this.ValidateFolderHierarchy(context, mailbox);
							noError = ErrorCode.NoError;
						}
					}
				}
				finally
				{
					context.Abort();
				}
				return noError;
			});
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, ErrorCode>(0L, "InMemoryFolderHierarchyCheckTask.Execute finished on mailbox {0} with error code {1}", mailboxEntry.MailboxGuid, errorCode);
			}
			return errorCode;
		}

		private bool ValidateFolderHierarchy(Context context, Mailbox mailbox)
		{
			FolderHierarchy folderHierarchyNoCreate = FolderHierarchy.GetFolderHierarchyNoCreate(context, mailbox);
			if (folderHierarchyNoCreate == null)
			{
				return true;
			}
			FolderHierarchy folderHierarchy = FolderHierarchy.FolderHierarchySnapshotFromDisk(context, mailbox, FolderInformationType.Basic);
			if (folderHierarchyNoCreate.HierarchyRoots.Count != folderHierarchy.HierarchyRoots.Count)
			{
				base.ReportCorruption("Mismatch number of hierarchy roots", this.currentMailbox, null, null, CorruptionType.FolderHierarchyRootCountMismatch, false);
				return false;
			}
			if (folderHierarchyNoCreate.TotalFolderCount != folderHierarchy.TotalFolderCount)
			{
				base.ReportCorruption("Mismatch number of total folder count", this.currentMailbox, null, null, CorruptionType.FolderHierarchyTotalFolderCountMismatch, false);
				return false;
			}
			FolderInformationComparer folderInformationComparer = new FolderInformationComparer(context.Culture.CompareInfo);
			List<IFolderInformation> list = folderHierarchyNoCreate.HierarchyRoots.ToList<IFolderInformation>();
			list.Sort(folderInformationComparer);
			List<IFolderInformation> list2 = folderHierarchy.HierarchyRoots.ToList<IFolderInformation>();
			list2.Sort(folderInformationComparer);
			Queue<IFolderInformation> queue = new Queue<IFolderInformation>(list);
			Queue<IFolderInformation> queue2 = new Queue<IFolderInformation>(list2);
			while (queue.Count > 0 && queue2.Count > 0)
			{
				IFolderInformation folderInformation = queue.Dequeue();
				IFolderInformation folderInformation2 = queue2.Dequeue();
				if (!this.FolderInfomationsAreEqual(folderInformation, folderInformation2, folderInformationComparer))
				{
					return false;
				}
				List<IFolderInformation> list3 = new List<IFolderInformation>(folderHierarchyNoCreate.GetChildren(context, folderInformation));
				List<IFolderInformation> list4 = new List<IFolderInformation>(folderHierarchy.GetChildren(context, folderInformation2));
				if (list3.Count != list4.Count)
				{
					base.ReportCorruption(string.Format("Mismatch number of children for fid:{0}", folderInformation.Fid), this.currentMailbox, null, null, CorruptionType.FolderChildrenCountMismatch, false);
					return false;
				}
				list3.Sort(folderInformationComparer);
				list4.Sort(folderInformationComparer);
				foreach (IFolderInformation item in list3)
				{
					queue.Enqueue(item);
				}
				foreach (IFolderInformation item2 in list4)
				{
					queue2.Enqueue(item2);
				}
			}
			return true;
		}

		private bool FolderInfomationsAreEqual(IFolderInformation folderInformation1, IFolderInformation folderInformation2, FolderInformationComparer folderInformationComparer)
		{
			bool flag = false;
			if (folderInformation1.Fid != folderInformation2.Fid)
			{
				flag = true;
				base.ReportCorruption(string.Format("Fid mismatch. folder1.fid:{0}, folder2.fid:{1}", folderInformation1.Fid, folderInformation2.Fid), this.currentMailbox, null, null, CorruptionType.FolderInformationFidMismatch, false);
			}
			if (folderInformation1.IsSearchFolder != folderInformation2.IsSearchFolder)
			{
				flag = true;
				base.ReportCorruption(string.Format("IsSearchFolder mismatch for fid:{0}", folderInformation1.Fid), this.currentMailbox, null, null, CorruptionType.FolderInformationIsSearchFolderMismatch, false);
			}
			if (folderInformationComparer.CompareInfo.Compare(folderInformation1.DisplayName, folderInformation2.DisplayName, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) != 0)
			{
				flag = true;
				base.ReportCorruption(string.Format("DisplayName mismatch for fid:{0}", folderInformation1.Fid), this.currentMailbox, null, null, CorruptionType.FolderInformationDisplayNameMismatch, false);
			}
			if (folderInformation1.IsPartOfContentIndexing != folderInformation2.IsPartOfContentIndexing)
			{
				flag = true;
				base.ReportCorruption(string.Format("IsPartOfContentIndexing mismatch for fid:{0}", folderInformation1.Fid), this.currentMailbox, null, null, CorruptionType.FolderInformationIsPartOfContentIndexingMismatch, false);
			}
			if (folderInformation1.MessageCount != folderInformation2.MessageCount)
			{
				flag = true;
				base.ReportCorruption(string.Format("MessageCount mismatch for fid:{0}", folderInformation1.Fid), this.currentMailbox, null, null, CorruptionType.FolderInformationMessageCountMismatch, false);
			}
			return !flag;
		}

		private MailboxEntry currentMailbox;
	}
}

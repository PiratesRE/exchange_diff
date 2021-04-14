using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class FolderAclCheckTask : IntegrityCheckTaskBase
	{
		public FolderAclCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "FolderAclCheckTask";
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
			if (folder != null)
			{
				bool flag = false;
				byte[] array = (byte[])folder.GetPropertyValue(currentOperationContext, PropTag.Folder.AclTableAndSecurityDescriptor);
				if (FolderSecurity.AclTableAndSecurityDescriptorProperty.IsEmpty(array))
				{
					flag = true;
				}
				else
				{
					try
					{
						FolderSecurity.AclTableAndSecurityDescriptorProperty.Parse(array);
					}
					catch (ArgumentException exception)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
						flag = true;
					}
					catch (IndexOutOfRangeException exception2)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(exception2);
						flag = true;
					}
					catch (EndOfStreamException exception3)
					{
						NullExecutionDiagnostics.Instance.OnExceptionCatch(exception3);
						flag = true;
					}
				}
				if (flag)
				{
					if (!detectOnly)
					{
						FolderTable folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(currentOperationContext.Database);
						if (mailbox.IsPublicFolderMailbox)
						{
							array = FolderSecurity.AclTableAndSecurityDescriptorProperty.GetDefaultBlobForPublicFolders();
						}
						else
						{
							array = FolderSecurity.AclTableAndSecurityDescriptorProperty.GetDefaultBlobForRootFolder();
						}
						folder.SetColumn(currentOperationContext, folderTable.AclTableAndSecurityDescriptor, array);
						folder.Save(currentOperationContext);
					}
					base.ReportCorruption("ACL corruption detected on the folder", mailboxEntry, folderEntry, null, CorruptionType.NoAclStampedOnFolder, !detectOnly);
				}
			}
			return ErrorCode.NoError;
		}

		protected internal override bool IgnoreFolder(Context context, Mailbox mailbox, ExchangeId folderId, ExchangeId parentFolderId, bool isSearchFolder, short specialFolderNumber)
		{
			if (specialFolderNumber == 20 || specialFolderNumber == 21)
			{
				return true;
			}
			ExchangeId id = SpecialFoldersCache.GetSpecialFolders(context, mailbox)[21];
			return id.IsValid && parentFolderId == id;
		}
	}
}

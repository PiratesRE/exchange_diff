using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class MissingSpecialFoldersCheckTask : IntegrityCheckTaskBase
	{
		public MissingSpecialFoldersCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "MissingSpecialFoldersCheckTask";
			}
		}

		public override ErrorCode Execute(Context context, MailboxEntry mailboxEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			this.specialFolders = new HashSet<SpecialFolders>();
			this.folderEntries = null;
			this.mailboxEntry = mailboxEntry;
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string, Guid, bool>(0L, "Integrity check task \"{0}\" invoked on mailbox {1}, detect only={2}", this.TaskName, mailboxEntry.MailboxGuid, detectOnly);
			}
			errorCode = IntegrityCheckTaskBase.LockMailboxForOperation(context, mailboxEntry.MailboxNumber, delegate(MailboxState mailboxState)
			{
				if (mailboxState.IsPublicFolderMailbox)
				{
					return ErrorCode.CreateUnknownMailbox((LID)35100U);
				}
				return this.GetFolderEntriesForMailbox(context, mailboxState, out this.folderEntries, shouldContinue);
			});
			if (!(errorCode != ErrorCode.NoError))
			{
				return this.CheckForSpecialFolder(context, SpecialFolders.SpoolerQueue, detectOnly, shouldContinue).Propagate((LID)59676U);
			}
			if (errorCode == ErrorCodeValue.UnknownMailbox)
			{
				return ErrorCode.NoError;
			}
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Integrity check encountered error, mailbox={0}, task={1}, detect only={2}, error={3}", new object[]
				{
					mailboxEntry.MailboxGuid,
					this.TaskName,
					detectOnly,
					errorCode
				});
			}
			return errorCode.Propagate((LID)39520U);
		}

		protected internal override bool IgnoreFolder(Context context, Mailbox mailbox, ExchangeId folderId, ExchangeId parentFolderId, bool isSearchFolder, short specialFolderNumber)
		{
			this.specialFolders.Add((SpecialFolders)specialFolderNumber);
			return false;
		}

		private ErrorCode CheckForSpecialFolder(Context context, SpecialFolders specialFolder, bool detectOnly, Func<bool> shouldContinue)
		{
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)43292U);
			}
			ExchangeId rootId = ExchangeId.Zero;
			ExchangeId expectedSpoolerQueueId = ExchangeId.Zero;
			ExchangeId actualSpoolerQueueId = ExchangeId.Zero;
			ExchangeId id = ExchangeId.Zero;
			FolderEntry folderEntry = null;
			for (int i = 0; i < this.folderEntries.Count; i++)
			{
				short specialFolderNumber = this.folderEntries[i].SpecialFolderNumber;
				if (specialFolderNumber != 1)
				{
					switch (specialFolderNumber)
					{
					case 7:
						id = this.folderEntries[i].FolderId;
						break;
					case 8:
						actualSpoolerQueueId = this.folderEntries[i].FolderId;
						folderEntry = this.folderEntries[i];
						break;
					}
				}
				else
				{
					rootId = this.folderEntries[i].FolderId;
				}
			}
			if (rootId == ExchangeId.Zero)
			{
				return ErrorCode.CreateExiting((LID)47900U);
			}
			if (id == ExchangeId.Zero)
			{
				return ErrorCode.CreateExiting((LID)64284U);
			}
			if (id.Replid != rootId.Replid)
			{
				return ErrorCode.CreateExiting((LID)56092U);
			}
			if (actualSpoolerQueueId.Replid == id.Replid)
			{
				return ErrorCode.NoError;
			}
			expectedSpoolerQueueId = ExchangeId.Create(id.Guid, 1UL + id.Counter, id.Replid);
			if (!detectOnly)
			{
				ErrorCode first = IntegrityCheckTaskBase.LockMailboxForOperation(context, this.mailboxEntry.MailboxNumber, delegate(MailboxState mailboxState)
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
					{
						Folder folder = Folder.OpenFolder(context, mailbox, expectedSpoolerQueueId);
						if (folder != null)
						{
							return ErrorCode.CreateExiting((LID)60188U);
						}
						if (actualSpoolerQueueId != ExchangeId.Zero)
						{
							Folder folder2 = Folder.OpenFolder(context, mailbox, actualSpoolerQueueId);
							if (folder2 != null)
							{
								folder2.Delete(context);
								mailbox.Save(context);
							}
						}
						Folder folder3 = Folder.OpenFolder(context, mailbox, rootId);
						SearchFolder searchFolder = SearchFolder.CreateSearchFolder(context, folder3, expectedSpoolerQueueId);
						Restriction restriction = new RestrictionAND(new Restriction[]
						{
							new RestrictionBitmask(PropTag.Message.MessageFlagsActual, 4L, BitmaskOperation.NotEqualToZero),
							new RestrictionBitmask(PropTag.Message.SubmitResponsibility, 1L, BitmaskOperation.EqualToZero)
						});
						searchFolder.SetName(context, "Spooler Queue");
						searchFolder.SetSpecialFolderNumber(context, specialFolder);
						searchFolder.SetSearchCriteria(context, restriction.Serialize(), new ExchangeId[]
						{
							rootId
						}, SetSearchCriteriaFlags.Recursive);
						byte[] value = (byte[])folder3.GetPropertyValue(context, PropTag.Folder.AclTableAndSecurityDescriptor);
						FolderTable folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(context.Database);
						searchFolder.SetColumn(context, folderTable.AclTableAndSecurityDescriptor, value);
						searchFolder.Save(context);
						mailbox.Save(context);
					}
					mailboxState.CleanupAsNonActive(context);
					return ErrorCode.NoError;
				});
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)51996U);
				}
			}
			if (actualSpoolerQueueId == ExchangeId.Zero)
			{
				base.ReportCorruption("Missing special folder " + specialFolder, this.mailboxEntry, null, null, CorruptionType.MissingSpecialFolder, !detectOnly);
			}
			else
			{
				base.ReportCorruption("Wrong id special folder " + specialFolder, this.mailboxEntry, folderEntry, null, CorruptionType.MissingSpecialFolder, !detectOnly);
			}
			return ErrorCode.NoError;
		}

		private HashSet<SpecialFolders> specialFolders;

		private MailboxEntry mailboxEntry;

		private List<FolderEntry> folderEntries;
	}
}

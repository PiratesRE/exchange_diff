using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Imap;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ImapSourceMailbox : ImapMailbox, ISourceMailbox, IMailbox, IDisposable, ISupportMime, IReplayProvider
	{
		public ImapSourceMailbox(ConnectionParameters connectionParameters, ImapAuthenticationParameters authenticationParameters, ImapServerParameters serverParameters, SmtpServerParameters smtpParameters) : base(connectionParameters, authenticationParameters, serverParameters, smtpParameters)
		{
		}

		internal override bool SupportsSavingSyncState
		{
			get
			{
				return true;
			}
		}

		public override SyncProtocol GetSyncProtocol()
		{
			return SyncProtocol.Imap;
		}

		Stream ISupportMime.GetMimeStream(MessageRec message, out PropValueData[] extraPropValues)
		{
			extraPropValues = null;
			string messageUid = ImapEntryId.ParseUid(message.EntryId).ToString(CultureInfo.InvariantCulture);
			Stream result;
			ExDateTime? exDateTime;
			this.FetchMessage(messageUid, out result, out exDateTime);
			if (exDateTime != null)
			{
				extraPropValues = new PropValueData[]
				{
					new PropValueData(PropTag.MessageDeliveryTime, exDateTime.Value.ToUtc())
				};
			}
			return result;
		}

		byte[] ISourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags flags)
		{
			throw new NotImplementedException();
		}

		ISourceFolder ISourceMailbox.GetFolder(byte[] entryId)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			return base.GetFolder<ImapSourceFolder>(entryId);
		}

		void ISourceMailbox.CopyTo(IFxProxy destMailboxProxy, PropTag[] excludeTags)
		{
			throw new NotImplementedException();
		}

		void ISourceMailbox.SetMailboxSyncState(string syncStateStr)
		{
			base.SetMailboxSyncState(syncStateStr);
		}

		string ISourceMailbox.GetMailboxSyncState()
		{
			return base.GetMailboxSyncState();
		}

		MailboxChangesManifest ISourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags flags, int maxChanges)
		{
			bool catchup = flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup);
			return this.EnumerateHierarchyChanges(catchup, (SyncHierarchyManifestState hierarchyData) => this.RunManualHierarchySync(catchup, hierarchyData));
		}

		void ISourceMailbox.ExportMessages(List<MessageRec> messages, IFxProxyPool proxyPool, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.ExportMessages({0} messages)", new object[]
			{
				messages.Count
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.CopyMessagesOneByOne(messages, proxyPool, propsToCopyExplicitly, excludeProps, delegate(MessageRec curMsg)
			{
				using (ImapFolder folder = base.GetFolder<ImapSourceFolder>(curMsg.FolderId))
				{
					if (folder == null)
					{
						throw new FolderIsMissingTransientException();
					}
					folder.Folder.SelectImapFolder(base.ImapConnection);
				}
			});
		}

		void ISourceMailbox.ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			throw new NotImplementedException();
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.ReplayActions({0} actions)", new object[]
			{
				actions.Count
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			return this.Replay(actions);
		}

		void IReplayProvider.MarkAsRead(IReadOnlyCollection<MarkAsReadAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.MarkAsRead({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (MarkAsReadAction markAsReadAction in actions)
			{
				this.SetReadFlags(markAsReadAction.ItemId, markAsReadAction.FolderId, true);
			}
		}

		void IReplayProvider.MarkAsUnRead(IReadOnlyCollection<MarkAsUnReadAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.MarkAsUnRead({0} actions)", new object[]
			{
				actions.Count
			});
			foreach (MarkAsUnReadAction markAsUnReadAction in actions)
			{
				this.SetReadFlags(markAsUnReadAction.ItemId, markAsUnReadAction.FolderId, false);
			}
		}

		IReadOnlyCollection<MoveActionResult> IReplayProvider.Move(IReadOnlyCollection<MoveAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.Move({0} actions)", new object[]
			{
				actions.Count
			});
			throw new ActionNotSupportedException();
		}

		void IReplayProvider.Send(SendAction action)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.Send({0})", new object[]
			{
				action
			});
			SmtpClientHelper.Submit(action, base.SmtpParameters.Server, base.SmtpParameters.Port, base.AuthenticationParameters.NetworkCredential);
		}

		void IReplayProvider.Delete(IReadOnlyCollection<DeleteAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.Delete({0} actions)", new object[]
			{
				actions.Count
			});
			throw new ActionNotSupportedException();
		}

		void IReplayProvider.Flag(IReadOnlyCollection<FlagAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.Flag({0} actions)", new object[]
			{
				actions.Count
			});
			throw new ActionNotSupportedException();
		}

		void IReplayProvider.FlagClear(IReadOnlyCollection<FlagClearAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.FlagClear({0} actions)", new object[]
			{
				actions.Count
			});
			throw new ActionNotSupportedException();
		}

		void IReplayProvider.FlagComplete(IReadOnlyCollection<FlagCompleteAction> actions)
		{
			MrsTracer.Provider.Function("ImapSourceMailbox.FlagComplete({0} actions)", new object[]
			{
				actions.Count
			});
			throw new ActionNotSupportedException();
		}

		IReadOnlyCollection<CreateCalendarEventActionResult> IReplayProvider.CreateCalendarEvent(IReadOnlyCollection<CreateCalendarEventAction> actions)
		{
			throw new ActionNotSupportedException();
		}

		void IReplayProvider.UpdateCalendarEvent(IReadOnlyCollection<UpdateCalendarEventAction> actions)
		{
			throw new ActionNotSupportedException();
		}

		protected override void CopySingleMessage(MessageRec message, IFolderProxy folderProxy, PropTag[] propTagsToExclude, PropTag[] excludeProps)
		{
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("ImapSourceMailbox.CopySingleMessage", OperationType.None),
				new EntryIDsDataContext(message.EntryId)
			}).Execute(delegate
			{
				SyncEmailUtils.CopyMimeStream(this, message, folderProxy);
			});
		}

		private void FetchMessage(string messageUid, out Stream mimeStream, out ExDateTime? internalDate)
		{
			base.CheckDisposed();
			internalDate = null;
			mimeStream = null;
			ImapResultData messageItemByUid = base.ImapConnection.GetMessageItemByUid(messageUid, ImapConnection.MessageBodyDataItems);
			if (messageItemByUid.MessageStream == null)
			{
				throw new UnableToFetchMimeStreamException(messageUid);
			}
			mimeStream = messageItemByUid.MessageStream;
			internalDate = ((messageItemByUid.MessageInternalDates != null && messageItemByUid.MessageInternalDates.Count == 1) ? messageItemByUid.MessageInternalDates[0] : null);
		}

		private void SetReadFlags(byte[] messageEntryId, byte[] folderEntryId, bool isRead)
		{
			base.CheckDisposed();
			using (ImapFolder folder = base.GetFolder<ImapSourceFolder>(folderEntryId))
			{
				if (folder == null)
				{
					MrsTracer.Provider.Warning("Source folder {0} doesn't exist", new object[]
					{
						TraceUtils.DumpBytes(folderEntryId)
					});
					throw new ImapObjectNotFoundException(TraceUtils.DumpBytes(folderEntryId));
				}
				uint item = ImapEntryId.ParseUid(messageEntryId);
				List<uint> list = new List<uint>(1);
				list.Add(item);
				List<ImapMessageRec> list2 = folder.Folder.LookupMessages(base.ImapConnection, list);
				if (list2.Count == 0)
				{
					MrsTracer.Provider.Warning("Source message {0} doesn't exist", new object[]
					{
						TraceUtils.DumpBytes(messageEntryId)
					});
					throw new ImapObjectNotFoundException(TraceUtils.DumpBytes(messageEntryId));
				}
				ImapMailFlags imapMailFlags = list2[0].ImapMailFlags;
				ImapMailFlags imapMailFlags2 = isRead ? (imapMailFlags | ImapMailFlags.Seen) : (imapMailFlags & ~ImapMailFlags.Seen);
				if (imapMailFlags != imapMailFlags2)
				{
					string text = item.ToString(CultureInfo.InvariantCulture);
					MrsTracer.Provider.Debug("StoreMessageFlags - uid: {0}, flagsToStore: {1}, previousFlags {2}", new object[]
					{
						text,
						imapMailFlags2,
						imapMailFlags
					});
					base.ImapConnection.StoreMessageFlags(text, imapMailFlags2, imapMailFlags);
				}
			}
		}

		ResourceHealthTracker ISupportMime.get_RHTracker()
		{
			return base.RHTracker;
		}
	}
}

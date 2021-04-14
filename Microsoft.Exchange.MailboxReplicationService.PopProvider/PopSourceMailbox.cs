using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PopSourceMailbox : PopMailbox, ISourceMailbox, IMailbox, IDisposable, ISupportMime
	{
		public PopSourceMailbox(ConnectionParameters connectionParameters, Pop3AuthenticationParameters authenticationParameters, Pop3ServerParameters serverParameters, SmtpServerParameters smtpParameters) : base(connectionParameters, authenticationParameters, serverParameters, smtpParameters)
		{
		}

		internal override bool SupportsSavingSyncState
		{
			get
			{
				return true;
			}
		}

		Stream ISupportMime.GetMimeStream(MessageRec message, out PropValueData[] extraPropValues)
		{
			extraPropValues = null;
			string messageUid = PopEntryId.ParseUid(message.EntryId);
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
			MrsTracer.Provider.Function("PopSourceMailbox.GetFolder({0})", new object[]
			{
				TraceUtils.DumpEntryId(entryId)
			});
			return base.GetFolder<PopSourceFolder>(entryId);
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
			MrsTracer.Provider.Function("PopSourceMailbox.ExportMessages({0} messages)", new object[]
			{
				messages.Count
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.CopyMessagesOneByOne(messages, proxyPool, propsToCopyExplicitly, excludeProps, delegate(MessageRec curMsg)
			{
			});
		}

		void ISourceMailbox.ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			throw new NotImplementedException();
		}

		List<ReplayActionResult> ISourceMailbox.ReplayActions(List<ReplayAction> actions)
		{
			MrsTracer.Provider.Function("PopSourceMailbox.ReplayActions({0} actions)", new object[]
			{
				actions.Count
			});
			base.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			List<ReplayActionResult> list = new List<ReplayActionResult>(actions.Count);
			foreach (ReplayAction replayAction in actions)
			{
				ActionId id = replayAction.Id;
				if (id != ActionId.Send)
				{
					throw new ActionNotSupportedException();
				}
				SmtpClientHelper.Submit((SendAction)replayAction, base.SmtpParameters.Server, base.SmtpParameters.Port, base.AuthenticationParameters.NetworkCredential);
				list.Add(null);
			}
			return list;
		}

		protected override void CopySingleMessage(MessageRec message, IFolderProxy folderProxy, PropTag[] propTagsToExclude, PropTag[] excludeProps)
		{
			ExecutionContext.Create(new DataContext[]
			{
				new OperationDataContext("PopSourceMailbox.CopySingleMessage", OperationType.None),
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
			if (!base.UniqueIdMap.ContainsKey(messageUid))
			{
				this.UpdateMessageMap();
			}
			if (!base.UniqueIdMap.ContainsKey(messageUid))
			{
				return;
			}
			Pop3ResultData email = base.PopConnection.GetEmail(base.UniqueIdMap[messageUid]);
			if (email.Email == null || email.Email.MimeStream == null)
			{
				throw new UnableToFetchMimeStreamException(messageUid);
			}
			mimeStream = email.Email.MimeStream;
		}

		private void UpdateMessageMap()
		{
			base.UniqueIdMap.Clear();
			Pop3ResultData uniqueIds = base.PopConnection.GetUniqueIds();
			if (uniqueIds == null)
			{
				return;
			}
			for (int i = 1; i <= uniqueIds.EmailDropCount; i++)
			{
				string uniqueId = uniqueIds.GetUniqueId(i);
				if (uniqueId != null)
				{
					base.UniqueIdMap[uniqueId] = i;
				}
			}
		}

		ResourceHealthTracker ISupportMime.get_RHTracker()
		{
			return base.RHTracker;
		}
	}
}

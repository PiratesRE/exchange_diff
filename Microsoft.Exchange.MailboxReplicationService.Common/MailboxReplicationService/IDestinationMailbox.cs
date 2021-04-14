using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IDestinationMailbox : IMailbox, IDisposable
	{
		bool MailboxExists();

		CreateMailboxResult CreateMailbox(byte[] mailboxData, MailboxSignatureFlags sourceSignatureFlags);

		void ProcessMailboxSignature(byte[] mailboxData);

		IDestinationFolder GetFolder(byte[] entryId);

		IFxProxy GetFxProxy();

		PropProblemData[] SetProps(PropValueData[] pva);

		IFxProxyPool GetFxProxyPool(ICollection<byte[]> folderIds);

		void CreateFolder(FolderRec sourceFolder, CreateFolderFlags createFolderFlags, out byte[] newFolderId);

		void MoveFolder(byte[] folderId, byte[] oldParentId, byte[] newParentId);

		void DeleteFolder(FolderRec folderRec);

		void SetMailboxSecurityDescriptor(RawSecurityDescriptor sd);

		void SetUserSecurityDescriptor(RawSecurityDescriptor sd);

		void PreFinalSyncDataProcessing(int? sourceMailboxVersion);

		ConstraintCheckResultType CheckDataGuarantee(DateTime commitTimestamp, out LocalizedString failureReason);

		void ForceLogRoll();

		List<ReplayAction> GetActions(string replaySyncState, int maxNumberOfActions);

		void SetMailboxSettings(ItemPropertiesBase item);
	}
}

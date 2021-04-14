using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface ISourceMailbox : IMailbox, IDisposable
	{
		byte[] GetMailboxBasicInfo(MailboxSignatureFlags flags);

		ISourceFolder GetFolder(byte[] entryId);

		void CopyTo(IFxProxy destMailbox, PropTag[] excludeProps);

		void SetMailboxSyncState(string syncStateStr);

		string GetMailboxSyncState();

		MailboxChangesManifest EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags flags, int maxChanges);

		void ExportMessages(List<MessageRec> messages, IFxProxyPool proxyPool, ExportMessagesFlags flags, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps);

		void ExportFolders(List<byte[]> folderIds, IFxProxyPool proxyPool, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags);

		List<ReplayActionResult> ReplayActions(List<ReplayAction> actions);

		List<ItemPropertiesBase> GetMailboxSettings(GetMailboxSettingsFlags flags);
	}
}

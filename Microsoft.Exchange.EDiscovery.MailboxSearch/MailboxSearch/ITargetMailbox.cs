using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal interface ITargetMailbox : ITarget, IDisposable
	{
		bool ExportLocationExist { get; }

		bool WorkingLocationExist { get; }

		string PrimarySmtpAddress { get; }

		string LegacyDistinguishedName { get; }

		IEwsClient EwsClientInstance { get; }

		string CreateResultFolder(string resultFolderName);

		void PreRemoveSearchResults(bool removeLogs);

		void RemoveSearchResults();

		BaseFolderType GetFolder(string folderId);

		BaseFolderType GetFolderByName(BaseFolderIdType parentFolderId, string folderName);

		BaseFolderType CreateFolder(BaseFolderIdType parentFolderId, string newFolderName, bool isHidden);

		List<ItemInformation> CopyItems(string parentFolderId, IList<ItemInformation> items);

		void CreateOrUpdateSearchLogEmail(MailboxDiscoverySearch searchObject, List<string> successfulMailboxes, List<string> unsuccessfulMailboxes);

		void WriteExportRecordLog(MailboxDiscoverySearch searchObject, IEnumerable<ExportRecord> records);

		void AttachDiscoveryLogFiles();
	}
}

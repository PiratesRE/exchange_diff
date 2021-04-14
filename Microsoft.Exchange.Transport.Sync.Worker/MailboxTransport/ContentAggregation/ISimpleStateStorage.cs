using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISimpleStateStorage : IDisposeTrackable, IDisposable
	{
		SyncStateStorage SyncStateStorage { get; }

		void SetForceRecoverySyncNext(bool forceRecoverySyncNext);

		void AddProperty(string property, string value);

		bool TryGetPropertyValue(string property, out string value);

		bool TryRemoveProperty(string property);

		void ChangePropertyValue(string property, string value);

		bool ContainsProperty(string property);

		bool TryFindItem(string cloudId, out string cloudFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties);

		bool TryFindItem(string cloudId, out string cloudFolderId, out string cloudVersion);

		bool TryFindFolder(string cloudId, out string cloudFolderId, out string cloudVersion);

		bool ContainsItem(string cloudId);

		bool ContainsFailedItem(string cloudId);

		bool ContainsFolder(string cloudId);

		bool ContainsFailedFolder(string cloudId);

		IEnumerator<string> GetCloudItemEnumerator();

		IEnumerator<string> GetCloudItemFilteredByCloudFolderIdEnumerator(string cloudFolderId);

		IEnumerator<string> GetFailedCloudItemEnumerator();

		IEnumerator<string> GetFailedCloudItemFilteredByCloudFolderIdEnumerator(string cloudFolderId);

		bool TryUpdateItemCloudVersion(string cloudId, string cloudVersion);

		IEnumerator<string> GetCloudFolderEnumerator();

		IEnumerator<string> GetCloudFolderFilteredByCloudFolderIdEnumerator(string cloudFolderId);

		bool TryFindFolder(string cloudId, out string cloudFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties);

		bool TryUpdateFolder(ISyncWorkerData subscription, string cloudId, string newCloudId, string cloudVersion);

		bool TryUpdateFolderCloudVersion(string cloudId, string cloudVersion);
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface INativeStateStorage : ISimpleStateStorage, IDisposeTrackable, IDisposable
	{
		bool ContainsItem(StoreObjectId nativeId);

		IEnumerator<StoreObjectId> GetNativeItemEnumerator();

		bool TryAddItem(string cloudId, string cloudFolderId, StoreObjectId nativeId, byte[] changeKey, StoreObjectId nativeFolderId, string cloudVersion, Dictionary<string, string> itemProperties);

		bool TryFindItem(string cloudId, out string cloudFolderId, out StoreObjectId nativeId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties);

		bool TryFindItem(StoreObjectId nativeId, out string cloudId, out string cloudFolderId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> itemProperties);

		bool TryUpdateItem(StoreObjectId nativeId, byte[] changeKey, string cloudVersion, Dictionary<string, string> itemProperties);

		bool TryRemoveItem(string cloudId);

		bool TryRemoveItem(StoreObjectId nativeId);

		bool ContainsFolder(StoreObjectId nativeId);

		IEnumerator<StoreObjectId> GetNativeFolderEnumerator();

		bool TryAddFolder(bool isInbox, string cloudId, string cloudFolderId, StoreObjectId nativeId, byte[] changeKey, StoreObjectId nativeFolderId, string cloudVersion, Dictionary<string, string> folderProperties);

		bool TryFindFolder(StoreObjectId nativeId, out string cloudId, out string cloudFolderId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties);

		bool TryFindFolder(string cloudId, out string cloudFolderId, out StoreObjectId nativeId, out byte[] changeKey, out StoreObjectId nativeFolderId, out string cloudVersion, out Dictionary<string, string> folderProperties);

		bool TryUpdateFolder(bool isInbox, StoreObjectId nativeId, StoreObjectId newNativeId, string cloudId, string newCloudId, string newCloudFolderId, byte[] changeKey, StoreObjectId newNativeFolderId, string cloudVersion, Dictionary<string, string> folderProperties);

		bool TryUpdateFolder(bool isInbox, StoreObjectId nativeId, StoreObjectId newNativeId);

		bool TryRemoveFolder(string cloudId);

		bool TryRemoveFolder(StoreObjectId nativeId);

		IEnumerator<StoreObjectId> GetNativeItemFilteredByNativeFolderIdEnumerator(StoreObjectId nativeFolderId);

		IEnumerator<StoreObjectId> GetNativeFolderFilteredByNativeFolderIdEnumerator(StoreObjectId nativeFolderId);
	}
}

using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISharingSubscriptionData
	{
		VersionedId Id { get; }

		SharingDataType DataType { get; }

		string RemoteFolderName { get; }

		StoreObjectId LocalFolderId { get; }
	}
}

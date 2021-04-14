using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncProviderFactory
	{
		ISyncProvider CreateSyncProvider(ISyncLogger syncLogger = null);

		byte[] GetCollectionIdBytes();

		void SetCollectionIdFromBytes(byte[] collectionBytes);
	}
}

using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISynchronizationUploadContextBase : IDisposable
	{
		StoreSession Session { get; }

		StoreObjectId SyncRootFolderId { get; }

		void GetCurrentState(ref StorageIcsState currentState);
	}
}

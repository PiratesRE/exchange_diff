using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum AmDbActionCategory
	{
		None = 10,
		Mount,
		Dismount,
		Move,
		Remount,
		ForceDismount,
		SyncState,
		SyncAd
	}
}

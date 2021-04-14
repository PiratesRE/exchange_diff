using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum LegacyAmDbActionCode
	{
		Unknown = 1,
		AdminMount,
		AdminDismount,
		AdminMove,
		StartupAutoMount,
		StoreRestartMount,
		AutomaticFailover,
		SwitchOver,
		Remount
	}
}

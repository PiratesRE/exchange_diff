using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum DeleteItemFlags
	{
		None = 0,
		SoftDelete = 1,
		HardDelete = 2,
		MoveToDeletedItems = 4,
		NormalizedDeleteFlags = 7,
		SuppressReadReceipt = 256,
		DeclineCalendarItemWithResponse = 4096,
		DeclineCalendarItemWithoutResponse = 8192,
		CancelCalendarItem = 16384,
		EmptyFolder = 65536,
		DeleteAllClutter = 131072,
		ClutterActionByUserOverride = 262144
	}
}

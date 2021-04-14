using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	[Serializable]
	public enum MigrationFlags
	{
		[LocDescription(ServerStrings.IDs.MigrationFlagsNone)]
		None = 0,
		[LocDescription(ServerStrings.IDs.MigrationFlagsStart)]
		Start = 1,
		[LocDescription(ServerStrings.IDs.MigrationFlagsStop)]
		Stop = 2,
		[LocDescription(ServerStrings.IDs.MigrationFlagsRemove)]
		Remove = 4,
		[LocDescription(ServerStrings.IDs.MigrationFlagsReport)]
		Report = 8
	}
}

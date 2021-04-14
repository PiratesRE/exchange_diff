using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	[Serializable]
	public enum MigrationBatchSupportedActions
	{
		[LocDescription(ServerStrings.IDs.MigrationBatchSupportedActionNone)]
		None = 0,
		[LocDescription(ServerStrings.IDs.MigrationBatchSupportedActionStart)]
		Start = 1,
		[LocDescription(ServerStrings.IDs.MigrationBatchSupportedActionStop)]
		Stop = 2,
		[LocDescription(ServerStrings.IDs.MigrationBatchSupportedActionSet)]
		Set = 4,
		[LocDescription(ServerStrings.IDs.MigrationBatchSupportedActionRemove)]
		Remove = 8,
		[LocDescription(ServerStrings.IDs.MigrationBatchSupportedActionComplete)]
		Complete = 16,
		[LocDescription(ServerStrings.IDs.MigrationBatchSupportedActionAppend)]
		Append = 32
	}
}

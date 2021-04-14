using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationState
	{
		[LocDescription(ServerStrings.IDs.MigrationStateActive)]
		Active,
		[LocDescription(ServerStrings.IDs.MigrationStateWaiting)]
		Waiting,
		[LocDescription(ServerStrings.IDs.MigrationStateCompleted)]
		Completed,
		[LocDescription(ServerStrings.IDs.MigrationStateStopped)]
		Stopped,
		[LocDescription(ServerStrings.IDs.MigrationStateFailed)]
		Failed,
		[LocDescription(ServerStrings.IDs.MigrationStateCorrupted)]
		Corrupted,
		[LocDescription(ServerStrings.IDs.MigrationStateDisabled)]
		Disabled
	}
}

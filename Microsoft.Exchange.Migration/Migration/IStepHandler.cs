using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal interface IStepHandler
	{
		bool ExpectMailboxData { get; }

		IStepSettings Discover(MigrationJobItem jobItem, MailboxData localMailbox);

		void Validate(MigrationJobItem jobItem);

		IStepSnapshot Inject(MigrationJobItem jobItem);

		IStepSnapshot Process(ISnapshotId id, MigrationJobItem jobItem, out bool updated);

		void Start(ISnapshotId id);

		IStepSnapshot Stop(ISnapshotId id);

		void Delete(ISnapshotId id);

		bool CanProcess(MigrationJobItem jobItem);

		MigrationUserStatus ResolvePresentationStatus(MigrationFlags flags, IStepSnapshot stepSnapshot = null);
	}
}

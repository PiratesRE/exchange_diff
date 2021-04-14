using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MigrationJobCompletionInitializingProcessor : JobProcessor
	{
		internal static MigrationJobCompletionInitializingProcessor CreateProcessor(MigrationType type)
		{
			if (type <= MigrationType.BulkProvisioning)
			{
				if (type == MigrationType.IMAP || type == MigrationType.ExchangeOutlookAnywhere)
				{
					throw new NotSupportedException("Exchange/IMAP not supported in CompletionInitializing state");
				}
				if (type == MigrationType.BulkProvisioning)
				{
					throw new NotSupportedException("Bulk Provisioning not supported in CompletionInitializing state");
				}
			}
			else
			{
				if (type == MigrationType.ExchangeRemoteMove || type == MigrationType.ExchangeLocalMove)
				{
					return new MigrationJobCompletionInitializingProcessor();
				}
				if (type == MigrationType.PublicFolder)
				{
					return new PublicFolderJobCompletionInitializingProcessor();
				}
			}
			throw new ArgumentException("Invalid MigrationType " + type);
		}

		internal override bool Validate()
		{
			return base.Job.Status == MigrationJobStatus.CompletionInitializing;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			return MigrationJobStatus.CompletionStarting;
		}

		protected override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			return LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationJobCompletionInitializingProcessor>(this);
		}
	}
}

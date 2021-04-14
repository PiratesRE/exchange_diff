using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MigrationJobValidatingProcessor : JobProcessor
	{
		protected virtual bool IsProvisioningSupported
		{
			get
			{
				return base.Job.IsProvisioningSupported;
			}
		}

		protected virtual bool IsValidationSupported
		{
			get
			{
				return base.SubscriptionHandler.SupportsAdvancedValidation;
			}
		}

		protected virtual bool IsValidationEnabled
		{
			get
			{
				return base.Job.UseAdvancedValidation;
			}
		}

		internal static MigrationJobValidatingProcessor CreateProcessor(MigrationType type)
		{
			if (type <= MigrationType.BulkProvisioning)
			{
				if (type == MigrationType.IMAP || type == MigrationType.ExchangeOutlookAnywhere || type == MigrationType.BulkProvisioning)
				{
					throw new ArgumentException("This MigrationType does not support Validation!: " + type);
				}
			}
			else if (type == MigrationType.ExchangeRemoteMove || type == MigrationType.ExchangeLocalMove || type == MigrationType.PublicFolder)
			{
				return new MigrationJobValidatingProcessor();
			}
			throw new ArgumentException("Invalid MigrationType " + type);
		}

		internal override bool Validate()
		{
			return true;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			if (this.IsProvisioningSupported)
			{
				return MigrationJobStatus.ProvisionStarting;
			}
			return MigrationJobStatus.SyncStarting;
		}

		protected sealed override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			legacyMigrationJobProcessorResponse.NumItemsOutstanding = new int?(0);
			legacyMigrationJobProcessorResponse.NumItemsProcessed = new int?(0);
			if (!this.IsValidationSupported)
			{
				return legacyMigrationJobProcessorResponse;
			}
			JobItemOperationResult jobItemOperationResult = base.FindAndRunJobItemOperation(MigrationJobValidatingProcessor.PollingValidationStatuses, MigrationUserStatus.Failed, ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationPollingBatchSize"), (MigrationUserStatus status, int itemCount) => base.Job.GetItemsByStatus(base.DataProvider, status, new int?(itemCount)), delegate(MigrationJobItem jobItem)
			{
				if (this.IsValidationEnabled && jobItem.SupportsAdvancedValidation)
				{
					return base.SubscriptionHandler.TestCreateUnderlyingSubscriptions(jobItem);
				}
				jobItem.SetStatus(base.DataProvider, this.IsProvisioningSupported ? MigrationUserStatus.Provisioning : MigrationUserStatus.Queued);
				return true;
			});
			legacyMigrationJobProcessorResponse.NumItemsProcessed = new int?(jobItemOperationResult.NumItemsProcessed);
			legacyMigrationJobProcessorResponse.NumItemsTransitioned = new int?(jobItemOperationResult.NumItemsTransitioned);
			legacyMigrationJobProcessorResponse.NumItemsOutstanding = new int?(base.GetJobItemCount(MigrationJobValidatingProcessor.PollingValidationStatuses));
			if (legacyMigrationJobProcessorResponse.NumItemsProcessed > 0 || legacyMigrationJobProcessorResponse.NumItemsOutstanding > 0)
			{
				legacyMigrationJobProcessorResponse.Result = MigrationProcessorResult.Working;
			}
			return legacyMigrationJobProcessorResponse;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationJobValidatingProcessor>(this);
		}

		private static readonly MigrationUserStatus[] PollingValidationStatuses = new MigrationUserStatus[]
		{
			MigrationUserStatus.Validating
		};
	}
}

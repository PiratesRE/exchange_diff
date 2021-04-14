using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal interface ILegacySubscriptionHandler : IForceReportDisposeTrackable, IDisposeTrackable, IDisposable
	{
		bool SupportsDupeDetection { get; }

		bool SupportsActiveIncrementalSync { get; }

		bool SupportsAdvancedValidation { get; }

		MigrationType SupportedMigrationType { get; }

		bool CreateUnderlyingSubscriptions(MigrationJobItem jobItem);

		void DeleteUnderlyingSubscriptions(MigrationJobItem jobItem);

		void DisableSubscriptions(MigrationJobItem jobItem);

		void ResumeUnderlyingSubscriptions(MigrationUserStatus startedStatus, MigrationJobItem jobItem);

		MigrationProcessorResult SyncToUnderlyingSubscriptions(MigrationJobItem jobItem);

		void CancelUnderlyingSubscriptions(MigrationJobItem jobItem);

		void StopUnderlyingSubscriptions(MigrationJobItem jobItem);

		bool TestCreateUnderlyingSubscriptions(MigrationJobItem jobItem);

		void SyncSubscriptionSettings(MigrationJobItem jobItem);

		IEnumerable<MigrationJobItem> GetJobItemsForSubscriptionCheck(ExDateTime? cutoffTime, MigrationUserStatus status, int maxItemsToCheck);
	}
}

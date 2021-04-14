using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal class AsyncNotificationAdapter : IAsyncNotificationAdapter
	{
		private AsyncNotificationAdapter()
		{
		}

		Guid? IAsyncNotificationAdapter.CreateNotification(IMigrationDataProvider dataProvider, MigrationJob job)
		{
			ADRecipientOrAddress owner = null;
			if (job.OwnerId == null)
			{
				if (!(job.OwnerExchangeObjectId != Guid.Empty))
				{
					goto IL_A0;
				}
			}
			try
			{
				ADRecipient adrecipient;
				if (job.OwnerExchangeObjectId != Guid.Empty)
				{
					adrecipient = dataProvider.ADProvider.GetADRecipientByExchangeObjectId(job.OwnerExchangeObjectId);
				}
				else
				{
					adrecipient = dataProvider.ADProvider.GetADRecipientByObjectId(job.OwnerId);
				}
				if (adrecipient != null && !string.IsNullOrEmpty((string)adrecipient[ADRecipientSchema.LegacyExchangeDN]))
				{
					Participant participant = new Participant(adrecipient);
					owner = new ADRecipientOrAddress(participant);
				}
			}
			catch (LocalizedException ex)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Error fetching the owner while creating async notification: {0}", new object[]
				{
					ex
				});
			}
			IL_A0:
			AsyncOperationNotificationDataProvider.CreateNotification(dataProvider.OrganizationId, job.JobId.ToString(), AsyncOperationType.Migration, AsyncOperationStatus.Created, new LocalizedString(job.JobName), owner, AsyncNotificationAdapter.GetExtendedData(job), false);
			MigrationLogger.Log(MigrationEventType.Verbose, "Created new async notification for job {0}", new object[]
			{
				job.JobName
			});
			return new Guid?(job.JobId);
		}

		void IAsyncNotificationAdapter.UpdateNotification(IMigrationDataProvider dataProvider, MigrationJob job)
		{
			if (job.NotificationId == null)
			{
				job.NotificationId = ((IAsyncNotificationAdapter)this).CreateNotification(dataProvider, job);
				job.SaveBatchFlagsAndNotificationId(dataProvider);
			}
			AsyncOperationStatus asyncOperationStatus;
			switch (job.Status)
			{
			case MigrationJobStatus.Created:
				asyncOperationStatus = AsyncOperationStatus.Created;
				break;
			case MigrationJobStatus.SyncInitializing:
			case MigrationJobStatus.CompletionInitializing:
			case MigrationJobStatus.Stopped:
				asyncOperationStatus = AsyncOperationStatus.Queued;
				break;
			case MigrationJobStatus.SyncStarting:
			case MigrationJobStatus.SyncCompleting:
			case MigrationJobStatus.ProvisionStarting:
			case MigrationJobStatus.Validating:
				asyncOperationStatus = AsyncOperationStatus.InProgress;
				break;
			case MigrationJobStatus.SyncCompleted:
				asyncOperationStatus = (job.SupportsMultiBatchFinalization ? AsyncOperationStatus.WaitingForFinalization : AsyncOperationStatus.Completed);
				break;
			case MigrationJobStatus.CompletionStarting:
			case MigrationJobStatus.Completing:
				asyncOperationStatus = AsyncOperationStatus.Completing;
				break;
			case MigrationJobStatus.Completed:
				asyncOperationStatus = AsyncOperationStatus.Completed;
				break;
			case MigrationJobStatus.Failed:
			case MigrationJobStatus.Corrupted:
				asyncOperationStatus = AsyncOperationStatus.Failed;
				break;
			case MigrationJobStatus.Removed:
			case MigrationJobStatus.Removing:
				asyncOperationStatus = AsyncOperationStatus.Removing;
				break;
			default:
				asyncOperationStatus = AsyncOperationStatus.InProgress;
				break;
			}
			if (job.NotificationId != null)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Updated async notification for job {0} with status {1} based on job staus {2}", new object[]
				{
					job.JobName,
					asyncOperationStatus,
					job.Status
				});
				AsyncOperationNotificationDataProvider.UpdateNotification(dataProvider.OrganizationId, job.NotificationId.Value.ToString(), new AsyncOperationStatus?(asyncOperationStatus), null, null, false, AsyncNotificationAdapter.GetExtendedData(job));
			}
		}

		void IAsyncNotificationAdapter.RemoveNotification(IMigrationDataProvider dataProvider, MigrationJob job)
		{
			if (job.NotificationId == null)
			{
				return;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "Removed async notification for job {0}", new object[]
			{
				job.JobName
			});
			AsyncOperationNotificationDataProvider.RemoveNotification(dataProvider.OrganizationId, job.NotificationId.Value.ToString(), false);
		}

		private static KeyValuePair<string, LocalizedString>[] GetExtendedData(MigrationJob job)
		{
			return new KeyValuePair<string, LocalizedString>[]
			{
				new KeyValuePair<string, LocalizedString>(AsyncNotificationAdapter.TotalItemCount, new LocalizedString(job.TotalItemCount.ToString(CultureInfo.InvariantCulture))),
				new KeyValuePair<string, LocalizedString>(AsyncNotificationAdapter.TotalFailedCount, new LocalizedString(job.FailedItemCount.ToString(CultureInfo.InvariantCulture))),
				new KeyValuePair<string, LocalizedString>(AsyncNotificationAdapter.TotalCompletedCount, new LocalizedString(job.FinalizedItemCount.ToString(CultureInfo.InvariantCulture))),
				new KeyValuePair<string, LocalizedString>(AsyncNotificationAdapter.TotalSyncedCount, new LocalizedString(job.SyncedItemCount.ToString(CultureInfo.InvariantCulture))),
				new KeyValuePair<string, LocalizedString>(AsyncNotificationAdapter.MigrationType, new LocalizedString(job.MigrationType.ToString())),
				new KeyValuePair<string, LocalizedString>(AsyncNotificationAdapter.TotalCompletedWithErrorCount, new LocalizedString((job.FailedOtherItemCount + job.FailedFinalizationItemCount).ToString(CultureInfo.InvariantCulture)))
			};
		}

		public static readonly string TotalItemCount = "TotalItemCount";

		public static readonly string TotalCompletedCount = "TotalCompletedCount";

		public static readonly string TotalSyncedCount = "TotalSyncedCount";

		public static readonly string TotalFailedCount = "TotalFailedCount";

		public static readonly string MigrationType = "MigrationType";

		public static readonly string TotalCompletedWithErrorCount = "TotalCompletedWithErrorCount";

		public static readonly IAsyncNotificationAdapter Empty = new AsyncNotificationAdapter.NullNotificationAdapter();

		public static readonly IAsyncNotificationAdapter Instance = new AsyncNotificationAdapter();

		private class NullNotificationAdapter : IAsyncNotificationAdapter
		{
			Guid? IAsyncNotificationAdapter.CreateNotification(IMigrationDataProvider dataProvider, MigrationJob job)
			{
				return null;
			}

			void IAsyncNotificationAdapter.UpdateNotification(IMigrationDataProvider dataProvider, MigrationJob job)
			{
			}

			void IAsyncNotificationAdapter.RemoveNotification(IMigrationDataProvider dataProvider, MigrationJob job)
			{
			}
		}
	}
}

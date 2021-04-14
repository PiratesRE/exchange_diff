using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal sealed class MigrationScheduler : MigrationComponent
	{
		internal MigrationScheduler(string name, WaitHandle stopEvent) : base(name, stopEvent)
		{
		}

		internal override bool ShouldProcess()
		{
			return true;
		}

		internal override bool Process(IMigrationJobCache cache)
		{
			MigrationUtil.ThrowOnNullArgument(cache, "cache");
			bool result = false;
			List<MigrationCacheEntry> list = cache.Get();
			foreach (MigrationCacheEntry migrationCacheEntry in list)
			{
				if (base.StopEvent.WaitOne(0, false))
				{
					break;
				}
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
				try
				{
					MigrationProcessorResponse migrationProcessorResponse;
					try
					{
						MigrationLogContext.Current.Organization = migrationCacheEntry.OrganizationId;
						MigrationLogger.Log(MigrationEventType.Verbose, "Looking at cache entry {0}", new object[]
						{
							migrationCacheEntry
						});
						if (migrationCacheEntry.NextProcessTime != null && migrationCacheEntry.NextProcessTime.Value > ExDateTime.UtcNow)
						{
							MigrationLogger.Log(MigrationEventType.Verbose, "Skipping cache entry {0} until time {1}", new object[]
							{
								migrationCacheEntry,
								migrationCacheEntry.NextProcessTime
							});
							continue;
						}
						migrationProcessorResponse = MigrationScheduler.ProcessCacheEntry(migrationCacheEntry);
					}
					catch (CannotResolveExternalDirectoryOrganizationIdException ex)
					{
						if (ExDateTime.UtcNow - MigrationScheduler.MaxADMissingObjectWait > migrationCacheEntry.CreationTime)
						{
							MigrationLogger.Log(MigrationEventType.Error, ex, "Failed to resolve organization while processing cache entry for too long, removing cache entry.", new object[0]);
							migrationProcessorResponse = MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
						}
						else
						{
							MigrationApplication.NotifyOfTransientException(ex, "Process: " + migrationCacheEntry);
							migrationProcessorResponse = MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
						}
					}
					switch (migrationProcessorResponse.Result)
					{
					case MigrationProcessorResult.Working:
						migrationCacheEntry.UpdateFromLastRun(migrationProcessorResponse.Result, null, false);
						result = true;
						continue;
					case MigrationProcessorResult.Waiting:
						migrationCacheEntry.UpdateFromLastRun(migrationProcessorResponse.Result, migrationProcessorResponse.DelayTime, false);
						continue;
					case MigrationProcessorResult.Failed:
						MigrationLogger.Log(MigrationEventType.Error, "marking migration job cache entry failed.. {0}", new object[]
						{
							migrationCacheEntry
						});
						cache.Remove(migrationCacheEntry);
						continue;
					case MigrationProcessorResult.Deleted:
						MigrationLogger.Log(MigrationEventType.Information, "Removing deleted migration job cache entry {0}", new object[]
						{
							migrationCacheEntry
						});
						cache.Remove(migrationCacheEntry);
						result = true;
						continue;
					case MigrationProcessorResult.Suspended:
						MigrationLogger.Log(MigrationEventType.Information, "Suspending migration job cache entry {0}", new object[]
						{
							migrationCacheEntry
						});
						migrationCacheEntry.Suspend();
						continue;
					}
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid MigrationProcessorResult {0}", new object[]
					{
						migrationProcessorResponse.Result
					}));
				}
				finally
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
					Thread.CurrentThread.CurrentUICulture = currentUICulture;
					MigrationLogContext.Current.Organization = null;
				}
			}
			return result;
		}

		private static MigrationProcessorResponse ProcessCacheEntry(MigrationCacheEntry cacheEntry)
		{
			MigrationUtil.ThrowOnNullArgument(cacheEntry, "cacheEntry");
			string str = cacheEntry.ToString();
			MigrationLogger.Log(MigrationEventType.Information, "ProcessCacheEntry: beginning processing {0}", new object[]
			{
				cacheEntry
			});
			MigrationProcessorResponse result;
			try
			{
				using (IMigrationDataProvider migrationDataProvider = MigrationServiceFactory.Instance.CreateProviderForMigrationMailbox(cacheEntry.TenantPartitionHint, cacheEntry.MigrationMailboxLegacyDN))
				{
					str = migrationDataProvider.MailboxName;
					MigrationSession migrationSession;
					if (!MigrationApplication.TryGetEnabledMigrationSession(migrationDataProvider, true, out migrationSession) || migrationSession == null)
					{
						return MigrationProcessorResponse.CreateWaitingMax();
					}
					migrationSession.CheckAndUpgradeToSupportedFeaturesAndVersion(migrationDataProvider);
					if (migrationSession.Config.IsSupported(MigrationFeature.PAW))
					{
						MigrationSessionProcessor migrationSessionProcessor = new MigrationSessionProcessor(migrationSession, migrationDataProvider);
						result = migrationSessionProcessor.Process();
					}
					else
					{
						result = LegacyMigrationProcessor.ProcessSession(migrationSession, migrationDataProvider);
					}
				}
			}
			catch (ObjectNotFoundException ex)
			{
				if (ex.InnerException is MapiExceptionNotFound || ex is UnableToFindServerForDatabaseException)
				{
					MigrationApplication.NotifyOfTransientException(ex, "ProcessCacheEntry: " + str);
					return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
				}
				MigrationApplication.NotifyOfCriticalError(ex, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
			}
			catch (MigrationMailboxNotFoundOnServerException ex2)
			{
				MigrationLogger.Log(MigrationEventType.Warning, ex2, "ProcessCacheEntry: cache entry {0} moved to {1}", new object[]
				{
					cacheEntry,
					ex2.HostServer
				});
				return MigrationProcessorResponse.Create(MigrationScheduler.AddCacheEntryToServer(ex2.HostServer, cacheEntry), null, null);
			}
			catch (MailboxInfoStaleException ex3)
			{
				MigrationApplication.NotifyOfTransientException(ex3, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
			}
			catch (TransientException ex4)
			{
				MigrationApplication.NotifyOfTransientException(ex4, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
			}
			catch (MailboxUnavailableException ex5)
			{
				if (ExDateTime.UtcNow - MigrationScheduler.MaxADMissingObjectWait > cacheEntry.CreationTime)
				{
					return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
				}
				MigrationApplication.NotifyOfTransientException(ex5, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
			}
			catch (MigrationObjectNotFoundInADException ex6)
			{
				if (ExDateTime.UtcNow - MigrationScheduler.MaxADMissingObjectWait > cacheEntry.CreationTime)
				{
					return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
				}
				MigrationApplication.NotifyOfTransientException(ex6, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex7)
			{
				if (ExDateTime.UtcNow - MigrationScheduler.MaxADMissingObjectWait > cacheEntry.CreationTime)
				{
					MigrationLogger.Log(MigrationEventType.Error, ex7, "Failed to resolve organization while processing cache entry for too long, failing cache entry.", new object[0]);
					return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
				}
				MigrationApplication.NotifyOfTransientException(ex7, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
			}
			catch (MailboxCrossSiteFailoverException ex8)
			{
				MigrationApplication.NotifyOfTransientException(ex8, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
			}
			catch (StoragePermanentException ex9)
			{
				if (MigrationUtil.IsTransientException(ex9))
				{
					MigrationApplication.NotifyOfTransientException(ex9, "ProcessCacheEntry: " + str);
					return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
				}
				MigrationApplication.NotifyOfCriticalError(ex9, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
			}
			catch (MigrationPermanentException ex10)
			{
				MigrationApplication.NotifyOfCriticalError(ex10, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
			}
			catch (MigrationDataCorruptionException ex11)
			{
				MigrationApplication.NotifyOfCriticalError(ex11, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
			}
			catch (InvalidDataException ex12)
			{
				MigrationApplication.NotifyOfCriticalError(ex12, "ProcessCacheEntry: " + str);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
			}
			return result;
		}

		private static MigrationProcessorResult AddCacheEntryToServer(string serverName, MigrationCacheEntry cacheEntry)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(serverName, "serverName");
			MigrationUtil.ThrowOnNullArgument(cacheEntry, "cacheEntry");
			IMigrationNotification migrationNotificationClient = MigrationServiceFactory.Instance.GetMigrationNotificationClient(serverName);
			try
			{
				migrationNotificationClient.RegisterMigrationBatch(new RegisterMigrationBatchArgs(cacheEntry.MdbGuid, cacheEntry.MigrationMailboxLegacyDN, cacheEntry.OrganizationId, false));
			}
			catch (MigrationObjectNotHostedException exception)
			{
				MigrationLogger.Log(MigrationEventType.Warning, exception, "AddCacheEntryToServer: server {0} thinks cache entry {1} belongs to {2}, but that server doesn't agree", new object[]
				{
					MigrationServiceFactory.Instance.GetLocalServerFqdn(),
					cacheEntry,
					serverName
				});
				return MigrationProcessorResult.Waiting;
			}
			catch (MigrationServiceRpcTransientException ex)
			{
				MigrationApplication.NotifyOfTransientException(ex, "AddCacheEntryToServer: " + cacheEntry);
				return MigrationProcessorResult.Waiting;
			}
			catch (MigrationServiceRpcException ex2)
			{
				MigrationApplication.NotifyOfPermanentException(ex2, "AddCacheEntryToServer: " + cacheEntry);
				return MigrationProcessorResult.Waiting;
			}
			return MigrationProcessorResult.Deleted;
		}

		private static MigrationProcessorResult UpdateJobStatus(IMigrationDataProvider dataProvider, MigrationJob job, MigrationJobStatus nextStatus)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationJobStatus status = job.Status;
			MigrationLogger.Log(MigrationEventType.Information, "Job {0} moving from status {1} to {2}", new object[]
			{
				job,
				status,
				nextStatus
			});
			if (nextStatus == MigrationJobStatus.Removed)
			{
				return MigrationProcessorResult.Deleted;
			}
			return MigrationProcessorResult.Working;
		}

		private static readonly TimeSpan MaxADMissingObjectWait = TimeSpan.FromDays(7.0);
	}
}

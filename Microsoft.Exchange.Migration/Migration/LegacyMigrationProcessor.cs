using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal class LegacyMigrationProcessor
	{
		internal static MigrationProcessorResponse ProcessSession(MigrationSession session, IMigrationDataProvider dataProvider)
		{
			MigrationProcessorResponse migrationProcessorResponse = MigrationProcessorResponse.Create((session.TotalJobCount > 0) ? MigrationProcessorResult.Suspended : MigrationProcessorResult.Deleted, null, null);
			IEnumerable<MigrationJob> enumerable = session.FindJobsToPickUp(dataProvider);
			IUpgradeConstraintAdapter upgradeConstraintAdapter = MigrationServiceFactory.Instance.GetUpgradeConstraintAdapter(session);
			upgradeConstraintAdapter.AddUpgradeConstraintIfNeeded(dataProvider, session);
			foreach (MigrationJob migrationJob in enumerable)
			{
				MigrationProcessorResponse migrationProcessorResponse2 = LegacyMigrationProcessor.RunJobOperation(dataProvider, session, migrationJob, new Func<IMigrationDataProvider, MigrationSession, MigrationJob, LegacyMigrationJobProcessorResponse>(LegacyMigrationProcessor.ProcessJob));
				if (migrationProcessorResponse2.Result == MigrationProcessorResult.Deleted)
				{
					MigrationLogger.Log(MigrationEventType.Information, "Removing job because processor result is Deleted", new object[0]);
					session.RemoveJob(dataProvider, migrationJob.JobId);
					migrationProcessorResponse2 = MigrationProcessorResponse.Create(MigrationProcessorResult.Working, null, null);
				}
				migrationProcessorResponse = MigrationProcessorResponse.MergeResponses<MigrationProcessorResponse>(migrationProcessorResponse, migrationProcessorResponse2);
			}
			if (migrationProcessorResponse.Result == MigrationProcessorResult.Deleted && session.TotalJobCount > 0)
			{
				MigrationLogger.Log(MigrationEventType.Warning, string.Format("setting processor result status to suspended as there are {0} non-active batches", session.TotalJobCount), new object[0]);
				migrationProcessorResponse = MigrationProcessorResponse.Create(MigrationProcessorResult.Suspended, null, null);
			}
			if (migrationProcessorResponse.Result == MigrationProcessorResult.Suspended)
			{
				int runnableJobCount = session.RunnableJobCount;
				int activeJobCount = session.ActiveJobCount;
				if (runnableJobCount > 0)
				{
					MigrationLogger.Log(MigrationEventType.Warning, string.Format("setting processor result status to working as there are {0} runnable batches", runnableJobCount), new object[0]);
					migrationProcessorResponse = MigrationProcessorResponse.Create(MigrationProcessorResult.Working, null, null);
				}
				else if (activeJobCount > 0)
				{
					MigrationLogger.Log(MigrationEventType.Information, string.Format("setting processor result status to waiting as there are {0} active batches", activeJobCount), new object[0]);
					migrationProcessorResponse = MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null);
				}
			}
			return migrationProcessorResponse;
		}

		internal static LegacyMigrationJobProcessorResponse ProcessJob(IMigrationDataProvider dataProvider, MigrationSession session, MigrationJob job)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse;
			using (JobProcessor jobProcessor = MigrationServiceFactory.Instance.CreateJobProcessor(job))
			{
				if (jobProcessor == null)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "ProcessJob: Skipping job {0} because no processor found for it", new object[]
					{
						job
					});
					return LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Waiting, null);
				}
				jobProcessor.Initialize(dataProvider, session, job);
				if (!jobProcessor.Validate())
				{
					MigrationLogger.Log(MigrationEventType.Error, "Job {0} is inconsistent with the processor {1}, marking it failed.", new object[]
					{
						job,
						jobProcessor
					});
					throw new MigrationProcessorValidationException(jobProcessor.ToString(), job.JobName);
				}
				legacyMigrationJobProcessorResponse = jobProcessor.Process();
				if (job.ReportData != null)
				{
					dataProvider.FlushReport(job.ReportData);
				}
				if (legacyMigrationJobProcessorResponse.Result == MigrationProcessorResult.Completed)
				{
					jobProcessor.OnComplete();
					legacyMigrationJobProcessorResponse.NextStatus = new MigrationJobStatus?(jobProcessor.GetNextStageStatus());
				}
			}
			return legacyMigrationJobProcessorResponse;
		}

		internal static MigrationProcessorResponse RunJobOperation(IMigrationDataProvider dataProvider, MigrationSession session, MigrationJob job, Func<IMigrationDataProvider, MigrationSession, MigrationJob, LegacyMigrationJobProcessorResponse> jobOperation)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(jobOperation, "jobOperation");
			MigrationUtil.ThrowOnNullArgument(job, "job");
			if (job.PoisonCount >= ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationPoisonedCountThreshold"))
			{
				LegacyMigrationProcessor.HandlePermanentException(dataProvider, job, new MigrationPoisonCountThresholdExceededException());
			}
			try
			{
				job.UpdatePoisonCount(dataProvider, job.PoisonCount + 1);
			}
			catch (SaveConflictException ex)
			{
				SaveConflictException ex9;
				MigrationApplication.NotifyOfTransientException(ex9, "MigrationApplication::RunJobOperation - UpdatePoisonCount: job " + job);
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, new TimeSpan?(TimeSpan.Zero), null);
			}
			catch (LocalizedException ex2)
			{
				if (CommonUtils.IsTransientException(ex2))
				{
					MigrationApplication.NotifyOfTransientException(ex2, "MigrationApplication::RunJobOperation - UpdatePoisonCount: job " + job);
					throw;
				}
				MigrationApplication.NotifyOfCriticalError(ex2, "MigrationApplication::RunJobOperation - UpdatePoisonCount => job = " + job);
				throw new MigrationTransientException(ex2.LocalizedString, ex2);
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Waiting, new TimeSpan?(TimeSpan.Zero));
			LocalizedException ex3 = null;
			try
			{
				if (job.AdminCulture.IsNeutralCulture)
				{
					throw new UnsupportedAdminCultureException(job.AdminCulture.ToString());
				}
				Thread.CurrentThread.CurrentCulture = job.AdminCulture;
				Thread.CurrentThread.CurrentUICulture = job.AdminCulture;
				MigrationLogContext.Current.Job = job;
				ExDateTime utcNow = ExDateTime.UtcNow;
				TimeSpan? timeSpan = null;
				if (job.StatusData.InternalErrorTime != null)
				{
					timeSpan = new TimeSpan?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorTransientErrorRunDelay") - (utcNow - job.StatusData.InternalErrorTime.Value));
					if (timeSpan.Value <= TimeSpan.Zero)
					{
						timeSpan = null;
					}
					else
					{
						MigrationLogger.Log(MigrationEventType.Information, "ProcessJob: Skipping job {0} because an internal error recently occurred {1}. {2} remaining", new object[]
						{
							job,
							job.StatusData,
							timeSpan
						});
					}
				}
				if (timeSpan != null)
				{
					legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Waiting, timeSpan);
				}
				else
				{
					legacyMigrationJobProcessorResponse = jobOperation(dataProvider, session, job);
				}
				if (legacyMigrationJobProcessorResponse.Result != MigrationProcessorResult.Deleted)
				{
					job.SetLastScheduled(dataProvider, ExDateTime.UtcNow);
				}
				if (legacyMigrationJobProcessorResponse.Result == MigrationProcessorResult.Waiting)
				{
					job.SetNextProcessTime(dataProvider, utcNow + (legacyMigrationJobProcessorResponse.DelayTime ?? TimeSpan.Zero));
				}
				if (legacyMigrationJobProcessorResponse.Result == MigrationProcessorResult.Completed)
				{
					job.SetJobStatus(dataProvider, legacyMigrationJobProcessorResponse.NextStatus.Value);
					legacyMigrationJobProcessorResponse = ((legacyMigrationJobProcessorResponse.NextStatus == MigrationJobStatus.Removed) ? LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Deleted, null) : LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Working, null));
				}
				if (job.StatusData.InternalErrorTime != null && job.StatusData.InternalErrorTime < utcNow)
				{
					MigrationLogger.Log(MigrationEventType.Information, "ProcessJob: clearing job transient error count for successful run job:{0}, status:{1}", new object[]
					{
						job,
						job.StatusData
					});
					job.ClearTransientErrorCount(dataProvider);
				}
			}
			catch (ConnectionFailedPermanentException ex4)
			{
				LegacyMigrationProcessor.HandleTransientException(dataProvider, job, ex4);
				ex3 = ex4;
			}
			catch (InvalidDataException innerException)
			{
				LegacyMigrationProcessor.HandlePermanentException(dataProvider, job, new MigrationDataCorruptionException("error running job", innerException));
			}
			catch (ObjectNotFoundException ex5)
			{
				LegacyMigrationProcessor.HandleTransientException(dataProvider, job, ex5);
				ex3 = ex5;
			}
			catch (StoragePermanentException ex6)
			{
				LegacyMigrationProcessor.HandleTransientException(dataProvider, job, ex6);
				ex3 = ex6;
			}
			catch (TransientException ex7)
			{
				LegacyMigrationProcessor.HandleTransientException(dataProvider, job, ex7);
				ex3 = ex7;
			}
			catch (LocalizedException ex8)
			{
				if (CommonUtils.IsTransientException(ex8))
				{
					LegacyMigrationProcessor.HandleTransientException(dataProvider, job, ex8);
					ex3 = ex8;
				}
				else
				{
					LegacyMigrationProcessor.HandlePermanentException(dataProvider, job, ex8);
				}
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
				MigrationLogContext.Current.Job = null;
			}
			if (legacyMigrationJobProcessorResponse.Result != MigrationProcessorResult.Deleted)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					job.UpdatePoisonCount(dataProvider, 0);
				}, delegate(Exception ex)
				{
					MigrationApplication.NotifyOfIgnoredException(ex, "Error clearing poison count for job: " + job);
				});
			}
			if (ex3 != null)
			{
				throw new MigrationTransientException(ex3.LocalizedString, ex3);
			}
			return legacyMigrationJobProcessorResponse;
		}

		private static void HandlePermanentException(IMigrationDataProvider dataProvider, MigrationJob job, LocalizedException ex)
		{
			if (ex is MigrationDataCorruptionException)
			{
				MigrationApplication.NotifyOfCorruptJob(ex, "MigrationApplication::ProcessJob => job = " + job);
				MigrationHelper.SendFriendlyWatson(ex, true, job.ToString());
			}
			else
			{
				MigrationApplication.NotifyOfCriticalError(ex, "MigrationApplication::ProcessJob => job = " + job);
			}
			job.SetFailedStatus(dataProvider, ex);
			throw new MigrationTransientException(ex.LocalizedString, ex);
		}

		private static void HandleTransientException(IMigrationDataProvider dataProvider, MigrationJob job, LocalizedException ex)
		{
			CommonUtils.CatchKnownExceptions(delegate
			{
				job.SetTransientError(dataProvider, ex);
			}, delegate(Exception failure)
			{
				MigrationApplication.NotifyOfIgnoredException(failure, "Error setting Transient Error: ");
			});
			if (MigrationApplication.HasTransientErrorReachedThreshold<MigrationJobStatus>(job.StatusData))
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Transient error reached threshold for job " + job, new object[0]);
				LegacyMigrationProcessor.HandlePermanentException(dataProvider, job, new TooManyTransientFailuresException(job.JobName, ex));
				return;
			}
			MigrationApplication.NotifyOfTransientException(ex, "MigrationApplication::ProcessJob => job " + job);
		}
	}
}

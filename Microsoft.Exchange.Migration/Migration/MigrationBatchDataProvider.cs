using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationBatchDataProvider : XsoMailboxDataProviderBase
	{
		public MigrationBatchDataProvider(MigrationDataProvider dataProvider, MigrationBatchStatus? status) : base(dataProvider.MailboxSession)
		{
			this.dataProvider = dataProvider;
			this.status = status;
			this.MigrationSession = MigrationSession.Get(this.dataProvider);
			this.diagnosticEnabled = false;
			this.JobCache = new MigrationJobObjectCache(dataProvider);
		}

		public IMigrationDataProvider MailboxProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		public MigrationSession MigrationSession { get; private set; }

		public MigrationJob MigrationJob { get; set; }

		public bool IncludeReport { get; set; }

		public MigrationJobObjectCache JobCache { get; private set; }

		public static MigrationBatchDataProvider CreateDataProvider(string action, IRecipientSession recipientSession, MigrationBatchStatus? status, ADUser partitionMailbox)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(action, "action");
			MigrationUtil.ThrowOnNullArgument(recipientSession, "recipientSession");
			MigrationBatchDataProvider result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MigrationDataProvider disposable = MigrationDataProvider.CreateProviderForMigrationMailbox(action, recipientSession, partitionMailbox);
				disposeGuard.Add<MigrationDataProvider>(disposable);
				MigrationBatchDataProvider migrationBatchDataProvider = new MigrationBatchDataProvider(disposable, status);
				disposeGuard.Success();
				result = migrationBatchDataProvider;
			}
			return result;
		}

		public static bool IsKnownException(Exception exception)
		{
			return exception is MigrationBatchNotFoundException || exception is CsvValidationException || exception is StorageTransientException || exception is StoragePermanentException || exception is MigrationTransientException || exception is MigrationPermanentException || exception is MigrationDataCorruptionException || exception is DiagnosticArgumentException;
		}

		public void EnableDiagnostics(string argument)
		{
			this.diagnosticEnabled = true;
			this.diagnosticArgument = new MigrationDiagnosticArgument(argument);
		}

		public MigrationJob CreateBatch(MigrationBatch migrationBatch)
		{
			migrationBatch.OriginalCreationTime = DateTime.UtcNow;
			migrationBatch.OriginalStatisticsEnabled = true;
			MigrationJob migrationJob = this.MigrationSession.CreateJob(this.dataProvider, migrationBatch);
			migrationBatch.Identity = new MigrationBatchId(migrationJob.JobName, migrationJob.JobId);
			return migrationJob;
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (this.MigrationJob != null)
			{
				return this.CreateBatchFromJob<T>(this.MigrationJob);
			}
			return this.FindBatches<T>(rootId);
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			switch (instance.ObjectState)
			{
			case ObjectState.New:
			case ObjectState.Unchanged:
			case ObjectState.Changed:
				break;
			case ObjectState.Deleted:
				base.Delete(instance);
				break;
			default:
				return;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.dataProvider != null)
					{
						this.dataProvider.Dispose();
					}
					this.dataProvider = null;
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationBatchDataProvider>(this);
		}

		private MigrationBatch GetMigrationBatch(MigrationJob job)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			if (this.IncludeReport)
			{
				this.dataProvider.LoadReport(job.ReportData);
			}
			MigrationBatch migrationBatch = MigrationJob.GetMigrationBatch(this.dataProvider, this.MigrationSession, job);
			if (this.diagnosticEnabled)
			{
				XElement diagnosticInfo = job.GetDiagnosticInfo(this.dataProvider, this.diagnosticArgument);
				if (diagnosticInfo != null)
				{
					migrationBatch.DiagnosticInfo = diagnosticInfo.ToString();
				}
			}
			return migrationBatch;
		}

		private IEnumerable<T> CreateBatchFromJob<T>(MigrationJob migrationJob)
		{
			if (typeof(T) != typeof(MigrationBatch))
			{
				throw new ArgumentException("unknown type: " + typeof(T));
			}
			if (migrationJob != null && migrationJob.MigrationType != MigrationType.BulkProvisioning)
			{
				MigrationBatch migrationBatch = this.GetMigrationBatch(migrationJob);
				yield return (T)((object)migrationBatch);
			}
			yield break;
		}

		private IEnumerable<T> FindBatches<T>(ObjectId rootId)
		{
			if (typeof(T) != typeof(MigrationBatch))
			{
				throw new ArgumentException("unknown type: " + typeof(T));
			}
			MigrationBatchId batchId = (MigrationBatchId)rootId;
			foreach (MigrationJob job in this.MigrationSession.GetOrderedJobs(this.dataProvider))
			{
				if (this.status == null && batchId == null && job.Status == MigrationJobStatus.Removed)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "Skipping batch {0} because it's removed", new object[]
					{
						job
					});
				}
				else if (job.MigrationType == MigrationType.BulkProvisioning)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "Skipping batch {0} because it's bulk provisioning", new object[]
					{
						job
					});
				}
				else
				{
					if (batchId != null && !batchId.Equals(MigrationBatchId.Any))
					{
						if (!batchId.Equals(new MigrationBatchId(job.JobName, job.JobId)))
						{
							continue;
						}
					}
					MigrationBatch migrationBatch;
					try
					{
						migrationBatch = this.GetMigrationBatch(job);
					}
					catch (ObjectNotFoundException innerException)
					{
						if (batchId != null && !MigrationBatchId.Any.Equals(batchId))
						{
							throw new MigrationBatchNotFoundException(job.JobName, innerException);
						}
						continue;
					}
					if (this.status != null && migrationBatch.Status != this.status.Value)
					{
						MigrationLogger.Log(MigrationEventType.Verbose, "Skipping batch {0} because status doesn't match {1}", new object[]
						{
							migrationBatch,
							this.status.Value
						});
					}
					else
					{
						yield return (T)((object)migrationBatch);
					}
				}
			}
			yield break;
		}

		private MigrationDataProvider dataProvider;

		private MigrationBatchStatus? status;

		private bool diagnosticEnabled;

		private MigrationDiagnosticArgument diagnosticArgument;
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderSyncJobQueue : JobQueue, IDisposeTrackable, IDisposable
	{
		public PublicFolderSyncJobQueue() : base(QueueType.PublicFolder, new Configuration(10, 10, TimeSpan.FromMilliseconds(10.0)))
		{
		}

		public override void OnJobCompletion(Job job)
		{
			PublicFolderSyncJob publicFolderSyncJob = (PublicFolderSyncJob)job;
			lock (this.lockObject)
			{
				this.queuedPublicFolderSyncJobs.Remove(publicFolderSyncJob.ContentMailboxGuid);
				this.completedPublicFolderSyncJobs.AddAbsolute(publicFolderSyncJob.ContentMailboxGuid, publicFolderSyncJob, PublicFolderSyncJobQueue.completedSyncJobExpirationTime, null);
			}
			base.OnJobCompletion(job);
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderSyncJobQueue>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected override bool TryCreateJob(byte[] data, out Job job, out EnqueueResult result)
		{
			job = null;
			if (data == null)
			{
				result = new EnqueueResult(EnqueueResultType.InvalidData, "Null arguments");
				return false;
			}
			PublicFolderSyncJobRpcInParameters publicFolderSyncJobRpcInParameters = null;
			try
			{
				publicFolderSyncJobRpcInParameters = new PublicFolderSyncJobRpcInParameters(data);
				if (publicFolderSyncJobRpcInParameters.ContentMailboxGuid == Guid.Empty)
				{
					result = new EnqueueResult(EnqueueResultType.InvalidData, "Empty ContentMailboxGuid");
				}
			}
			catch (SerializationException ex)
			{
				result = new EnqueueResult(EnqueueResultType.InvalidData, ex.Message);
				return false;
			}
			if (publicFolderSyncJobRpcInParameters.SyncAction == PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.SyncFolder)
			{
				LocalizedException lastError = null;
				try
				{
					using (PublicFolderSynchronizerContext publicFolderSynchronizerContext = new PublicFolderSynchronizerContext(publicFolderSyncJobRpcInParameters.OrganizationId, publicFolderSyncJobRpcInParameters.ContentMailboxGuid, true, false, Guid.NewGuid()))
					{
						PublicFolderHierarchySyncExecutor publicFolderHierarchySyncExecutor = PublicFolderHierarchySyncExecutor.CreateForSingleFolderSync(publicFolderSynchronizerContext);
						publicFolderHierarchySyncExecutor.SyncSingleFolder(publicFolderSyncJobRpcInParameters.FolderId);
					}
				}
				catch (PublicFolderSyncPermanentException ex2)
				{
					lastError = ex2;
				}
				catch (PublicFolderSyncTransientException ex3)
				{
					lastError = ex3;
				}
				result = new PublicFolderSyncJobEnqueueResult(EnqueueResultType.Successful, new PublicFolderSyncJobState(PublicFolderSyncJobState.Status.None, lastError));
				return false;
			}
			if (publicFolderSyncJobRpcInParameters.SyncAction == PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.StartSyncHierarchy || publicFolderSyncJobRpcInParameters.SyncAction == PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.StartSyncHierarchyWithFolderReconciliation)
			{
				lock (this.lockObject)
				{
					if (this.completedPublicFolderSyncJobs.Contains(publicFolderSyncJobRpcInParameters.ContentMailboxGuid))
					{
						this.completedPublicFolderSyncJobs.Remove(publicFolderSyncJobRpcInParameters.ContentMailboxGuid);
					}
					if (this.queuedPublicFolderSyncJobs.ContainsKey(publicFolderSyncJobRpcInParameters.ContentMailboxGuid))
					{
						result = new PublicFolderSyncJobEnqueueResult(EnqueueResultType.Successful, new PublicFolderSyncJobState(PublicFolderSyncJobState.Status.Queued, null));
						return false;
					}
					result = new PublicFolderSyncJobEnqueueResult(EnqueueResultType.Successful, new PublicFolderSyncJobState(PublicFolderSyncJobState.Status.Queued, null));
					job = new PublicFolderSyncJob(this, publicFolderSyncJobRpcInParameters.OrganizationId, publicFolderSyncJobRpcInParameters.ContentMailboxGuid, publicFolderSyncJobRpcInParameters.SyncAction == PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.StartSyncHierarchyWithFolderReconciliation);
					this.queuedPublicFolderSyncJobs[publicFolderSyncJobRpcInParameters.ContentMailboxGuid] = (PublicFolderSyncJob)job;
					return true;
				}
			}
			if (publicFolderSyncJobRpcInParameters.SyncAction == PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.QueryStatusSyncHierarchy)
			{
				lock (this.lockObject)
				{
					PublicFolderSyncJob publicFolderSyncJob = null;
					if (this.queuedPublicFolderSyncJobs.TryGetValue(publicFolderSyncJobRpcInParameters.ContentMailboxGuid, out publicFolderSyncJob))
					{
						result = new PublicFolderSyncJobEnqueueResult(EnqueueResultType.Successful, new PublicFolderSyncJobState(PublicFolderSyncJobState.Status.Queued, null));
					}
					else if (this.completedPublicFolderSyncJobs.TryGetValue(publicFolderSyncJobRpcInParameters.ContentMailboxGuid, out publicFolderSyncJob))
					{
						result = new PublicFolderSyncJobEnqueueResult(EnqueueResultType.Successful, new PublicFolderSyncJobState(PublicFolderSyncJobState.Status.Completed, (LocalizedException)publicFolderSyncJob.LastError));
					}
					else
					{
						result = new PublicFolderSyncJobEnqueueResult(EnqueueResultType.Successful, new PublicFolderSyncJobState(PublicFolderSyncJobState.Status.None, null));
					}
					return false;
				}
			}
			throw new InvalidOperationException(string.Format("Should not have reached here. SyncAction: {0}", publicFolderSyncJobRpcInParameters.SyncAction));
		}

		private static TimeSpan completedSyncJobExpirationTime = TimeSpan.FromMinutes(30.0);

		private TimeoutCache<Guid, PublicFolderSyncJob> completedPublicFolderSyncJobs = new TimeoutCache<Guid, PublicFolderSyncJob>(1, 1000, false);

		private Dictionary<Guid, PublicFolderSyncJob> queuedPublicFolderSyncJobs = new Dictionary<Guid, PublicFolderSyncJob>();

		private object lockObject = new object();

		private DisposeTracker disposeTracker;
	}
}

using System;

namespace Microsoft.Exchange.Transport
{
	internal class ResourceMonitorFactory
	{
		internal ResourceMonitorFactory(ResourceManagerConfiguration resourceManagerConfig)
		{
			if (resourceManagerConfig == null)
			{
				throw new ArgumentNullException("resourceManagerConfig");
			}
			this.resourceManagerConfig = resourceManagerConfig;
		}

		public ResourceMonitor MailDatabaseMonitor
		{
			get
			{
				if (this.mailDatabaseMonitor == null)
				{
					this.mailDatabaseMonitor = new DatabaseMonitor(Components.MessagingDatabase.Database.DataSource, this.resourceManagerConfig.DatabaseDiskSpaceResourceMonitor);
				}
				return this.mailDatabaseMonitor;
			}
			protected set
			{
				this.mailDatabaseMonitor = value;
			}
		}

		public ResourceMonitor MailDatabaseLoggingFolderMonitor
		{
			get
			{
				if (this.mailDatabaseLoggingFolderMonitor == null)
				{
					this.mailDatabaseLoggingFolderMonitor = new DatabaseLoggingFolderMonitor(Components.MessagingDatabase.Database.DataSource, this.resourceManagerConfig.DatabaseLoggingDiskSpaceResourceMonitor);
				}
				return this.mailDatabaseLoggingFolderMonitor;
			}
			protected set
			{
				this.mailDatabaseLoggingFolderMonitor = value;
			}
		}

		public ResourceMonitor TempDriveMonitor
		{
			get
			{
				if (this.tempDriveMonitor == null)
				{
					this.tempDriveMonitor = new DiskSpaceMonitor(Strings.TemporaryStorageResource(Components.TransportAppConfig.WorkerProcess.TemporaryStoragePath), Components.TransportAppConfig.WorkerProcess.TemporaryStoragePath, new ResourceManagerConfiguration.ResourceMonitorConfiguration(100, 100, 100), Components.TransportAppConfig.ResourceManager.TempDiskSpaceRequired.ToBytes());
				}
				return this.tempDriveMonitor;
			}
			protected set
			{
				this.tempDriveMonitor = value;
			}
		}

		public ResourceMonitor VersionBucketResourceMonitor
		{
			get
			{
				if (this.versionBucketResourceMonitor == null)
				{
					this.versionBucketResourceMonitor = new ResourceMonitorStabilizer(new VersionBucketsMonitor(Components.MessagingDatabase.Database.DataSource, this.resourceManagerConfig.VersionBucketsResourceMonitor), this.resourceManagerConfig.VersionBucketsResourceMonitor);
				}
				return this.versionBucketResourceMonitor;
			}
			protected set
			{
				this.versionBucketResourceMonitor = value;
			}
		}

		public ResourceMonitor MemoryPrivateBytesMonitor
		{
			get
			{
				if (this.memoryPrivateBytesMonitor == null)
				{
					this.memoryPrivateBytesMonitor = new ResourceMonitorStabilizer(new MemoryPrivateBytesMonitor(this.resourceManagerConfig.PrivateBytesResourceMonitor), this.resourceManagerConfig.PrivateBytesResourceMonitor);
				}
				return this.memoryPrivateBytesMonitor;
			}
			protected set
			{
				this.memoryPrivateBytesMonitor = value;
			}
		}

		public ResourceMonitor MemoryTotalBytesMonitor
		{
			get
			{
				if (this.memoryTotalBytesMonitor == null)
				{
					this.memoryTotalBytesMonitor = new MemoryTotalBytesMonitor(this.resourceManagerConfig.MemoryTotalBytesResourceMonitor);
				}
				return this.memoryTotalBytesMonitor;
			}
			protected set
			{
				this.memoryTotalBytesMonitor = value;
			}
		}

		public ResourceMonitor SubmissionQueueMonitor
		{
			get
			{
				if (this.submissionQueueMonitor == null)
				{
					this.submissionQueueMonitor = new ResourceMonitorStabilizer(new SubmissionQueueMonitor(this.resourceManagerConfig.SubmissionQueueResourceMonitor), this.resourceManagerConfig.SubmissionQueueResourceMonitor);
				}
				return this.submissionQueueMonitor;
			}
			protected set
			{
				this.submissionQueueMonitor = value;
			}
		}

		private ResourceManagerConfiguration resourceManagerConfig;

		private ResourceMonitor mailDatabaseMonitor;

		private ResourceMonitor mailDatabaseLoggingFolderMonitor;

		private ResourceMonitor tempDriveMonitor;

		private ResourceMonitor versionBucketResourceMonitor;

		private ResourceMonitor memoryPrivateBytesMonitor;

		private ResourceMonitor memoryTotalBytesMonitor;

		private ResourceMonitor submissionQueueMonitor;
	}
}

using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PhotoGarbageCollectionStatsTracker : IDisposable
	{
		public PhotoGarbageCollectionStatsTracker(string marker, IPerformanceDataLogger logger)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("marker", marker);
			this.marker = marker;
			this.logger = (logger ?? NullPerformanceDataLogger.Instance);
			this.fileCount = 0U;
			this.deletedFileCount = 0U;
			this.totalFileSizeInMB = 0.0;
			this.totalDeletedFileSizeInMB = 0.0;
		}

		public void Account(FileInfo file)
		{
			if (file == null)
			{
				return;
			}
			this.fileCount += 1U;
			this.totalFileSizeInMB += (double)file.Length / 1048576.0;
		}

		public void AccountDeleted(FileInfo file)
		{
			if (file == null)
			{
				return;
			}
			this.deletedFileCount += 1U;
			this.totalDeletedFileSizeInMB += (double)file.Length / 1048576.0;
		}

		public void Dispose()
		{
			this.Stop();
		}

		public void Stop()
		{
			this.logger.Log(this.marker, "FileCount", this.fileCount);
			this.logger.Log(this.marker, "TotalFileSizeInMB", (uint)Math.Round(this.totalFileSizeInMB));
			this.logger.Log(this.marker, "DeletedFileCount", this.deletedFileCount);
			this.logger.Log(this.marker, "TotalDeletedFileSizeInMB", (uint)Math.Round(this.totalDeletedFileSizeInMB));
		}

		private const string FileCount = "FileCount";

		private const string DeletedFileCount = "DeletedFileCount";

		private const string TotalFileSizeInMB = "TotalFileSizeInMB";

		private const string TotalDeletedFileSizeInMB = "TotalDeletedFileSizeInMB";

		private const double BytesInMegabyte = 1048576.0;

		private readonly string marker;

		private readonly IPerformanceDataLogger logger;

		private uint fileCount;

		private uint deletedFileCount;

		private double totalFileSizeInMB;

		private double totalDeletedFileSizeInMB;
	}
}

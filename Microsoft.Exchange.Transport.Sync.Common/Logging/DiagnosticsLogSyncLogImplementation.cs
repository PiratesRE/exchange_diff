using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DiagnosticsLogSyncLogImplementation : DisposeTrackableBase, ISyncLogImplementation
	{
		public DiagnosticsLogSyncLogImplementation(LogSchema schema, SyncLogConfiguration syncLogConfiguration)
		{
			this.log = new Log(syncLogConfiguration.LogFilePrefix, new LogHeaderFormatter(schema), syncLogConfiguration.LogComponent);
		}

		public void Configure(bool enabled, string path, long ageQuota, long directorySizeQuota, long perFileSizeQuota)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNullOrEmpty("path", path);
			SyncUtilities.ThrowIfArgumentLessThanZero("ageQuota", ageQuota);
			SyncUtilities.ThrowIfArgumentLessThanZero("directorySizeQuota", directorySizeQuota);
			SyncUtilities.ThrowIfArgumentLessThanZero("perFileSizeQuota", perFileSizeQuota);
			SyncUtilities.ThrowIfArg1LessThenArg2("directorySizeQuota", directorySizeQuota, "perFileSizeQuota", perFileSizeQuota);
			this.log.Configure(path, TimeSpan.FromHours((double)ageQuota), directorySizeQuota * 1024L, perFileSizeQuota * 1024L);
			this.enabled = enabled;
		}

		public bool IsEnabled()
		{
			base.CheckDisposed();
			return this.enabled;
		}

		public void Append(LogRowFormatter row, int timestampField)
		{
			base.CheckDisposed();
			this.log.Append(row, timestampField);
		}

		public void Close()
		{
			base.CheckDisposed();
			this.Dispose();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.log != null)
			{
				this.log.Close();
				this.log = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DiagnosticsLogSyncLogImplementation>(this);
		}

		private bool enabled;

		private Log log;
	}
}

using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EdgeSync.Logging;
using Microsoft.Exchange.MessageSecurity.EdgeSync;

namespace Microsoft.Exchange.EdgeSync.Datacenter
{
	internal class FileLeaseManager
	{
		public FileLeaseManager(string leaseFileName, string primaryLeaseLocation, string backupLeaseLocation, EnhancedTimeSpan syncInterval, EdgeSyncLogSession logSession, Trace tracer)
		{
			this.logSession = logSession;
			this.tracer = tracer;
			this.interSiteLeaseExpiryInterval = 2L * syncInterval;
			this.primaryLeaseFilePath = Path.Combine(primaryLeaseLocation, leaseFileName);
			this.backupLeaseFilePath = Path.Combine(backupLeaseLocation, leaseFileName);
		}

		public static FileLeaseManager.LeaseOperationResult TryRunLeaseOperation(FileLeaseManager.LeaseOperation leaseOperation, FileLeaseManager.LeaseOperationRequest request)
		{
			Exception e = null;
			for (int i = 0; i < 10; i++)
			{
				e = null;
				if (i > 0)
				{
					Thread.Sleep(50);
				}
				try
				{
					return leaseOperation(request);
				}
				catch (FileNotFoundException ex)
				{
					e = ex;
				}
				catch (DirectoryNotFoundException ex2)
				{
					e = ex2;
				}
				catch (IOException ex3)
				{
					e = ex3;
				}
				catch (UnauthorizedAccessException ex4)
				{
					e = ex4;
				}
			}
			return new FileLeaseManager.LeaseOperationResult(e);
		}

		public static FileLeaseManager.LeaseOperationResult GetLeaseOperation(FileLeaseManager.LeaseOperationRequest request)
		{
			FileLeaseManager.LeaseOperationResult result;
			using (FileStream fileStream = new FileStream(request.LeasePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
			{
				byte[] array = new byte[fileStream.Length];
				int count = fileStream.Read(array, 0, array.Length);
				string @string = Encoding.ASCII.GetString(array, 0, count);
				result = new FileLeaseManager.LeaseOperationResult(LeaseToken.Parse(@string));
			}
			return result;
		}

		public LeaseToken GetLease()
		{
			FileLeaseManager.LeaseOperationResult leaseOperationResult = FileLeaseManager.TryRunLeaseOperation(new FileLeaseManager.LeaseOperation(FileLeaseManager.GetLeaseOperation), new FileLeaseManager.LeaseOperationRequest(this.primaryLeaseFilePath));
			if (leaseOperationResult.Succeeded)
			{
				if (this.useBackupLeaseLocation)
				{
					this.logSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, null, "Switch back to primary lease from backup lease");
				}
				this.useBackupLeaseLocation = false;
				return leaseOperationResult.ResultToken;
			}
			this.logSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, leaseOperationResult.Exception, "Failed to open primary lease file. Switch to backup lease file");
			leaseOperationResult = FileLeaseManager.TryRunLeaseOperation(new FileLeaseManager.LeaseOperation(FileLeaseManager.GetLeaseOperation), new FileLeaseManager.LeaseOperationRequest(this.backupLeaseFilePath));
			if (leaseOperationResult.Succeeded)
			{
				this.logSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, null, "Successfully failed over to backup lease file");
				this.useBackupLeaseLocation = true;
				return leaseOperationResult.ResultToken;
			}
			this.logSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, leaseOperationResult.Exception, "Failed to open backup lease file");
			throw new ExDirectoryException(leaseOperationResult.Exception);
		}

		public bool CanTakeOverLease(bool force, LeaseToken lease, DateTime now, HashSet<string> localSiteHubs, out bool siteChanged)
		{
			siteChanged = !localSiteHubs.Contains(lease.Path);
			if (force)
			{
				return true;
			}
			if (!siteChanged)
			{
				return lease.Expiry < now;
			}
			bool flag = lease.Expiry + this.interSiteLeaseExpiryInterval < now;
			this.tracer.TraceDebug((long)this.GetHashCode(), "{0} over out of site lease {1}, Expiry {2}, TimeNow {3}", new object[]
			{
				flag ? "Took" : "Did not take",
				lease.Path,
				lease.Expiry,
				now
			});
			return flag;
		}

		public void SetLease(LeaseToken leaseToken)
		{
			LeaseToken token = new LeaseToken(leaseToken.Path, leaseToken.Expiry, leaseToken.Type, leaseToken.LastSync, leaseToken.Expiry + this.interSiteLeaseExpiryInterval + FileLeaseManager.LeaseExpiryCriticalAlertPadding, leaseToken.Version);
			FileLeaseManager.LeaseOperationResult leaseOperationResult = FileLeaseManager.TryRunLeaseOperation(new FileLeaseManager.LeaseOperation(this.SetLeaseOperation), new FileLeaseManager.LeaseOperationRequest(token));
			if (!leaseOperationResult.Succeeded)
			{
				this.logSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, leaseOperationResult.Exception, "Sync failed because Edgesync failed to update lease file");
				throw new ExDirectoryException(leaseOperationResult.Exception);
			}
		}

		private FileLeaseManager.LeaseOperationResult SetLeaseOperation(FileLeaseManager.LeaseOperationRequest request)
		{
			string path = this.useBackupLeaseLocation ? this.backupLeaseFilePath : this.primaryLeaseFilePath;
			FileLeaseManager.LeaseOperationResult result;
			using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				byte[] bytes = Encoding.ASCII.GetBytes(request.Token.StringForm);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Flush();
				result = new FileLeaseManager.LeaseOperationResult();
			}
			return result;
		}

		private const int LeaseRetryLimit = 10;

		private static readonly TimeSpan LeaseExpiryCriticalAlertPadding = TimeSpan.FromMinutes(5.0);

		private readonly string primaryLeaseFilePath;

		private readonly string backupLeaseFilePath;

		private readonly EnhancedTimeSpan interSiteLeaseExpiryInterval;

		private bool useBackupLeaseLocation;

		private EdgeSyncLogSession logSession;

		private Trace tracer;

		public delegate FileLeaseManager.LeaseOperationResult LeaseOperation(FileLeaseManager.LeaseOperationRequest request);

		public class LeaseOperationRequest
		{
			public LeaseOperationRequest(string leasePath)
			{
				this.leasePath = leasePath;
			}

			public LeaseOperationRequest(LeaseToken token)
			{
				this.token = token;
			}

			public string LeasePath
			{
				get
				{
					return this.leasePath;
				}
			}

			public LeaseToken Token
			{
				get
				{
					return this.token;
				}
			}

			private string leasePath;

			private LeaseToken token;
		}

		public class LeaseOperationResult
		{
			public LeaseOperationResult(LeaseToken resultToken)
			{
				this.resultToken = resultToken;
			}

			public LeaseOperationResult()
			{
			}

			public LeaseOperationResult(Exception e)
			{
				this.exception = e;
			}

			public bool Succeeded
			{
				get
				{
					return this.exception == null;
				}
			}

			public Exception Exception
			{
				get
				{
					return this.exception;
				}
			}

			public LeaseToken ResultToken
			{
				get
				{
					return this.resultToken;
				}
			}

			private LeaseToken resultToken;

			private Exception exception;
		}
	}
}

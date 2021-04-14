using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncLog : DisposeTrackableBase
	{
		public SyncLog(SyncLogConfiguration syncLogConfiguration)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogConfiguration", syncLogConfiguration);
			this.schema = syncLogConfiguration.CreateLogSchema(SyncLog.Fields);
			this.syncLogImplementation = new DiagnosticsLogSyncLogImplementation(this.schema, syncLogConfiguration);
			this.ConfigureLog(syncLogConfiguration.Enabled, syncLogConfiguration.LogFilePath, syncLogConfiguration.AgeQuotaInHours, syncLogConfiguration.DirectorySizeQuota, syncLogConfiguration.PerFileSizeQuota, syncLogConfiguration.SyncLoggingLevel);
			this.blackBox = new SyncLogBlackBox();
			ExTraceGlobals.SyncLogTracer.TraceDebug((long)this.GetHashCode(), "sync Log Created and Configured");
		}

		private SyncLog()
		{
			SyncLogConfiguration syncLogConfiguration = new SyncLogConfiguration(SyncLoggingLevel.Debugging);
			this.schema = syncLogConfiguration.CreateLogSchema(SyncLog.Fields);
			this.syncLogImplementation = new InMemorySyncLogImplementation();
			this.blackBox = new SyncLogBlackBox();
			this.loggingLevel = SyncLoggingLevel.Debugging;
			ExTraceGlobals.SyncLogTracer.TraceDebug((long)this.GetHashCode(), "in memory sync Log Created");
		}

		internal static SyncLog InMemorySyncLog
		{
			get
			{
				return SyncLog.inMemorySyncLog;
			}
		}

		internal WatsonReporter WatsonReporter
		{
			get
			{
				base.CheckDisposed();
				return this.watsonReporter;
			}
		}

		internal SyncLoggingLevel LoggingLevel
		{
			get
			{
				base.CheckDisposed();
				return this.loggingLevel;
			}
		}

		public void ConfigureLog(bool enabled, string path, long ageQuota, long directorySizeQuota, long perFileSizeQuota, SyncLoggingLevel loggingLevel)
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				this.syncLogImplementation.Configure(enabled, path, ageQuota, directorySizeQuota, perFileSizeQuota);
				this.loggingLevel = loggingLevel;
			}
			ExTraceGlobals.SyncLogTracer.TraceDebug((long)this.GetHashCode(), "sync Logs Configured");
		}

		public SyncLogSession OpenSession()
		{
			base.CheckDisposed();
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[3] = string.Empty;
			logRowFormatter[5] = string.Empty;
			logRowFormatter[6] = string.Empty;
			return new SyncLogSession(this, logRowFormatter);
		}

		public GlobalSyncLogSession OpenGlobalSession()
		{
			base.CheckDisposed();
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[3] = string.Empty;
			logRowFormatter[5] = string.Empty;
			logRowFormatter[6] = string.Empty;
			return new GlobalSyncLogSession(this, logRowFormatter);
		}

		public SyncLogSession OpenSession(AggregationSubscription subscription)
		{
			base.CheckDisposed();
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[3] = ((subscription.UserExchangeMailboxSmtpAddress != null) ? subscription.UserExchangeMailboxSmtpAddress : subscription.UserLegacyDN);
			logRowFormatter[4] = subscription.SubscriptionGuid.ToString();
			logRowFormatter[5] = subscription.SubscriptionType.ToString();
			logRowFormatter[6] = string.Empty;
			return new SyncLogSession(this, logRowFormatter);
		}

		public SyncLogSession OpenSession(Guid mailboxGuid, Guid subscriptionId)
		{
			base.CheckDisposed();
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[3] = mailboxGuid.ToString();
			logRowFormatter[4] = subscriptionId.ToString();
			logRowFormatter[5] = string.Empty;
			logRowFormatter[6] = string.Empty;
			return new SyncLogSession(this, logRowFormatter);
		}

		public SyncLogSession OpenSession(Guid userMailboxGuid, AggregationSubscriptionType subscriptionType, Guid subscriptionId)
		{
			base.CheckDisposed();
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[3] = userMailboxGuid.ToString();
			logRowFormatter[4] = subscriptionId.ToString();
			logRowFormatter[5] = subscriptionType.ToString();
			logRowFormatter[6] = string.Empty;
			return new SyncLogSession(this, logRowFormatter);
		}

		public void Close()
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				if (this.syncLogImplementation != null)
				{
					this.syncLogImplementation.Close();
					this.syncLogImplementation = null;
				}
			}
			ExTraceGlobals.SyncLogTracer.TraceDebug((long)this.GetHashCode(), "sync Logs Closed");
		}

		public string GenerateBlackBoxLog()
		{
			if (!Monitor.TryEnter(this.syncRoot, 30000))
			{
				return "Unable to take this.syncRoot lock in GenerateBlackBoxLog.";
			}
			string result;
			try
			{
				if (base.IsDisposed)
				{
				}
				string text = this.blackBox.ToString();
				result = text;
			}
			finally
			{
				Monitor.Exit(this.syncRoot);
			}
			return result;
		}

		public void Append(LogRowFormatter row)
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				if (this.syncLogImplementation != null && this.syncLogImplementation.IsEnabled())
				{
					this.blackBox.Append(row);
					this.syncLogImplementation.Append(row, 0);
				}
			}
		}

		internal static SyncLog CreateInMemorySyncLog()
		{
			return new SyncLog();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncLog>(this);
		}

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"thread-id",
			"level",
			"user",
			"subscription-guid",
			"subscription-type",
			"logentry-id",
			"context",
			"property-bag"
		};

		private static readonly int processId = Process.GetCurrentProcess().Id;

		private static readonly SyncLog inMemorySyncLog = new SyncLog();

		private readonly WatsonReporter watsonReporter = new WatsonReporter();

		private SyncLogBlackBox blackBox;

		private LogSchema schema;

		private SyncLoggingLevel loggingLevel;

		private ISyncLogImplementation syncLogImplementation;

		private object syncRoot = new object();

		internal enum Field
		{
			Time,
			ThreadId,
			Level,
			User,
			SubscriptionGuid,
			SubscriptionType,
			LogEntryId,
			Context,
			PropertyBag
		}
	}
}

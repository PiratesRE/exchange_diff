using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Compliance.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GccProtocolActivityLoggerSession : DisposeTrackableBase
	{
		public GccProtocolActivityLoggerSession(StoreSession storeSession)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.storeSessionReference = GCHandle.Alloc(storeSession, GCHandleType.Weak);
				this.sessionResourceIdentifier = "Unknown";
				this.sessionClientIPAddress = storeSession.ClientIPAddress;
				this.sessionServerIPAddress = storeSession.ServerIPAddress;
				this.sessionTimestamp = ExDateTime.MinValue;
				this.gccLogger = GccProtocolActivityLoggerSingleton.Get(storeSession.ClientInfoString);
				this.needToLog = false;
				if (this.gccLogger != null)
				{
					this.timeoutTimer = new System.Timers.Timer();
					this.timeoutTimer.Elapsed += this.LoggerTimeoutEventHandler;
					this.timeoutTimer.Interval = GccProtocolActivityLogger.Config.ReportIntervalMilliseconds;
					this.timeoutTimer.AutoReset = false;
				}
				disposeGuard.Success();
			}
		}

		~GccProtocolActivityLoggerSession()
		{
			base.Dispose(false);
		}

		public bool MessagesWereDownloaded { get; set; }

		public void StartSession()
		{
			this.InternalStartSession();
			this.TagCurrentIntervalAsLogworthy();
		}

		public void EndSession()
		{
			if (this.gccLogger == null)
			{
				return;
			}
			if (this.timeoutTimer != null)
			{
				this.timeoutTimer.Stop();
				this.timeoutTimer.Dispose();
				this.timeoutTimer = null;
			}
			this.InternalEndSession();
		}

		public void SetStoreSessionInfo(string gccResourceIdentifier, IPAddress clientIPAddress, IPAddress serverIPAddress)
		{
			this.sessionResourceIdentifier = gccResourceIdentifier;
			this.sessionClientIPAddress = clientIPAddress;
			this.sessionServerIPAddress = serverIPAddress;
			this.TagCurrentIntervalAsLogworthy();
		}

		public void TagCurrentIntervalAsLogworthy()
		{
			this.needToLog = true;
		}

		private void InternalStartSession()
		{
			this.sessionTimestamp = ExDateTime.UtcNow;
			if (this.timeoutTimer != null)
			{
				this.timeoutTimer.Start();
			}
		}

		public void InternalEndSession()
		{
			if (!this.needToLog)
			{
				if (this.timeoutTimer != null)
				{
					this.timeoutTimer.Start();
				}
				return;
			}
			if (Interlocked.Increment(ref this.useCount) > 1)
			{
				if (this.timeoutTimer != null)
				{
					this.timeoutTimer.Start();
				}
				Interlocked.Decrement(ref this.useCount);
				return;
			}
			ExDateTime utcNow = ExDateTime.UtcNow;
			try
			{
				this.gccLogger.Append(this.sessionResourceIdentifier, this.sessionClientIPAddress, this.sessionServerIPAddress, this.sessionTimestamp, utcNow - this.sessionTimestamp, this.MessagesWereDownloaded);
			}
			finally
			{
				Interlocked.Decrement(ref this.useCount);
				this.needToLog = false;
				if (this.timeoutTimer != null)
				{
					this.timeoutTimer.Start();
				}
			}
		}

		private void LoggerTimeoutEventHandler(object sender, ElapsedEventArgs e)
		{
			if (this.storeSessionReference.IsAllocated)
			{
				this.InternalEndSession();
				this.InternalStartSession();
				return;
			}
			this.EndSession();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.timeoutTimer != null)
			{
				this.timeoutTimer.Dispose();
				this.timeoutTimer = null;
			}
			this.storeSessionReference.Free();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<GccProtocolActivityLoggerSession>(this);
		}

		private GCHandle storeSessionReference;

		private string sessionResourceIdentifier;

		private IPAddress sessionClientIPAddress;

		private IPAddress sessionServerIPAddress;

		private ExDateTime sessionTimestamp;

		private GccProtocolActivityLogger gccLogger;

		private bool needToLog;

		private System.Timers.Timer timeoutTimer;

		private int useCount;
	}
}

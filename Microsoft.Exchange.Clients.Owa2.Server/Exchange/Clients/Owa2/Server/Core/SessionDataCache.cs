using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SessionDataCache : DisposeTrackableBase
	{
		internal MemoryStream OutputStream
		{
			get
			{
				MemoryStream result;
				try
				{
					Monitor.Enter(this.lockObject);
					ExAssert.RetailAssert(this.outputStream != null, "Output stream should be initialized before using.");
					ExAssert.RetailAssert(this.state == SessionDataCacheState.Building || this.state == SessionDataCacheState.Obsolete, "Output stream should not be invoked in any other state than building or obsolete: " + this.state.ToString());
					result = this.outputStream;
				}
				finally
				{
					if (Monitor.IsEntered(this.lockObject))
					{
						Monitor.Exit(this.lockObject);
					}
				}
				return result;
			}
		}

		internal UTF8Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		internal bool StartBuilding()
		{
			bool flag = false;
			bool result;
			try
			{
				Monitor.Enter(this.lockObject);
				if (this.state == SessionDataCacheState.Uninitialized)
				{
					this.signalEvent.Reset();
					this.lastSessionDataBuildStartTime = ExDateTime.Now;
					this.lastSessionDataBuildEndTime = ExDateTime.MinValue;
					this.outputStream = new MemoryStream();
					this.state = SessionDataCacheState.Building;
					flag = true;
				}
				result = flag;
			}
			finally
			{
				if (Monitor.IsEntered(this.lockObject))
				{
					Monitor.Exit(this.lockObject);
				}
				ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), string.Format("[SessionDataCache] Started building session data cache. StartTime = {0}, state = {1}", this.lastSessionDataBuildStartTime.ToString(), this.state));
				OwaSingleCounters.SessionDataCacheBuildsStarted.Increment();
			}
			return result;
		}

		internal void CompleteBuilding()
		{
			try
			{
				Monitor.Enter(this.lockObject);
				ExAssert.RetailAssert(this.state == SessionDataCacheState.Building || this.state == SessionDataCacheState.Obsolete, "Ready building session data in an invalid state: " + this.state);
				if (this.state == SessionDataCacheState.Obsolete)
				{
					this.InternalDispose(true);
				}
				else
				{
					this.lastSessionDataBuildEndTime = ExDateTime.Now;
					this.state = SessionDataCacheState.Ready;
					this.signalEvent.Set();
					Timer staleTimer = null;
					staleTimer = new Timer(delegate(object param0)
					{
						this.Dispose();
						DisposeGuard.DisposeIfPresent(staleTimer);
					}, null, SessionDataCache.FreshnessTime, Timeout.InfiniteTimeSpan);
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.lockObject))
				{
					Monitor.Exit(this.lockObject);
				}
				ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), string.Format("[SessionDataCache] Ready building session data cache. StartTime = {0}, EndTime = {1}, state = {2}", this.lastSessionDataBuildStartTime.ToString(), this.lastSessionDataBuildEndTime.ToString(), this.state));
				OwaSingleCounters.SessionDataCacheBuildsCompleted.Increment();
			}
		}

		internal void UseIfReady(RequestDetailsLogger logger, Stream responseStream, out bool used)
		{
			ExAssert.RetailAssert(responseStream != null, "responseStream is null");
			used = false;
			if (this.state != SessionDataCacheState.Obsolete)
			{
				bool flag = false;
				RequestDetailsLogger.LogEvent(logger, SessionDataMetadata.SessionDataCacheFirstTimeRetriveveBegin);
				this.InternalUseIfReady(responseStream, out used, out flag);
				RequestDetailsLogger.LogEvent(logger, SessionDataMetadata.SessionDataCacheFirstTimeRetriveveEnd);
				if (flag)
				{
					OwaSingleCounters.SessionDataCacheWaitedForPreload.Increment();
					bool flag2 = this.signalEvent.WaitOne(5000);
					if (!flag2)
					{
						OwaSingleCounters.SessionDataCacheWaitTimeout.Increment();
					}
					RequestDetailsLogger.LogEvent(logger, SessionDataMetadata.SessionDataCacheSecondTimeRetriveveBegin);
					this.InternalUseIfReady(responseStream, out used, out flag);
					RequestDetailsLogger.LogEvent(logger, SessionDataMetadata.SessionDataCacheSecondTimeRetriveveEnd);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(logger, SessionDataMetadata.SessionDataCacheWaitTimeOut, !flag2);
				}
				if (used)
				{
					OwaSingleCounters.SessionDataCacheUsed.Increment();
				}
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(logger, SessionDataMetadata.SessionDataCacheUsed, used);
			}
		}

		private void InternalUseIfReady(Stream responseStream, out bool used, out bool needToWait)
		{
			needToWait = false;
			used = false;
			try
			{
				Monitor.Enter(this.lockObject);
				if (this.state == SessionDataCacheState.Obsolete)
				{
					ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), string.Format("[SessionDataCache] Not using session data cache. It has already been consumed or never will be = {0}", this.state));
				}
				else if (this.state == SessionDataCacheState.Uninitialized)
				{
					ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), string.Format("[SessionDataCache] Not using session data cache. Preload has not started yet = {0}", this.state));
					this.state = SessionDataCacheState.Obsolete;
				}
				else if (this.state == SessionDataCacheState.Ready && this.lastSessionDataBuildEndTime >= ExDateTime.Now.Add(-SessionDataCache.FreshnessTime))
				{
					this.outputStream.Seek(0L, SeekOrigin.Begin);
					this.outputStream.CopyTo(responseStream);
					this.outputStream.Dispose();
					this.outputStream = null;
					used = true;
					this.state = SessionDataCacheState.Obsolete;
				}
				else if (this.state == SessionDataCacheState.Building)
				{
					needToWait = true;
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.lockObject))
				{
					Monitor.Exit(this.lockObject);
				}
				ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), string.Format("[SessionDataCache] Trying to use session data cache. Obsolete = {0}, needToWait = {1}, state = {2}", used, needToWait, this.state));
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.lockObject)
				{
					if (this.state != SessionDataCacheState.Building)
					{
						DisposeGuard.DisposeIfPresent(this.outputStream);
					}
					this.state = SessionDataCacheState.Obsolete;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SessionDataCache>(this);
		}

		private const int WaitTime = 5000;

		private static readonly TimeSpan FreshnessTime = TimeSpan.FromSeconds(10.0);

		private readonly object lockObject = new object();

		private readonly ManualResetEvent signalEvent = new ManualResetEvent(false);

		private MemoryStream outputStream;

		private UTF8Encoding encoding = new UTF8Encoding();

		private SessionDataCacheState state;

		private ExDateTime lastSessionDataBuildStartTime = ExDateTime.MinValue;

		private ExDateTime lastSessionDataBuildEndTime = ExDateTime.MinValue;
	}
}

using System;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public abstract class UserContextBase : DisposeTrackableBase
	{
		internal UserContextBase(UserContextKey key)
		{
			this.key = key;
			this.sessionBeginTime = Stopwatch.GetTimestamp();
			this.sessionLastAccessedTime = this.sessionBeginTime;
			this.writerLock = UserContextManager.GetUserContextKeyLock(key.ToString());
		}

		internal long SessionBeginTime
		{
			get
			{
				return this.sessionBeginTime;
			}
		}

		internal long LastAccessedTime
		{
			get
			{
				return this.sessionLastAccessedTime;
			}
		}

		internal long RequestCount
		{
			get
			{
				return this.requestCount;
			}
		}

		internal bool IsUserRequestLockHeld
		{
			get
			{
				return this.userRequestLockHeld;
			}
		}

		internal void UpdateLastAccessedTime()
		{
			this.sessionLastAccessedTime = Stopwatch.GetTimestamp();
			this.requestCount += 1L;
		}

		internal CacheItemRemovedReason AbandonedReason
		{
			get
			{
				return this.abandonedReason;
			}
			set
			{
				this.abandonedReason = value;
			}
		}

		public void Touch()
		{
			HttpRuntime.Cache.Get(this.key.ToString());
		}

		internal UserContextKey Key
		{
			get
			{
				return this.key;
			}
		}

		internal UserContextState State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		internal bool LastLockRequestFailed
		{
			get
			{
				if (HttpContext.Current == null || !HttpContext.Current.Items.Contains("LastLockRequestFailed"))
				{
					return false;
				}
				if (HttpContext.Current.Items["LastLockRequestFailed"] is bool)
				{
					return (bool)HttpContext.Current.Items["LastLockRequestFailed"];
				}
				throw new InvalidOperationException("HttpContext.Current.Items[LastLockRequestFailedKey] value is not a bool.  Other code is re-using this key.");
			}
			set
			{
				if (HttpContext.Current != null)
				{
					HttpContext.Current.Items["LastLockRequestFailed"] = value;
				}
			}
		}

		internal void Lock()
		{
			this.Lock(false);
		}

		internal void Lock(bool requestLock)
		{
			ExTraceGlobals.UserContextCallTracer.TraceDebug<UserContextBase>(0L, "UserContext.Lock, User context instance={0}", this);
			try
			{
				this.writerLock.LockWriterElastic(Globals.UserContextLockTimeout);
			}
			catch (OwaLockTimeoutException)
			{
				this.LastLockRequestFailed = true;
				throw;
			}
			this.userRequestLockHeld = requestLock;
			ExTraceGlobals.UserContextCallTracer.TraceDebug(0L, "Acquired lock succesfully");
			this.LastLockRequestFailed = false;
		}

		public void Unlock()
		{
			ExTraceGlobals.UserContextCallTracer.TraceDebug<UserContextBase>(0L, "UserContext.Unlock, User context instance={0}", this);
			if (!this.writerLock.IsWriterLockHeld)
			{
				throw new ApplicationException("Current thread does not have the writerLock.");
			}
			try
			{
				this.OnBeforeUnlock();
			}
			finally
			{
				if (this.NumberOfLocksHeld <= 1)
				{
					this.userRequestLockHeld = false;
				}
				this.writerLock.ReleaseWriterLock();
			}
		}

		internal void TraceObject()
		{
			ExTraceGlobals.UserContextDataTracer.TraceDebug(0L, "UserContext instance: Key.UserContextId={0}, Key={1}, State={2}, User context instance={3}", new object[]
			{
				this.Key.UserContextId,
				this.Key,
				(uint)this.State,
				this
			});
		}

		internal bool LockedByCurrentThread()
		{
			return this.writerLock.IsWriterLockHeld;
		}

		internal void UnlockForcefully()
		{
			this.Unlock();
			if (this.LockedByCurrentThread())
			{
				this.ForceReleaseLock();
				throw new InvalidOperationException("Had to forcefully unlock user context!");
			}
		}

		protected void ForceReleaseLock()
		{
			this.writerLock.ReleaseLock();
			this.userRequestLockHeld = false;
		}

		protected int NumberOfLocksHeld
		{
			get
			{
				return this.writerLock.NumberOfWriterLocksHeld;
			}
		}

		protected abstract void OnBeforeUnlock();

		protected override void InternalDispose(bool isDisposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UserContextBase>(this);
		}

		protected const int LockTimeout = 60000;

		private const string LastLockRequestFailedKey = "LastLockRequestFailed";

		private long requestCount;

		private long sessionBeginTime;

		private long sessionLastAccessedTime;

		private UserContextState state;

		private UserContextKey key;

		private OwaRWLockWrapper writerLock;

		private CacheItemRemovedReason abandonedReason;

		private volatile bool userRequestLockHeld;
	}
}

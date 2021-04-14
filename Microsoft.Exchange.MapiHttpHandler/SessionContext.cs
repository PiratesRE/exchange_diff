using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MapiHttpHandler;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SessionContext
	{
		public SessionContext(UserContext userContext, string mailboxIdentifier, SessionContextIdentifier sessionContextIdentifier, TimeSpan idleTimeout, SessionContext.ISessionEnvironment sessionEnvironment = null)
		{
			this.userContext = userContext;
			this.mailboxIdentifier = mailboxIdentifier;
			this.sessionEnvironment = (sessionEnvironment ?? SessionContext.DefaultSessionEnvironment.Instance);
			this.sessionContextIdentifier = sessionContextIdentifier;
			this.contextHandle = null;
			this.creationTime = this.sessionEnvironment.GetUtcNow();
			this.lastActivity = this.sessionEnvironment.GetTickCount();
			this.lastActivityTime = this.creationTime;
			this.idleTimeout = idleTimeout;
			this.rundownReason = null;
			this.rundownTime = ExDateTime.MinValue;
			this.markForRundownReason = null;
			this.TraceState("Creation", null, this.rundownTime, TimeSpan.Zero, this.idleTimeout, this.idleTimeout, this.activityCount);
		}

		public UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		public string MailboxIdentifier
		{
			get
			{
				return this.mailboxIdentifier;
			}
		}

		public long Id
		{
			get
			{
				return this.sessionContextIdentifier.Id;
			}
		}

		public string Cookie
		{
			get
			{
				return this.sessionContextIdentifier.Cookie;
			}
		}

		public object ContextHandle
		{
			get
			{
				return this.contextHandle;
			}
			set
			{
				this.contextHandle = value;
				if (this.contextHandle == null)
				{
					this.MarkForRundown(SessionRundownReason.ContextHandleCleared);
				}
			}
		}

		public SessionContextIdentifier Identifier
		{
			get
			{
				return this.sessionContextIdentifier;
			}
		}

		public AsyncOperationTracker AsyncOperationTracker
		{
			get
			{
				return this.asyncOperationTracker;
			}
		}

		public TimeSpan ExpirationInfo
		{
			get
			{
				TimeSpan result;
				bool flag;
				ExDateTime exDateTime;
				this.GetState("get_ExpirationInfo", out result, out flag, out exDateTime);
				return result;
			}
		}

		public bool IsRundown
		{
			get
			{
				TimeSpan timeSpan;
				bool result;
				ExDateTime exDateTime;
				this.GetState("get_IsRundown", out timeSpan, out result, out exDateTime);
				return result;
			}
		}

		public ExDateTime Expires
		{
			get
			{
				TimeSpan t;
				bool flag;
				ExDateTime result;
				this.GetState("get_Expires", out t, out flag, out result);
				if (flag)
				{
					return result;
				}
				return this.sessionEnvironment.GetUtcNow() + t;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		public TimeSpan IdleTimeout
		{
			set
			{
				this.TryUpdateState("set_IdleTimeout", true, null, new TimeSpan?(value), false, false);
				TimeSpan t;
				bool flag;
				ExDateTime exDateTime;
				this.GetState("set_IdleTimeout", out t, out flag, out exDateTime);
				this.sessionEnvironment.WakeUpIdleContextMonitor(this.sessionEnvironment.GetUtcNow() + t);
			}
		}

		public SessionRundownReason? RundownReason
		{
			get
			{
				return this.rundownReason;
			}
		}

		public ExDateTime RundownTime
		{
			get
			{
				return this.rundownTime;
			}
		}

		public void MarkForRundown(SessionRundownReason rundownReason)
		{
			if (rundownReason == SessionRundownReason.Expired)
			{
				throw new InvalidOperationException("Cannot explicitly expire a session context.");
			}
			this.TryUpdateState("MarkForRundown", false, new SessionRundownReason?(rundownReason), null, false, false);
		}

		public bool TryAddReference()
		{
			return this.TryUpdateState("TryAddReference", true, null, null, true, false);
		}

		public void ReleaseReference()
		{
			this.TryUpdateState("ReleaseReference", true, null, null, false, true);
		}

		public SessionContextInfo GetSessionContextInfo()
		{
			AsyncOperationInfo[] activeAsyncOperations = null;
			AsyncOperationInfo[] completedAsyncOperations = null;
			AsyncOperationInfo[] failedAsyncOperations = null;
			this.asyncOperationTracker.GetAsyncOperationInfo(out activeAsyncOperations, out completedAsyncOperations, out failedAsyncOperations);
			return new SessionContextInfo(this.creationTime, this.rundownReason, this.rundownTime, this.activityCount, this.lastActivityTime, this.sessionContextIdentifier.Cookie, activeAsyncOperations, completedAsyncOperations, failedAsyncOperations);
		}

		private void GetState(string methodName, out TimeSpan remainingTime, out bool isRundown, out ExDateTime rundownTime)
		{
			remainingTime = TimeSpan.Zero;
			isRundown = false;
			rundownTime = ExDateTime.MinValue;
			int num;
			TimeSpan timeSpan;
			TimeSpan timeSpan2;
			SessionRundownReason? sessionRundownReason;
			ExDateTime exDateTime;
			TimeSpan idleTime;
			lock (this.sessionContextLock)
			{
				num = this.activityCount;
				timeSpan = this.idleTimeout;
				this.ComputeState(out timeSpan2, out sessionRundownReason, out exDateTime, out idleTime);
			}
			this.TraceState(methodName, sessionRundownReason, exDateTime, idleTime, timeSpan, timeSpan2, num);
			remainingTime = timeSpan2;
			isRundown = (sessionRundownReason != null);
			rundownTime = exDateTime;
		}

		private bool TryUpdateState(string methodName, bool setLastActivity, SessionRundownReason? setRundownReason, TimeSpan? newIdleTimeout, bool newActivity, bool releaseActivity)
		{
			bool result = false;
			TimeSpan remainingTime;
			SessionRundownReason? sessionRundownReason;
			ExDateTime exDateTime;
			TimeSpan idleTime;
			int num;
			TimeSpan timeSpan;
			lock (this.sessionContextLock)
			{
				this.ComputeState(out remainingTime, out sessionRundownReason, out exDateTime, out idleTime);
				if (setRundownReason != null)
				{
					this.markForRundownReason = setRundownReason;
					result = true;
				}
				if (sessionRundownReason == null)
				{
					if (setLastActivity)
					{
						this.lastActivity = this.sessionEnvironment.GetTickCount();
						this.lastActivityTime = this.sessionEnvironment.GetUtcNow();
						result = true;
					}
					if (newIdleTimeout != null)
					{
						this.idleTimeout = newIdleTimeout.Value;
						result = true;
					}
					if (newActivity)
					{
						this.activityCount++;
						result = true;
					}
				}
				if (releaseActivity)
				{
					if (this.activityCount == 0)
					{
						throw new InvalidOperationException("Cannot release a reference on a SessionContext object with no references.");
					}
					this.activityCount--;
					result = true;
				}
				num = this.activityCount;
				timeSpan = this.idleTimeout;
				this.ComputeState(out remainingTime, out sessionRundownReason, out exDateTime, out idleTime);
			}
			this.TraceState(methodName, sessionRundownReason, exDateTime, idleTime, timeSpan, remainingTime, num);
			return result;
		}

		private void TraceState(string methodName, SessionRundownReason? rundownReason, ExDateTime rundownTime, TimeSpan idleTime, TimeSpan idleTimeout, TimeSpan remainingTime, int activityCount)
		{
			if (ExTraceGlobals.SessionContextTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (rundownReason != null)
				{
					ExTraceGlobals.SessionContextTracer.TraceInformation(53980, 0L, "SessionContext: [{0}] {1}; ContextHandle={2}, RundownReason={3}, RundownTime={4}", new object[]
					{
						this.Id,
						methodName,
						(this.contextHandle != null) ? this.contextHandle.ToString() : "<null>",
						rundownReason.Value,
						rundownTime
					});
					return;
				}
				ExTraceGlobals.SessionContextTracer.TraceInformation(47520, 0L, "SessionContext: [{0}] {1}; ContextHandle={2}, IdleTime={3}, IdleTimeout={4}, TimeRemaining={5}, ActivityCount={6}", new object[]
				{
					this.Id,
					methodName,
					(this.contextHandle != null) ? this.contextHandle.ToString() : "<null>",
					idleTime.TotalMilliseconds,
					idleTimeout.TotalMilliseconds,
					remainingTime.TotalMilliseconds,
					activityCount
				});
			}
		}

		private void ComputeState(out TimeSpan remainingTime, out SessionRundownReason? rundownReason, out ExDateTime rundownTime, out TimeSpan idleTime)
		{
			remainingTime = TimeSpan.Zero;
			rundownReason = null;
			rundownTime = ExDateTime.MinValue;
			idleTime = TimeSpan.FromMilliseconds((double)(this.sessionEnvironment.GetTickCount() - this.lastActivity));
			if (this.rundownReason != null)
			{
				rundownReason = this.rundownReason;
				rundownTime = this.rundownTime;
				idleTime = ((this.rundownReason.Value == SessionRundownReason.Expired) ? this.idleTimeout : TimeSpan.Zero);
				return;
			}
			if (this.markForRundownReason != null)
			{
				remainingTime = TimeSpan.Zero;
				idleTime = TimeSpan.Zero;
				if (this.activityCount == 0)
				{
					this.rundownReason = this.markForRundownReason;
					this.rundownTime = this.sessionEnvironment.GetUtcNow();
					rundownTime = this.rundownTime;
				}
				return;
			}
			if (this.activityCount != 0)
			{
				idleTime = TimeSpan.Zero;
				remainingTime = this.idleTimeout;
				return;
			}
			if (idleTime > this.idleTimeout)
			{
				remainingTime = TimeSpan.Zero;
				this.rundownReason = new SessionRundownReason?(SessionRundownReason.Expired);
				this.rundownTime = this.sessionEnvironment.GetUtcNow();
				rundownReason = this.rundownReason;
				rundownTime = this.rundownTime;
				idleTime = this.idleTimeout;
				return;
			}
			remainingTime = this.idleTimeout - idleTime;
		}

		private readonly object sessionContextLock = new object();

		private readonly UserContext userContext;

		private readonly string mailboxIdentifier;

		private readonly SessionContextIdentifier sessionContextIdentifier;

		private readonly ExDateTime creationTime;

		private readonly AsyncOperationTracker asyncOperationTracker = new AsyncOperationTracker();

		private TimeSpan idleTimeout;

		private object contextHandle;

		private SessionRundownReason? rundownReason;

		private ExDateTime rundownTime;

		private SessionRundownReason? markForRundownReason;

		private int activityCount;

		private int lastActivity;

		private ExDateTime lastActivityTime;

		private SessionContext.ISessionEnvironment sessionEnvironment;

		internal interface ISessionEnvironment
		{
			int GetTickCount();

			ExDateTime GetUtcNow();

			void WakeUpIdleContextMonitor(ExDateTime wakeupTime);
		}

		private sealed class DefaultSessionEnvironment : SessionContext.ISessionEnvironment
		{
			public static SessionContext.DefaultSessionEnvironment Instance
			{
				get
				{
					if (SessionContext.DefaultSessionEnvironment.instance == null)
					{
						SessionContext.DefaultSessionEnvironment.instance = new SessionContext.DefaultSessionEnvironment();
					}
					return SessionContext.DefaultSessionEnvironment.instance;
				}
			}

			int SessionContext.ISessionEnvironment.GetTickCount()
			{
				return Environment.TickCount;
			}

			ExDateTime SessionContext.ISessionEnvironment.GetUtcNow()
			{
				return ExDateTime.UtcNow;
			}

			void SessionContext.ISessionEnvironment.WakeUpIdleContextMonitor(ExDateTime wakeupTime)
			{
				SessionContextManager.WakeupIdleContextMonitor(wakeupTime);
			}

			private static SessionContext.DefaultSessionEnvironment instance;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Exchange.UM.UMCore.OCS;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UserNotificationEventManager
	{
		internal UserNotificationEventManager()
		{
			this.TraceDebug("UserNotificationEventManager()", new object[0]);
			UserNotificationEvent.TryParseTargetUser = new TryParseTargetUserDelegate(UserNotificationEventManager.TryParseTargetUser);
			TimeSpan sessionCheckTimerInterval = Constants.OCS.SessionCheckTimerInterval;
			this.sessionCheckTimer = new Timer(new TimerCallback(this.SessionCheckCallback), null, sessionCheckTimerInterval, sessionCheckTimerInterval);
		}

		internal void OnIncomingSession(DiagnosticHelper diagnostics, SessionReceivedEventArgs args, UserNotificationEventHandler handler)
		{
			this.TraceDebug("OnIncomingSession called", new object[0]);
			UserNotificationEventSession userNotificationEventSession = new UserNotificationEventSession(diagnostics, args.Session, handler);
			lock (this)
			{
				this.TraceDebug("Adding session {0} to the session table", new object[]
				{
					userNotificationEventSession.Id
				});
				this.sessionTable[userNotificationEventSession.Id] = userNotificationEventSession;
				if (this.sessionTable.Count == 1)
				{
					this.sessionTableEmpty.Reset();
				}
			}
			userNotificationEventSession.Start(args);
		}

		internal void Terminate()
		{
			this.TraceDebug("UserNotificationEventManager.Terminate()", new object[0]);
			lock (this)
			{
				this.TraceDebug("Disposing session check timer", new object[0]);
				if (this.sessionCheckTimer != null)
				{
					this.sessionCheckTimer.Dispose();
					this.sessionCheckTimer = null;
					TimeSpan timeSpan = TimeSpan.FromSeconds(2.0);
					TimeSpan timeSpan2 = TimeSpan.FromSeconds(30.0);
					do
					{
						this.TraceDebug("Waiting for all sessions to terminate", new object[0]);
						this.CloseInactiveSessions(true);
						timeSpan2 = timeSpan2.Subtract(timeSpan);
					}
					while (timeSpan2.TotalMilliseconds > 0.0 && !this.sessionTableEmpty.WaitOne(timeSpan, false));
					this.TraceDebug("All sessions terminated", new object[0]);
				}
				else
				{
					this.TraceDebug("UserNotificationEventManager was already terminated", new object[0]);
				}
			}
		}

		private static bool TryParseTargetUser(string target, out string userPart, out string hostPart, out bool isPhoneNumber)
		{
			userPart = string.Empty;
			hostPart = string.Empty;
			isPhoneNumber = false;
			SipUriParser sipUriParser = null;
			if (SipUriParser.TryParse(target, ref sipUriParser))
			{
				userPart = sipUriParser.User;
				hostPart = sipUriParser.Host;
				isPhoneNumber = string.Equals(sipUriParser.UserParameter, "phone", StringComparison.InvariantCultureIgnoreCase);
				return true;
			}
			return false;
		}

		private void RemoveSession(UserNotificationEventSession session)
		{
			this.TraceDebug("RemoveSession({0}) called", new object[]
			{
				session.Id
			});
			if (this.sessionTable.ContainsKey(session.Id))
			{
				this.TraceDebug("Removing session {0}", new object[]
				{
					session.Id
				});
				this.sessionTable.Remove(session.Id);
				if (this.sessionTable.Count == 0)
				{
					this.sessionTableEmpty.Set();
					return;
				}
			}
			else
			{
				this.TraceDebug("Session {0} had already been removed", new object[]
				{
					session.Id
				});
			}
		}

		private void SessionCheckCallback(object state)
		{
			this.TraceDebug("Begin SessionCheckCallback", new object[0]);
			lock (this)
			{
				if (this.sessionCheckTimer == null)
				{
					this.TraceDebug("SessionCheckCallback: timer already disposed, returning", new object[0]);
				}
				else
				{
					this.CloseInactiveSessions(false);
				}
			}
		}

		private void CloseInactiveSessions(bool terminating)
		{
			this.TraceDebug("Closing inactive sessions. terminating={0}", new object[]
			{
				terminating
			});
			TimeSpan sessionMaxIdleTime = Constants.OCS.SessionMaxIdleTime;
			List<UserNotificationEventSession> list = new List<UserNotificationEventSession>();
			foreach (UserNotificationEventSession userNotificationEventSession in this.sessionTable.Values)
			{
				lock (userNotificationEventSession)
				{
					if (!userNotificationEventSession.IsActive)
					{
						this.TraceDebug("Session {0} is inactive, adding it to the remove list", new object[]
						{
							userNotificationEventSession.Id
						});
						list.Add(userNotificationEventSession);
					}
					else if (terminating || userNotificationEventSession.IdleTime > sessionMaxIdleTime)
					{
						this.TraceDebug("Session {0} has been idle for {1} minutes and terminating={2}", new object[]
						{
							userNotificationEventSession.Id,
							userNotificationEventSession.IdleTime,
							terminating
						});
						userNotificationEventSession.Terminate();
					}
				}
			}
			foreach (UserNotificationEventSession userNotificationEventSession2 in list)
			{
				this.TraceDebug("Removing session {0}", new object[]
				{
					userNotificationEventSession2.Id
				});
				this.RemoveSession(userNotificationEventSession2);
			}
		}

		private void TraceDebug(string format, params object[] args)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, this.GetHashCode(), format, args);
		}

		private Dictionary<string, UserNotificationEventSession> sessionTable = new Dictionary<string, UserNotificationEventSession>();

		private Timer sessionCheckTimer;

		private ManualResetEvent sessionTableEmpty = new ManualResetEvent(true);
	}
}

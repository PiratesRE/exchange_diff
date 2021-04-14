using System;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceModel;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	internal class NotifyService : IInternalNotify
	{
		public NotifyService(INotifyCallback callback, NotificationListener listener)
		{
			this.m_callback = callback;
			this.m_listener = listener;
		}

		public void BecomePame()
		{
			if (!NotifyService.CheckSecurity())
			{
				return;
			}
			ReplayCrimsonEvents.TPRNotificationListenerReceivedBecomePame.Log();
			this.m_callback.BecomePame();
		}

		public void RevokePame()
		{
			if (!NotifyService.CheckSecurity())
			{
				return;
			}
			ReplayCrimsonEvents.TPRNotificationListenerReceivedRevokePame.Log();
			this.m_callback.RevokePame();
		}

		public NotificationResponse DatabaseMoveNeeded(Guid dbId, string currentActiveFqdn, bool mountDesired)
		{
			if (!NotifyService.CheckSecurity())
			{
				return NotificationResponse.Incomplete;
			}
			ReplayCrimsonEvents.TPRNotificationListenerReceivedDatabaseMoveNeeded.Log<Guid, string, bool>(dbId, currentActiveFqdn, mountDesired);
			NotificationResponse notificationResponse = this.m_callback.DatabaseMoveNeeded(dbId, currentActiveFqdn, mountDesired);
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug(0L, "NotificationListener is responded with {0} to DatabaseMoveNeeded({1},{2},{3})", new object[]
			{
				notificationResponse,
				dbId,
				currentActiveFqdn,
				mountDesired
			});
			return notificationResponse;
		}

		public int GetTimeouts(out TimeSpan retryDelay, out TimeSpan openTimeout, out TimeSpan sendTimeout, out TimeSpan receiveTimeout)
		{
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug(0L, "NotificationListener is responding to GetTimeouts");
			retryDelay = this.m_listener.RetryDelay;
			openTimeout = this.m_listener.OpenTimeout;
			sendTimeout = this.m_listener.SendTimeout;
			receiveTimeout = this.m_listener.ReceiveTimeout;
			return 0;
		}

		private static bool AuthorizeRequest()
		{
			WindowsIdentity windowsIdentity = ServiceSecurityContext.Current.PrimaryIdentity as WindowsIdentity;
			if (windowsIdentity == null)
			{
				return false;
			}
			IdentityReferenceCollection groups = windowsIdentity.Groups;
			foreach (IdentityReference left in groups)
			{
				if (left == NotifyService.s_localAdminsSid)
				{
					return true;
				}
			}
			return false;
		}

		private static bool CheckSecurity()
		{
			if (!NotifyService.AuthorizeRequest())
			{
				string name = ServiceSecurityContext.Current.PrimaryIdentity.Name;
				StackFrame stackFrame = new StackFrame(1);
				string name2 = stackFrame.GetMethod().Name;
				ExTraceGlobals.ThirdPartyClientTracer.TraceError<string, string>(0L, "Access denied to user '{0}'. MethodCalled='{1}'", name, name2);
				if (!NotifyService.s_authFailedLogged)
				{
					NotifyService.s_authFailedLogged = true;
					ReplayCrimsonEvents.TPRAuthorizationFailed.Log<string>(ServiceSecurityContext.Current.PrimaryIdentity.Name);
				}
				return false;
			}
			return true;
		}

		private static SecurityIdentifier s_localAdminsSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

		private static bool s_authFailedLogged = false;

		private INotifyCallback m_callback;

		private NotificationListener m_listener;
	}
}

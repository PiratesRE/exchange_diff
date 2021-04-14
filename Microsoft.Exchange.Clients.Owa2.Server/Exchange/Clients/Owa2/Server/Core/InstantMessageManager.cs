using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class InstantMessageManager : DisposeTrackableBase
	{
		internal InstantMessageManager(UserContext userContext)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager Constructor.");
			this.userContext = userContext;
		}

		public InstantMessageProvider Provider
		{
			get
			{
				return this.provider;
			}
		}

		public InstantMessageNotifier Notifier
		{
			get
			{
				return this.notifier;
			}
		}

		public void Subscribe(string subscriptionId)
		{
			if (this.notifier == null)
			{
				if (this.userContext.InstantMessageType == InstantMessagingTypeOptions.Ocs)
				{
					this.notifier = new InstantMessageOCSNotifier(this.userContext);
				}
				else
				{
					this.notifier = new InstantMessageNotifier(this.userContext);
				}
			}
			if (!this.notifier.IsRegistered)
			{
				this.notifier.SubscriptionId = subscriptionId;
				this.notifier.RegisterWithPendingRequestNotifier();
				if (this.userContext.PendingRequestManager != null)
				{
					this.userContext.PendingRequestManager.ClientDisconnected += this.ClientDisconnected;
				}
			}
		}

		public void TerminateProvider(string reason)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager.TerminateProvider.");
			InstantMessageProvider instantMessageProvider = Interlocked.Exchange<InstantMessageProvider>(ref this.provider, null);
			if (instantMessageProvider != null)
			{
				if (this.notifier != null)
				{
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.notifier, null, "Disconnected from IM by server due to timeout: " + reason, InstantMessageServiceError.ServerTimeout, false);
				}
				instantMessageProvider.Dispose();
			}
		}

		public void SignOut()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager.SignOut.");
			InstantMessageProvider instantMessageProvider = Interlocked.Exchange<InstantMessageProvider>(ref this.provider, null);
			if (instantMessageProvider != null)
			{
				if (this.notifier != null)
				{
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.notifier, null, "Signed out manually.", InstantMessageServiceError.ClientSignOut, false);
				}
				instantMessageProvider.Dispose();
			}
		}

		public InstantMessageOperationError StartProvider(MailboxSession session)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager.StartProvider.");
			if ((this.provider == null || this.provider.IsDisposed) && this.userContext.InstantMessageType == InstantMessagingTypeOptions.Ocs)
			{
				this.Subscribe("InstantMessageNotification");
				Stopwatch stopwatch = Stopwatch.StartNew();
				this.provider = InstantMessageOCSProvider.Create(this.userContext, this.notifier);
				stopwatch.Stop();
				OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.CreateProvider, stopwatch.ElapsedMilliseconds);
			}
			if (this.provider == null)
			{
				return InstantMessageOperationError.UnableToCreateProvider;
			}
			Stopwatch stopwatch2 = Stopwatch.StartNew();
			this.Provider.EstablishSession();
			stopwatch2.Stop();
			OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.EstablishSession, stopwatch2.ElapsedMilliseconds);
			Stopwatch stopwatch3 = Stopwatch.StartNew();
			this.Provider.GetExpandedGroups(session);
			stopwatch3.Stop();
			OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.GetExpandedGroups, stopwatch3.ElapsedMilliseconds);
			Stopwatch stopwatch4 = Stopwatch.StartNew();
			this.ResetPresence(true);
			stopwatch4.Stop();
			OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.ResetPresence, stopwatch4.ElapsedMilliseconds);
			return InstantMessageOperationError.Success;
		}

		public void ResetPresence()
		{
			this.ResetPresence(false);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<bool>((long)this.GetHashCode(), "InstantMessageManager.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing)
			{
				if (this.userContext.PendingRequestManager != null)
				{
					this.userContext.PendingRequestManager.ClientDisconnected -= this.ClientDisconnected;
				}
				this.TerminateProvider("Dispose");
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<InstantMessageManager>(this);
		}

		private void ResetPresence(bool forceReset)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager.ResetPresence");
			if (this.Provider != null && (forceReset || this.Provider.IsActivityBasedPresenceSet))
			{
				this.Provider.ResetPresence();
			}
		}

		private void ClientDisconnected(object sender, EventArgs e)
		{
			this.TerminateProvider("ClientDisconnect");
		}

		private InstantMessageProvider provider;

		private InstantMessageNotifier notifier;

		private UserContext userContext;
	}
}

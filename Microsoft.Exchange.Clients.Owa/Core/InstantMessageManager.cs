using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class InstantMessageManager : DisposeTrackableBase
	{
		internal InstantMessageManager(UserContext userContext)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager Constructor.");
			this.userContext = userContext;
			if (userContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
			{
				this.payload = new InstantMessageOCSPayload(userContext);
			}
			else
			{
				this.payload = new InstantMessagePayload(userContext);
			}
			this.payload.RegisterWithPendingRequestNotifier();
			if (userContext.PendingRequestManager != null)
			{
				userContext.PendingRequestManager.ClientDisconnected += this.ClientDisconnected;
			}
		}

		private void ClientDisconnected(object sender, EventArgs e)
		{
			this.TerminateProvider("ClientDisconnect");
		}

		public InstantMessageProvider Provider
		{
			get
			{
				return this.provider;
			}
		}

		public InstantMessagePayload Payload
		{
			get
			{
				return this.payload;
			}
		}

		public void TerminateProvider(string reason)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager.TerminateProvider.");
			InstantMessageProvider instantMessageProvider = Interlocked.Exchange<InstantMessageProvider>(ref this.provider, null);
			if (instantMessageProvider != null)
			{
				if (this.payload != null)
				{
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.payload, null, "Disconnected from IM by server due to timeout: " + reason, InstantMessageFailure.ServerTimeout, false);
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
				if (this.payload != null)
				{
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.payload, null, "Signed out manually.", InstantMessageFailure.ClientSignOut, false);
				}
				instantMessageProvider.Dispose();
			}
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

		public void StartProvider()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager.StartProvider.");
			if (this.userContext.IsSignedOutOfIM())
			{
				if (this.payload != null)
				{
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.payload, null, "Not signed in because IsSignedOutOfIM was true.", InstantMessageFailure.ClientSignOut, false);
				}
			}
			else if ((this.provider == null || this.provider.IsDisposed) && this.userContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
			{
				this.StartOcsProvider();
			}
			if (this.provider != null)
			{
				this.Provider.EstablishSession();
			}
		}

		public void ResetPresence()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageManager.ResetPresence");
			if (this.Provider != null)
			{
				this.Provider.MakeEndpointMostActive();
				if (this.Provider.IsActivityBasedPresenceSet)
				{
					this.Provider.ResetPresence();
				}
			}
		}

		private void StartOcsProvider()
		{
			this.provider = InstantMessageOCSProvider.Create(this.userContext, this.payload);
		}

		private InstantMessageProvider provider;

		private InstantMessagePayload payload;

		private UserContext userContext;
	}
}

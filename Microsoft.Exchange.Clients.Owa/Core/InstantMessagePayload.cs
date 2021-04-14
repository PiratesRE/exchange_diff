using System;
using System.Text;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class InstantMessagePayload : IPendingRequestNotifier
	{
		public event EventHandler<EventArgs> ChangeUserPresenceAfterInactivity;

		public bool ShouldThrottle
		{
			get
			{
				return false;
			}
		}

		internal InstantMessagePayload(UserContext userContext)
		{
			this.payloadString = new StringBuilder(256);
			this.userContext = userContext;
		}

		internal void RegisterWithPendingRequestNotifier()
		{
			if (this.userContext != null && this.userContext.PendingRequestManager != null)
			{
				this.userContext.PendingRequestManager.AddPendingRequestNotifier(this);
			}
		}

		public event DataAvailableEventHandler DataAvailable;

		public int Length
		{
			get
			{
				return this.payloadString.Length;
			}
		}

		public virtual string ReadDataAndResetState()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<string>((long)this.GetHashCode(), "InstantMessagePayload.ReadDataAndResetState. SIP Uri: {0}", this.GetUriForUser());
			string result;
			lock (this)
			{
				result = this.payloadString.ToString();
				this.Clear();
			}
			return result;
		}

		public void Append(string value)
		{
			if (this.payloadString.Length < 1000000)
			{
				if (!this.overMaxSize)
				{
					this.payloadString.Append(value);
					return;
				}
			}
			else
			{
				this.LogPayloadNotPickedEvent();
			}
		}

		public void Append(int value)
		{
			if (this.payloadString.Length < 1000000)
			{
				if (!this.overMaxSize)
				{
					this.payloadString.Append(value);
					return;
				}
			}
			else
			{
				this.LogPayloadNotPickedEvent();
			}
		}

		public void Append(StringBuilder value)
		{
			if (this.payloadString.Length < 1000000)
			{
				if (!this.overMaxSize)
				{
					this.payloadString.Append(value);
					return;
				}
			}
			else
			{
				this.LogPayloadNotPickedEvent();
			}
		}

		public void Clear()
		{
			this.payloadString.Remove(0, this.payloadString.Length);
		}

		public void PickupData(int length)
		{
			if (length == 0 && this.payloadString.Length > 0)
			{
				this.DataAvailable(this, new EventArgs());
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<string>((long)this.GetHashCode(), "InstantMessagePayload.PickupData. DataAvailable method called. SIP Uri: {0}", this.GetUriForUser());
			}
			else
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<string>((long)this.GetHashCode(), "InstantMessagePayload.PickupData. No need to call DataAvailable method. SIP Uri: {0}", this.GetUriForUser());
			}
			if (this.overMaxSize)
			{
				lock (this)
				{
					this.overMaxSize = false;
				}
			}
		}

		public void ConnectionAliveTimer()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<string>((long)this.GetHashCode(), "InstantMessagePayload.ConnectionAliveTimer. User: {0}", this.GetUriForUser());
			long num = Globals.ApplicationTime - this.userContext.LastUserRequestTime;
			if (num > (long)Globals.ActivityBasedPresenceDuration)
			{
				EventArgs e = new EventArgs();
				EventHandler<EventArgs> changeUserPresenceAfterInactivity = this.ChangeUserPresenceAfterInactivity;
				if (changeUserPresenceAfterInactivity != null)
				{
					changeUserPresenceAfterInactivity(this, e);
				}
			}
		}

		protected virtual void Cancel()
		{
			this.Clear();
		}

		private void LogPayloadNotPickedEvent()
		{
			if (!this.overMaxSize)
			{
				this.overMaxSize = true;
				string uriForUser = this.GetUriForUser();
				ExTraceGlobals.InstantMessagingTracer.TraceError<string>((long)this.GetHashCode(), "InstantMessagePayload.LogPayloadNotPickedEvent. Payload has grown too large without being picked up. User: {0}", uriForUser);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_PayloadNotBeingPickedup, string.Empty, new object[]
				{
					this.GetUriForUser()
				});
				PendingRequestManager pendingRequestManager = this.userContext.PendingRequestManager;
				if (pendingRequestManager != null)
				{
					ChunkedHttpResponse chunkedHttpResponse = pendingRequestManager.ChunkedHttpResponse;
					if (chunkedHttpResponse != null && chunkedHttpResponse.IsClientConnected)
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessagePayload.LogPayloadNotPickedEvent", this.userContext, new OverflowException(string.Format("Payload has grown too large without being picked up. User: {0}", uriForUser)));
					}
				}
				this.Cancel();
				this.payloadString.Append(InstantMessagePayload.overMaxSizeUnavailablePayload);
			}
		}

		private string GetUriForUser()
		{
			if (this.userContext.InstantMessagingType != InstantMessagingTypeOptions.Ocs)
			{
				return string.Empty;
			}
			return this.userContext.SipUri;
		}

		private const int MaxPayloadSize = 1000000;

		private const int DefaultPayloadStringSize = 256;

		private static string overMaxSizeUnavailablePayload = "UN(" + 2004 + ");";

		private volatile bool overMaxSize;

		private StringBuilder payloadString;

		protected UserContext userContext;
	}
}

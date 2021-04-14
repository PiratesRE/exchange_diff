using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PlayOnPhoneStateProvider : DisposeTrackableBase, IPlayOnPhoneStateProvider, IDisposable
	{
		public PlayOnPhoneStateProvider(UserContext userContext)
		{
			this.client = new UMClientCommon(userContext.ExchangePrincipal);
		}

		public virtual UMCallState GetCallState(string callId)
		{
			UMCallState result = UMCallState.Disconnected;
			try
			{
				result = this.client.GetCallInfo(callId).CallState;
			}
			catch (InvalidCallIdException)
			{
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.client != null)
			{
				this.client.Dispose();
				this.client = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PlayOnPhoneStateProvider>(this);
		}

		private UMClientCommon client;
	}
}

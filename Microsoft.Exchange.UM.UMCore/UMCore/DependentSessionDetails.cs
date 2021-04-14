using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DependentSessionDetails
	{
		internal DependentSessionDetails(UMCallSessionHandler<OutboundCallDetailsEventArgs> args, UMSubscriber caller, string callerId, UMSipPeer remotePeer, PhoneNumber numberToCall, BaseUMCallSession parentUMCallSession)
		{
			this.caller = caller;
			this.callerId = callerId;
			this.numberToCall = numberToCall;
			this.remotePeerToUse = remotePeer;
			this.onoutboundCallRequestCompleted = args;
			this.parentUMCallSession = parentUMCallSession;
		}

		internal UMSubscriber Caller
		{
			get
			{
				return this.caller;
			}
		}

		internal string CallerID
		{
			get
			{
				return this.callerId;
			}
		}

		internal BaseUMCallSession ParentUMCallSession
		{
			get
			{
				return this.parentUMCallSession;
			}
		}

		internal UMSipPeer RemotePeerToUse
		{
			get
			{
				return this.remotePeerToUse;
			}
		}

		internal PhoneNumber NumberToCall
		{
			get
			{
				return this.numberToCall;
			}
			set
			{
				this.numberToCall = value;
			}
		}

		internal BaseUMCallSession DependentUMCallSession
		{
			get
			{
				return this.dependentUMCallSession;
			}
			set
			{
				this.dependentUMCallSession = value;
			}
		}

		internal UMCallSessionHandler<OutboundCallDetailsEventArgs> OutBoundCallConnectedHandler
		{
			get
			{
				return this.onoutboundCallRequestCompleted;
			}
		}

		private UMCallSessionHandler<OutboundCallDetailsEventArgs> onoutboundCallRequestCompleted;

		private UMSubscriber caller;

		private string callerId;

		private PhoneNumber numberToCall;

		private UMSipPeer remotePeerToUse;

		private BaseUMCallSession parentUMCallSession;

		private BaseUMCallSession dependentUMCallSession;
	}
}

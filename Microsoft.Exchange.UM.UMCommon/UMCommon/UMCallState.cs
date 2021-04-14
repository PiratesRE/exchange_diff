using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public enum UMCallState
	{
		Idle,
		Connecting,
		Alerted,
		Connected,
		Disconnected,
		Incoming,
		Transferring,
		Forwarding
	}
}

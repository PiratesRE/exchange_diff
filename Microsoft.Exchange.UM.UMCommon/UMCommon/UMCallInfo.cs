using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public class UMCallInfo
	{
		public UMCallInfo()
		{
		}

		public UMCallInfo(UMCallInfoEx properties)
		{
			this.callState = properties.CallState;
			this.EventCause = properties.EventCause;
		}

		public UMCallState CallState
		{
			get
			{
				return this.callState;
			}
			set
			{
				this.callState = value;
			}
		}

		public UMEventCause EventCause
		{
			get
			{
				return this.eventCause;
			}
			set
			{
				this.eventCause = value;
			}
		}

		private UMCallState callState = UMCallState.Disconnected;

		private UMEventCause eventCause;
	}
}

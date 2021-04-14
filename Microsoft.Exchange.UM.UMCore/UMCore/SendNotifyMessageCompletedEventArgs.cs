using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SendNotifyMessageCompletedEventArgs : EventArgs
	{
		public Exception Error { get; set; }

		public int ResponseCode { get; set; }

		public string ResponseReason { get; set; }

		public object UserState { get; set; }
	}
}

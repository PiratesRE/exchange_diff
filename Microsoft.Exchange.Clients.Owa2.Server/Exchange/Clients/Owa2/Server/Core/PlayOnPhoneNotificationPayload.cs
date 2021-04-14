using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class PlayOnPhoneNotificationPayload : NotificationPayloadBase
	{
		public PlayOnPhoneNotificationPayload()
		{
		}

		public PlayOnPhoneNotificationPayload(string callState)
		{
			this.CallState = callState;
		}

		public string CallState
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

		[DataMember(EmitDefaultValue = false)]
		private string callState;
	}
}

using System;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class PlayOnPhoneResponse : ResponseBase
	{
		public string CallId
		{
			get
			{
				return this.callId;
			}
			set
			{
				this.callId = value;
			}
		}

		private string callId;
	}
}

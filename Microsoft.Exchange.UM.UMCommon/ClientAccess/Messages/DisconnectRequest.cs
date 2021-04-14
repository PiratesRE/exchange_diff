using System;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class DisconnectRequest : RequestBase
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

		internal override string GetFriendlyName()
		{
			return Strings.DisconnectRequest;
		}

		private string callId;
	}
}

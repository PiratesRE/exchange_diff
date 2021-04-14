using System;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class PlayOnPhoneMessageRequest : PlayOnPhoneUserRequest
	{
		public string ObjectId
		{
			get
			{
				return this.objectId;
			}
			set
			{
				this.objectId = value;
			}
		}

		internal override string GetFriendlyName()
		{
			return Strings.PlayOnPhoneMessageRequest;
		}

		private string objectId;
	}
}

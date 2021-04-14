using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class PlayOnPhoneGreetingRequest : PlayOnPhoneUserRequest
	{
		public UMGreetingType GreetingType
		{
			get
			{
				return this.greetingType;
			}
			set
			{
				this.greetingType = value;
			}
		}

		internal override string GetFriendlyName()
		{
			return Strings.PlayOnPhoneGreetingRequest;
		}

		private UMGreetingType greetingType;
	}
}

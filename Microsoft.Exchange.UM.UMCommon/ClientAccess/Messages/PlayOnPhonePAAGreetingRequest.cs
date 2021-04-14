using System;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class PlayOnPhonePAAGreetingRequest : PlayOnPhoneUserRequest
	{
		public Guid Identity
		{
			get
			{
				return this.paaIdentity;
			}
			set
			{
				this.paaIdentity = value;
			}
		}

		internal override string GetFriendlyName()
		{
			return Strings.PlayOnPhonePAAGreetingRequest;
		}

		private Guid paaIdentity;
	}
}

using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class SendMeetingInvitationsOrCancellationsRequiredException : ServicePermanentException
	{
		public SendMeetingInvitationsOrCancellationsRequiredException() : base((CoreResources.IDs)3422864683U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}

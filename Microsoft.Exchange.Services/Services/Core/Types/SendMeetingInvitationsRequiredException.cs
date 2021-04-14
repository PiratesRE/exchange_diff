using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class SendMeetingInvitationsRequiredException : ServicePermanentException
	{
		public SendMeetingInvitationsRequiredException() : base((CoreResources.IDs)3383701276U)
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

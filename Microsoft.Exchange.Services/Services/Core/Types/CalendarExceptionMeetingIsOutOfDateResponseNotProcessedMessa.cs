using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionMeetingIsOutOfDateResponseNotProcessedMessageSent : ServicePermanentException
	{
		public CalendarExceptionMeetingIsOutOfDateResponseNotProcessedMessageSent() : base((CoreResources.IDs)3407017993U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}

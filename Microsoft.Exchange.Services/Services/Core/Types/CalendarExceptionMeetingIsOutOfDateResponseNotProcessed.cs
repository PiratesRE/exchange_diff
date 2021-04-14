using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionMeetingIsOutOfDateResponseNotProcessed : ServicePermanentException
	{
		public CalendarExceptionMeetingIsOutOfDateResponseNotProcessed() : base((CoreResources.IDs)3573754788U)
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

using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionMeetingRequestIsOutOfDate : ServicePermanentException
	{
		public CalendarExceptionMeetingRequestIsOutOfDate() : base((CoreResources.IDs)3227656327U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}

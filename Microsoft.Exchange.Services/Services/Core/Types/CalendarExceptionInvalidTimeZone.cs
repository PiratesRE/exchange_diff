using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionInvalidTimeZone : ServicePermanentException
	{
		public CalendarExceptionInvalidTimeZone(Exception exception) : base(CoreResources.IDs.ErrorCalendarInvalidTimeZone, exception)
		{
		}

		public CalendarExceptionInvalidTimeZone(Enum messageId) : base(ResponseCodeType.ErrorCalendarInvalidTimeZone, messageId)
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

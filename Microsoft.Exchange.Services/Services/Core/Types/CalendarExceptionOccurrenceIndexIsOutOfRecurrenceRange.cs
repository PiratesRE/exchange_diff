using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionOccurrenceIndexIsOutOfRecurrenceRange : ServicePermanentException
	{
		public CalendarExceptionOccurrenceIndexIsOutOfRecurrenceRange() : base(CoreResources.IDs.ErrorCalendarOccurrenceIndexIsOutOfRecurrenceRange)
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

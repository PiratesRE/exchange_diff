using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionCannotUseIdForRecurringMasterId : ServicePermanentException
	{
		public CalendarExceptionCannotUseIdForRecurringMasterId() : base(CoreResources.IDs.ErrorCalendarCannotUseIdForRecurringMasterId)
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

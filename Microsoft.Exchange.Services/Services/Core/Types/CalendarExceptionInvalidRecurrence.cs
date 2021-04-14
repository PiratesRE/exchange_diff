using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionInvalidRecurrence : ServicePermanentException
	{
		public CalendarExceptionInvalidRecurrence() : base(CoreResources.IDs.ErrorCalendarInvalidRecurrence)
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

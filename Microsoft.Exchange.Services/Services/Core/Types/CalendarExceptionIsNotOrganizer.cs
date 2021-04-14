using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionIsNotOrganizer : ServicePermanentException
	{
		public CalendarExceptionIsNotOrganizer() : base(CoreResources.IDs.ErrorCalendarIsNotOrganizer)
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

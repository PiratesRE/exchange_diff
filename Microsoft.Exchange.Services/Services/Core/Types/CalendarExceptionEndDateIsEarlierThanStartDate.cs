using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionEndDateIsEarlierThanStartDate : ServicePermanentException
	{
		public CalendarExceptionEndDateIsEarlierThanStartDate() : base((CoreResources.IDs)4006585486U)
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

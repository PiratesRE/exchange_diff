using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionCannotUseIdForOccurrenceId : ServicePermanentException
	{
		public CalendarExceptionCannotUseIdForOccurrenceId() : base((CoreResources.IDs)4180336284U)
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

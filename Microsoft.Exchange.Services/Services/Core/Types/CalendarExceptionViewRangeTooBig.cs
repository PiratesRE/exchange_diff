using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionViewRangeTooBig : ServicePermanentException
	{
		public CalendarExceptionViewRangeTooBig() : base((CoreResources.IDs)2945703152U)
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

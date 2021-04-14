using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionDurationIsTooLong : ServicePermanentException
	{
		public CalendarExceptionDurationIsTooLong() : base((CoreResources.IDs)2484699530U)
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

using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionIsCancelledMessageSent : ServicePermanentException
	{
		public CalendarExceptionIsCancelledMessageSent() : base((CoreResources.IDs)3167358706U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}

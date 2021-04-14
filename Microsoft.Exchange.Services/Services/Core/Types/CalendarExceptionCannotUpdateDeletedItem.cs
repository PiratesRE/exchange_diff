using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionCannotUpdateDeletedItem : ServicePermanentException
	{
		public CalendarExceptionCannotUpdateDeletedItem() : base((CoreResources.IDs)3843271914U)
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

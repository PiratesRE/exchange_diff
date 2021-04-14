using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionSeekToConditionNotSupported : ServicePermanentException
	{
		public CalendarExceptionSeekToConditionNotSupported() : base(ResponseCodeType.ErrorCalendarSeekToConditionNotSupported, CoreResources.ErrorCalendarSeekToConditionNotSupported("SeekToConditionPageView"))
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2013;
			}
		}
	}
}

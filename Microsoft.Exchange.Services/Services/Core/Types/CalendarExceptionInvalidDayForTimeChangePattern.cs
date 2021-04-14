using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionInvalidDayForTimeChangePattern : ServicePermanentExceptionWithPropertyPath
	{
		public CalendarExceptionInvalidDayForTimeChangePattern(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorCalendarInvalidDayForTimeChangePattern, propertyPath)
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

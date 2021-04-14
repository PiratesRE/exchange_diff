using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionInvalidDayForWeeklyRecurrence : ServicePermanentExceptionWithPropertyPath
	{
		public CalendarExceptionInvalidDayForWeeklyRecurrence(PropertyPath propertyPath) : base((CoreResources.IDs)2681298929U, propertyPath)
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

using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionInvalidPropertyState : ServicePermanentExceptionWithPropertyPath
	{
		public CalendarExceptionInvalidPropertyState(PropertyPath propertyPath) : base(CoreResources.IDs.ErrorCalendarInvalidPropertyState, propertyPath)
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

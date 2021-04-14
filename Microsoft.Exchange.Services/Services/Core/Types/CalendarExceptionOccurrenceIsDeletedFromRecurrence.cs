using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionOccurrenceIsDeletedFromRecurrence : ServicePermanentException
	{
		public CalendarExceptionOccurrenceIsDeletedFromRecurrence() : base((CoreResources.IDs)3335161738U)
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

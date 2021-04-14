using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionCannotMoveOrCopyOccurrence : ServicePermanentException
	{
		public CalendarExceptionCannotMoveOrCopyOccurrence() : base(CoreResources.IDs.ErrorCalendarCannotMoveOrCopyOccurrence)
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

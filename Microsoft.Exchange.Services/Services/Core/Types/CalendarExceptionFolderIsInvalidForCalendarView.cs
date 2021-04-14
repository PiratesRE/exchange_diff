using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionFolderIsInvalidForCalendarView : ServicePermanentException
	{
		public CalendarExceptionFolderIsInvalidForCalendarView() : base((CoreResources.IDs)2989650895U)
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

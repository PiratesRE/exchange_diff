using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionIsCancelled : ServicePermanentException
	{
		static CalendarExceptionIsCancelled()
		{
			CalendarExceptionIsCancelled.errorMappings.Add(typeof(AcceptItemType).Name, CoreResources.IDs.ErrorCalendarIsCancelledForAccept);
			CalendarExceptionIsCancelled.errorMappings.Add(typeof(DeclineItemType).Name, (CoreResources.IDs)2997278338U);
			CalendarExceptionIsCancelled.errorMappings.Add(typeof(TentativelyAcceptItemType).Name, CoreResources.IDs.ErrorCalendarIsCancelledForTentative);
			CalendarExceptionIsCancelled.errorMappings.Add(typeof(RemoveItemType).Name, (CoreResources.IDs)4064247940U);
		}

		public CalendarExceptionIsCancelled(string operation) : base(CalendarExceptionIsCancelled.errorMappings[operation])
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}

		private static Dictionary<string, Enum> errorMappings = new Dictionary<string, Enum>();
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class CalendarExceptionIsOrganizer : ServicePermanentException
	{
		static CalendarExceptionIsOrganizer()
		{
			CalendarExceptionIsOrganizer.errorMappings.Add(typeof(AcceptItemType).Name, (CoreResources.IDs)2633097826U);
			CalendarExceptionIsOrganizer.errorMappings.Add(typeof(DeclineItemType).Name, (CoreResources.IDs)2980490932U);
			CalendarExceptionIsOrganizer.errorMappings.Add(typeof(TentativelyAcceptItemType).Name, (CoreResources.IDs)3371251772U);
			CalendarExceptionIsOrganizer.errorMappings.Add(typeof(RemoveItemType).Name, CoreResources.IDs.ErrorCalendarIsOrganizerForRemove);
		}

		public CalendarExceptionIsOrganizer(string operation) : base(CalendarExceptionIsOrganizer.errorMappings[operation])
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		private static Dictionary<string, Enum> errorMappings = new Dictionary<string, Enum>();
	}
}

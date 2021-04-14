using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SchemaInfo
	{
		internal SchemaInfo(CalendarPropertyId calendarPropertyId, object promotionMethod, object demotionMethod, CalendarMethod methods, IcalFlags flags)
		{
			this.CalendarPropertyId = calendarPropertyId;
			this.PromotionMethod = promotionMethod;
			this.DemotionMethod = demotionMethod;
			this.Methods = methods;
			this.Flags = flags;
		}

		public bool IsMultiValue
		{
			get
			{
				return (this.Flags & IcalFlags.MultiValue) == IcalFlags.MultiValue;
			}
		}

		internal readonly CalendarPropertyId CalendarPropertyId;

		internal readonly object PromotionMethod;

		internal readonly object DemotionMethod;

		internal readonly CalendarMethod Methods;

		internal readonly IcalFlags Flags;
	}
}

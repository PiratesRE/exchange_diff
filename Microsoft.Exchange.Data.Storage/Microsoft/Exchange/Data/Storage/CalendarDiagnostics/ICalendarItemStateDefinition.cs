using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICalendarItemStateDefinition : IEquatable<ICalendarItemStateDefinition>, IEqualityComparer<CalendarItemState>
	{
		bool IsRecurringMasterSpecific { get; }

		string SchemaKey { get; }

		StorePropertyDefinition[] RequiredProperties { get; }

		bool Evaluate(CalendarItemState state, PropertyBag propertyBag, MailboxSession session);
	}
}

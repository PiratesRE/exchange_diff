using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditRecordStrategy<T>
	{
		SortBy[] QuerySortOrder { get; }

		PropertyDefinition[] Columns { get; }

		bool RecordFilter(IReadOnlyPropertyBag propertyBag, out bool stopNow);

		T Convert(IReadOnlyPropertyBag propertyBag);
	}
}

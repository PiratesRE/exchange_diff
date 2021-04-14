using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditQueryStrategy<T>
	{
		bool RecordFilter(IReadOnlyPropertyBag propertyBag, out bool stopNow);

		T Convert(IReadOnlyPropertyBag propertyBag);

		Exception GetTimeoutException(TimeSpan timeout);

		Exception GetQueryFailedException();
	}
}

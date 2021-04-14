using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharePointSignalStore
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IAnalyticsSignalSource
	{
		IEnumerable<AnalyticsSignal> GetSignals();

		string GetSourceName();
	}
}

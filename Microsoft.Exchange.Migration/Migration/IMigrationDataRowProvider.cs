using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationDataRowProvider
	{
		IEnumerable<IMigrationDataRow> GetNextBatchItem(string cursorPosition, int maxCountHint);
	}
}

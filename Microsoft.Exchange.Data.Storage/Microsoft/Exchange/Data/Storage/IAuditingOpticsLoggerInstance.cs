using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAuditingOpticsLoggerInstance
	{
		void InternalLogRow(List<KeyValuePair<string, object>> customData);
	}
}

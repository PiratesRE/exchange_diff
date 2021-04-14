using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IActivityLogger
	{
		void Log(IEnumerable<Activity> activities);
	}
}

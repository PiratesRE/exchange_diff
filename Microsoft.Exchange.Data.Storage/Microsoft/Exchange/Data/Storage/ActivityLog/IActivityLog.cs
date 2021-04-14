using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IActivityLog
	{
		void Append(IEnumerable<Activity> activities);

		IEnumerable<Activity> Query();

		void Reset();

		bool IsGroup();
	}
}

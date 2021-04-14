using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal interface IGroupQueuesDataProvider
	{
		void Start();

		void Stop();

		IDictionary<ADObjectId, ServerQueuesSnapshot> GetCurrentGroupServerToQueuesMap();
	}
}

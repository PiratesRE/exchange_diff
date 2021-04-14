using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal interface ILocalQueuesDataProvider
	{
		void Start();

		void Stop();

		ADObjectId GetLocalServerId();

		ServerQueuesSnapshot GetLocalServerQueues();
	}
}

using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class ServerLocatorPerfmonCounters
	{
		public void RecordOneWCFCall()
		{
			ReplayServerPerfmon.WCFGetServerForDatabaseCalls.Increment();
			ReplayServerPerfmon.WCFGetServerForDatabaseCallsPerSec.Increment();
		}

		public void RecordOneWCFGetAllCall()
		{
			ReplayServerPerfmon.WCFGetAllCalls.Increment();
			ReplayServerPerfmon.WCFGetAllCallsPerSec.Increment();
		}

		public void RecordOneWCFCallError()
		{
			ReplayServerPerfmon.WCFGetServerForDatabaseCallErrors.Increment();
			ReplayServerPerfmon.WCFGetServerForDatabaseCallErrorsPerSec.Increment();
		}

		public void RecordWCFCallLatency(long tics)
		{
			ReplayServerPerfmon.AvgWCFCallLatency.IncrementBy(tics);
			ReplayServerPerfmon.AvgWCFCallLatencyBase.Increment();
		}

		public void RecordWCFGetAllCallLatency(long tics)
		{
			ReplayServerPerfmon.AvgWCFGetAllCallLatency.IncrementBy(tics);
			ReplayServerPerfmon.AvgWCFGetAllCallLatencyBase.Increment();
		}
	}
}

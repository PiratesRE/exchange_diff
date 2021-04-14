using System;
using System.Collections.Concurrent;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class ProbeRunSequence
	{
		public static long GetProbeRunSequenceNumber(string probeId, out bool firstRun)
		{
			firstRun = false;
			long num = 0L;
			if (!ProbeRunSequence.dictProbeRunSeqNumber.TryGetValue(probeId, out num))
			{
				firstRun = true;
				num = DateTime.UtcNow.Ticks;
			}
			num += 1L;
			ProbeRunSequence.dictProbeRunSeqNumber[probeId] = num;
			return num;
		}

		private static ConcurrentDictionary<string, long> dictProbeRunSeqNumber = new ConcurrentDictionary<string, long>();
	}
}

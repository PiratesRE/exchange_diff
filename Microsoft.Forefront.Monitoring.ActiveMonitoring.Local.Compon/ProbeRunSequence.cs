using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class ProbeRunSequence
	{
		public static long GetProbeRunSequenceNumber(string probeId, out bool firstRun)
		{
			long num;
			long probeRunSequenceNumber = ProbeRunSequence.GetProbeRunSequenceNumber(probeId, out num);
			firstRun = (num == 1L);
			return probeRunSequenceNumber;
		}

		public static long GetProbeRunSequenceNumber(string probeId, out long count)
		{
			ProbeRunSequence.SequenceInfo orAdd = ProbeRunSequence.dictProbeRunSeqNumber.GetOrAdd(probeId, (string key) => new ProbeRunSequence.SequenceInfo(DateTime.UtcNow.Ticks));
			return orAdd.GetNextSequence(out count);
		}

		private static ConcurrentDictionary<string, ProbeRunSequence.SequenceInfo> dictProbeRunSeqNumber = new ConcurrentDictionary<string, ProbeRunSequence.SequenceInfo>();

		private class SequenceInfo
		{
			public SequenceInfo(long value)
			{
				this.currentValue = value;
				this.StartValue = value;
			}

			public long StartValue { get; private set; }

			public long GetNextSequence(out long count)
			{
				long num = Interlocked.Increment(ref this.currentValue);
				count = num - this.StartValue;
				return num;
			}

			private long currentValue;
		}
	}
}

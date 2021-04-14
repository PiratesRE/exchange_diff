using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal sealed class RpcFailureCounters : IRpcCounters
	{
		public RpcFailureCounters()
		{
			this.failureCounters = new Dictionary<uint, int>();
		}

		public void IncrementCounter(IRpcCounterData counterData)
		{
			FailureCounterData failureCounterData = counterData as FailureCounterData;
			int num = 0;
			if (!this.failureCounters.TryGetValue(failureCounterData.FailureCode, out num))
			{
				this.failureCounters.Add(failureCounterData.FailureCode, 1);
				return;
			}
			this.failureCounters[failureCounterData.FailureCode] = num + 1;
		}

		private IEnumerable<KeyValuePair<uint, int>> GetTopFailureCounters(out int othersCount)
		{
			IOrderedEnumerable<KeyValuePair<uint, int>> source = from entry in this.failureCounters
			orderby entry.Value descending
			select entry;
			othersCount = source.Skip(10).Sum((KeyValuePair<uint, int> entry) => entry.Value);
			return source.Take(10);
		}

		public override string ToString()
		{
			int num = 0;
			IEnumerable<KeyValuePair<uint, int>> topFailureCounters = this.GetTopFailureCounters(out num);
			List<string> list = (from entry in topFailureCounters
			select string.Format("0x{0:X}={1}", entry.Key, entry.Value)).ToList<string>();
			if (num != 0)
			{
				list.Add(string.Format("O={0}", num));
			}
			return string.Join(";", list);
		}

		private const int NumberOfRelevantCounters = 10;

		private readonly IDictionary<uint, int> failureCounters;
	}
}

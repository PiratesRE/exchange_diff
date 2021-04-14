using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class CountOperationStatistics : OperationStatistics
	{
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		internal override void AddCall(float value = 0f, int count = 1)
		{
			Interlocked.Add(ref this.count, count);
		}

		internal override void Merge(OperationStatistics s2)
		{
			CountOperationStatistics countOperationStatistics = s2 as CountOperationStatistics;
			if (countOperationStatistics != null)
			{
				this.count += countOperationStatistics.count;
			}
		}

		internal override void AppendStatistics(OperationKey operationKey, List<KeyValuePair<string, object>> customData)
		{
			customData.Add(new KeyValuePair<string, object>(base.ToCountKey(operationKey), this.Count));
		}

		private int count;
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class AverageOperationStatistics : OperationStatistics
	{
		internal AverageOperationStatistics()
		{
			this.total = 0.0;
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public float CumulativeAverage
		{
			get
			{
				if (this.count == 0)
				{
					return 0f;
				}
				return (float)(this.total / (double)this.count);
			}
		}

		internal override void AppendStatistics(OperationKey operationKey, List<KeyValuePair<string, object>> customData)
		{
			customData.Add(new KeyValuePair<string, object>(base.ToCountKey(operationKey), this.Count));
			customData.Add(new KeyValuePair<string, object>(this.ToAverageKey(operationKey), this.CumulativeAverage));
		}

		internal override void AddCall(float value = 0f, int count = 1)
		{
			if (count < 1)
			{
				Guid? localId = SingleContext.Singleton.LocalId;
				ExTraceGlobals.ActivityContextTracer.TraceDebug<Guid?, int>((long)localId.GetHashCode(), "OperationStatistics.AddCall - failed to update the Average for ActivityId {0}, count was less than 1: {1}.", (localId != null) ? localId : new Guid?(Guid.Empty), count);
				return;
			}
			Interlocked.Add(ref this.count, count);
			double num = 0.0;
			double num2;
			do
			{
				num2 = num;
				double value2 = num2 + (double)value;
				num = Interlocked.CompareExchange(ref this.total, value2, num2);
			}
			while (num != num2);
		}

		internal override void Merge(OperationStatistics s2)
		{
			AverageOperationStatistics averageOperationStatistics = s2 as AverageOperationStatistics;
			if (averageOperationStatistics != null)
			{
				float value = averageOperationStatistics.CumulativeAverage * (float)averageOperationStatistics.Count;
				this.AddCall(value, averageOperationStatistics.Count);
			}
		}

		private string ToAverageKey(OperationKey operationKey)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			stringBuilder.Append(DisplayNameAttribute.GetEnumName(operationKey.ActivityOperationType));
			stringBuilder.Append(".AL[");
			if (!string.IsNullOrWhiteSpace(operationKey.Instance))
			{
				base.AppendValidChars(stringBuilder, operationKey.Instance);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private double total;

		private int count;
	}
}

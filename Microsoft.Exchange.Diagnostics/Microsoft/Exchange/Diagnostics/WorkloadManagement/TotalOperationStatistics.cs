using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class TotalOperationStatistics : OperationStatistics
	{
		internal TotalOperationStatistics()
		{
		}

		public double Total
		{
			get
			{
				return this.total;
			}
		}

		internal override void AddCall(float value = 0f, int count = 1)
		{
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
			TotalOperationStatistics totalOperationStatistics = s2 as TotalOperationStatistics;
			if (totalOperationStatistics != null)
			{
				this.total += totalOperationStatistics.total;
			}
		}

		internal override void AppendStatistics(OperationKey operationKey, List<KeyValuePair<string, object>> customData)
		{
			customData.Add(new KeyValuePair<string, object>(this.ToTotalKey(operationKey), this.Total));
		}

		private string ToTotalKey(OperationKey operationKey)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			stringBuilder.Append(DisplayNameAttribute.GetEnumName(operationKey.ActivityOperationType));
			stringBuilder.Append(".T[");
			if (!string.IsNullOrWhiteSpace(operationKey.Instance))
			{
				base.AppendValidChars(stringBuilder, operationKey.Instance);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private double total;
	}
}

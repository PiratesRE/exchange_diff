using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal abstract class OperationStatistics
	{
		public static OperationStatistics GetInstance(OperationKey operationKey)
		{
			switch (operationKey.ActivityOperationType)
			{
			case ActivityOperationType.StoreCall:
			case ActivityOperationType.StoreCpu:
			case ActivityOperationType.UserDelay:
			case ActivityOperationType.ResourceDelay:
			case ActivityOperationType.CustomCpu:
			case ActivityOperationType.MailboxLogBytes:
			case ActivityOperationType.MailboxMessagesCreated:
			case ActivityOperationType.BudgetUsed:
			case ActivityOperationType.RpcLatency:
			case ActivityOperationType.MapiLatency:
				return new TotalOperationStatistics();
			case ActivityOperationType.OverBudget:
			case ActivityOperationType.ResourceBlocked:
			case ActivityOperationType.RpcCount:
			case ActivityOperationType.Rop:
			case ActivityOperationType.MapiCount:
				return new CountOperationStatistics();
			}
			return new AverageOperationStatistics();
		}

		internal abstract void AddCall(float value = 0f, int count = 1);

		internal abstract void Merge(OperationStatistics s2);

		internal abstract void AppendStatistics(OperationKey operationKey, List<KeyValuePair<string, object>> customData);

		protected void AppendValidChars(StringBuilder dest, string src)
		{
			foreach (char c in src)
			{
				if (SpecialCharacters.IsValidKeyChar(c))
				{
					dest.Append(c);
				}
				else
				{
					dest.Append('-');
				}
			}
		}

		protected string ToCountKey(OperationKey operationKey)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			stringBuilder.Append(DisplayNameAttribute.GetEnumName(operationKey.ActivityOperationType));
			stringBuilder.Append(".C[");
			if (!string.IsNullOrWhiteSpace(operationKey.Instance))
			{
				this.AppendValidChars(stringBuilder, operationKey.Instance);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}

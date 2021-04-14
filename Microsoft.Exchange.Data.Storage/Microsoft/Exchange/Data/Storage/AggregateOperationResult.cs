using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregateOperationResult
	{
		public AggregateOperationResult(OperationResult operationResult, GroupOperationResult[] groupOperationResults)
		{
			EnumValidator.ThrowIfInvalid<OperationResult>(operationResult, "operationResult");
			this.OperationResult = operationResult;
			this.GroupOperationResults = groupOperationResults;
		}

		public static AggregateOperationResult Merge(AggregateOperationResult first, AggregateOperationResult second)
		{
			OperationResult operationResult;
			if (first.OperationResult == OperationResult.Succeeded && second.OperationResult == OperationResult.Succeeded)
			{
				operationResult = OperationResult.Succeeded;
			}
			else if (first.OperationResult == OperationResult.Failed && second.OperationResult == OperationResult.Failed)
			{
				operationResult = OperationResult.Failed;
			}
			else
			{
				operationResult = OperationResult.PartiallySucceeded;
			}
			GroupOperationResult[] array = new GroupOperationResult[first.GroupOperationResults.Length + second.GroupOperationResults.Length];
			first.GroupOperationResults.Concat(second.GroupOperationResults).CopyTo(array, 0);
			return new AggregateOperationResult(operationResult, array);
		}

		public readonly OperationResult OperationResult;

		public readonly GroupOperationResult[] GroupOperationResults;
	}
}

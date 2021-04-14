using System;

namespace Microsoft.Exchange.Migration
{
	internal struct JobItemOperationResult
	{
		public int NumItemsProcessed { get; set; }

		public int NumItemsSuccessful { get; set; }

		public int NumItemsTransitioned { get; set; }

		public static JobItemOperationResult operator +(JobItemOperationResult value1, JobItemOperationResult value2)
		{
			return new JobItemOperationResult
			{
				NumItemsProcessed = value1.NumItemsProcessed + value2.NumItemsProcessed,
				NumItemsSuccessful = value1.NumItemsSuccessful + value2.NumItemsSuccessful,
				NumItemsTransitioned = value1.NumItemsTransitioned + value2.NumItemsTransitioned
			};
		}

		public static JobItemOperationResult operator -(JobItemOperationResult value1, JobItemOperationResult value2)
		{
			return new JobItemOperationResult
			{
				NumItemsProcessed = value1.NumItemsProcessed - value2.NumItemsProcessed,
				NumItemsSuccessful = value1.NumItemsSuccessful - value2.NumItemsSuccessful,
				NumItemsTransitioned = value1.NumItemsTransitioned - value2.NumItemsTransitioned
			};
		}
	}
}

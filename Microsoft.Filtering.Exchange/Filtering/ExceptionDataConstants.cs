using System;

namespace Microsoft.Filtering
{
	public static class ExceptionDataConstants
	{
		public class Key
		{
			public const string RetryCount = "RetryCount";

			public const string MinSecondsBetweenRetries = "MinSecondsBetweenRetries";
		}

		public class RetryCount
		{
			public const int Infinite = -1;

			public const int None = 0;
		}
	}
}

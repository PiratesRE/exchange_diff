using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal class DocumentFailureDescription
	{
		private DocumentFailureDescription()
		{
		}

		public ComponentException Exception { get; private set; }

		public int FailureCode { get; private set; }

		public bool IsWarning { get; private set; }

		public bool IsPermanent { get; private set; }

		public string AdditionalInfo { get; private set; }

		public ExDateTime RetryTime { get; private set; }

		public static DocumentFailureDescription CreatePermanentError(ComponentException ex, int errorCode, string additionalInfo)
		{
			Util.ThrowOnNullArgument(ex, "ex");
			return new DocumentFailureDescription
			{
				Exception = ex,
				IsWarning = false,
				IsPermanent = true,
				FailureCode = errorCode,
				AdditionalInfo = additionalInfo,
				RetryTime = ExDateTime.MaxValue
			};
		}

		public static DocumentFailureDescription CreateTransientError(ComponentException ex, TimeSpan retryDelay)
		{
			Util.ThrowOnNullArgument(ex, "ex");
			return new DocumentFailureDescription
			{
				Exception = ex,
				IsWarning = false,
				IsPermanent = false,
				FailureCode = 0,
				AdditionalInfo = ex.ToString(),
				RetryTime = ExDateTime.UtcNow + retryDelay
			};
		}

		public static DocumentFailureDescription CreatePermanentWarning(ComponentException ex, int errorCode, string additionalInfo)
		{
			Util.ThrowOnNullArgument(ex, "ex");
			return new DocumentFailureDescription
			{
				Exception = ex,
				IsWarning = true,
				IsPermanent = true,
				FailureCode = errorCode,
				AdditionalInfo = additionalInfo,
				RetryTime = ExDateTime.MaxValue
			};
		}

		public static DocumentFailureDescription CreateTransientWarning(ComponentException ex, TimeSpan retryDelay)
		{
			Util.ThrowOnNullArgument(ex, "ex");
			return new DocumentFailureDescription
			{
				Exception = ex,
				IsWarning = true,
				IsPermanent = false,
				FailureCode = 0,
				AdditionalInfo = ex.ToString(),
				RetryTime = ExDateTime.UtcNow + retryDelay
			};
		}
	}
}

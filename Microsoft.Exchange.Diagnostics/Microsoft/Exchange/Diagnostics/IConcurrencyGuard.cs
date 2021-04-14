using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IConcurrencyGuard
	{
		string GuardName { get; }

		int MaxConcurrency { get; }

		long GetCurrentValue();

		long GetCurrentValue(string bucketName);

		long Increment(object stateObject = null);

		long Increment(string bucketName, object stateObject = null);

		long Decrement(object stateObject = null);

		long Decrement(string bucketName, object stateObject = null);
	}
}

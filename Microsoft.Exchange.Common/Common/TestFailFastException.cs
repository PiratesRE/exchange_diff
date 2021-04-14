using System;

namespace Microsoft.Exchange.Common
{
	public class TestFailFastException : Exception
	{
		internal TestFailFastException(string message) : base(message)
		{
		}
	}
}

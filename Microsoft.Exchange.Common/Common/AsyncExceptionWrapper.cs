using System;

namespace Microsoft.Exchange.Common
{
	public class AsyncExceptionWrapper : Exception
	{
		public AsyncExceptionWrapper(Exception innerExceptions) : base(CommonStrings.AsyncExceptionMessage(innerExceptions.Message), innerExceptions)
		{
		}
	}
}

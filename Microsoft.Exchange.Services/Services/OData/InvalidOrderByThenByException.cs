using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class InvalidOrderByThenByException : InvalidOrderByException
	{
		public InvalidOrderByThenByException() : base(CoreResources.ErrorInvalidOrderbyThenby)
		{
		}
	}
}

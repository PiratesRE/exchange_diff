using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal class DuplicateKeyException : ArgumentException
	{
		public DuplicateKeyException() : base("Cannot add a duplicate key.  Use Insert instead")
		{
		}
	}
}

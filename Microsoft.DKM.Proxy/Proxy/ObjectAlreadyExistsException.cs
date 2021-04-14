using System;

namespace Microsoft.Exchange.Security.Dkm.Proxy
{
	internal class ObjectAlreadyExistsException : Exception
	{
		public ObjectAlreadyExistsException()
		{
		}

		public ObjectAlreadyExistsException(string message) : base(message)
		{
		}
	}
}

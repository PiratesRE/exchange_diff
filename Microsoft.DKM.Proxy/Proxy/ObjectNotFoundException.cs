using System;

namespace Microsoft.Exchange.Security.Dkm.Proxy
{
	internal class ObjectNotFoundException : Exception
	{
		public ObjectNotFoundException()
		{
		}

		public ObjectNotFoundException(string message) : base(message)
		{
		}
	}
}

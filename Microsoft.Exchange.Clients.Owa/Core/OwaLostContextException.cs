using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaLostContextException : OwaTransientException
	{
		public OwaLostContextException() : base(null)
		{
		}

		public OwaLostContextException(string message) : base(message)
		{
		}
	}
}

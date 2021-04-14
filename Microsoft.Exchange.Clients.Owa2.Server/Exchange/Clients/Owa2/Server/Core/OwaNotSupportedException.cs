using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaNotSupportedException : OwaPermanentException
	{
		public OwaNotSupportedException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaNotSupportedException(string message) : base(message)
		{
		}
	}
}

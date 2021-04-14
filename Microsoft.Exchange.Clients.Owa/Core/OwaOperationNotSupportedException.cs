using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaOperationNotSupportedException : OwaPermanentException
	{
		public OwaOperationNotSupportedException(string message) : base(message)
		{
		}
	}
}

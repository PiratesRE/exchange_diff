using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaOperationNotSupportedException : OwaPermanentException
	{
		public OwaOperationNotSupportedException(string message) : base(message)
		{
		}
	}
}

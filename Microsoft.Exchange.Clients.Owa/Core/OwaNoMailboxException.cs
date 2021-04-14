using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaNoMailboxException : OwaPermanentException
	{
		public OwaNoMailboxException(Exception innerException) : base(null, innerException)
		{
		}
	}
}

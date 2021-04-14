using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaMethodArgumentException : OwaPermanentException
	{
		public OwaMethodArgumentException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

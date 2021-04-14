using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaSerializationException : OwaPermanentException
	{
		public OwaSerializationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

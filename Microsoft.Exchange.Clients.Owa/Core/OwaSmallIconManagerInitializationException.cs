using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaSmallIconManagerInitializationException : OwaPermanentException
	{
		public OwaSmallIconManagerInitializationException(string message) : base(message)
		{
		}
	}
}

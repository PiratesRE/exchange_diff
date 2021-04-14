using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaForbiddenRequestException : OwaPermanentException
	{
		public OwaForbiddenRequestException(string message) : base(message)
		{
		}
	}
}

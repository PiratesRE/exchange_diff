using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaCannotEditIrmDraftException : OwaPermanentException
	{
		internal OwaCannotEditIrmDraftException(string message) : base(message)
		{
		}
	}
}

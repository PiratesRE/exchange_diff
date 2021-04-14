using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaNeedsSMimeControlToEditDraftException : OwaPermanentException
	{
		internal OwaNeedsSMimeControlToEditDraftException(string message) : base(message)
		{
		}
	}
}

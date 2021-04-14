using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaUnsupportedConversationItemException : OwaPermanentException
	{
		public OwaUnsupportedConversationItemException() : base(null)
		{
		}
	}
}

using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaURLIsOutOfDateException : OwaPermanentException
	{
		public OwaURLIsOutOfDateException() : base(null)
		{
		}
	}
}

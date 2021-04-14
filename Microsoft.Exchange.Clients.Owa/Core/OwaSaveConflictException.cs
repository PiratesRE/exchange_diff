using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaSaveConflictException : OwaPermanentException
	{
		public OwaSaveConflictException(string message, object thisObject) : base(message, null, thisObject)
		{
		}
	}
}

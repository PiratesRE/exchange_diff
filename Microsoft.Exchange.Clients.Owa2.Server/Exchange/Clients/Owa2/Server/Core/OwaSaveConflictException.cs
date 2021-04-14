using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaSaveConflictException : OwaPermanentException
	{
		public OwaSaveConflictException(string message, object thisObject) : base(message, null, thisObject)
		{
		}
	}
}

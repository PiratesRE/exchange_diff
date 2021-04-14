using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaNoReplicaException : OwaPermanentException
	{
		internal OwaNoReplicaException() : base(null)
		{
		}
	}
}

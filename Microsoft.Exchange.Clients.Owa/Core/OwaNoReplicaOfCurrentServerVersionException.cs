using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaNoReplicaOfCurrentServerVersionException : OwaPermanentException
	{
		internal OwaNoReplicaOfCurrentServerVersionException() : base(null)
		{
		}
	}
}

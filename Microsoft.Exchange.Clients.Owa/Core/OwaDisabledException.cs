using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	internal class OwaDisabledException : OwaPermanentException
	{
		public OwaDisabledException() : base(null)
		{
		}
	}
}

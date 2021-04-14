using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	internal class OwaLightDisabledException : OwaPermanentException
	{
		public OwaLightDisabledException() : base(null)
		{
		}
	}
}

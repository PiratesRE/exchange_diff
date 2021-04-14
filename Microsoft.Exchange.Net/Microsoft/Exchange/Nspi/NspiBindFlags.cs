using System;

namespace Microsoft.Exchange.Nspi
{
	[Flags]
	public enum NspiBindFlags
	{
		None = 0,
		AnonymousLogin = 32
	}
}

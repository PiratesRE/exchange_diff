using System;

namespace Microsoft.Exchange.MapiHttp
{
	public enum ServiceCode : uint
	{
		Success,
		TooBusy = 1723U,
		Unavailable = 1722U
	}
}

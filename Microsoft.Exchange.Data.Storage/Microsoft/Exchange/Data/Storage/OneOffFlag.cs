using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum OneOffFlag : uint
	{
		Simple = 0U,
		Cont = 1U,
		NewEntry = 2U,
		NoTnef = 65536U,
		DontLookup = 268435456U,
		NonTransmittable = 536870912U,
		Extended = 1073741824U,
		Unicode = 2147483648U,
		SendInternetEncodingMask = 8257536U
	}
}

using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	[Flags]
	internal enum RequiredProperty
	{
		None = 0,
		Server = 1,
		Domain = 2,
		Target = 4,
		Data = 8,
		Exception = 16
	}
}

using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[Flags]
	internal enum CalculatePreferredHomeServerFlags
	{
		None = 0,
		ReadThrough = 1
	}
}

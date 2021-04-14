using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ContentFilterTags
	{
		public const int Initialization = 0;

		public const int ScanMessage = 1;

		public const int BypassedSenders = 2;

		public const int ComInterop = 3;

		public static Guid guid = new Guid("A1FD20D2-933F-4505-A0C4-C1FBFFCB9E62");
	}
}

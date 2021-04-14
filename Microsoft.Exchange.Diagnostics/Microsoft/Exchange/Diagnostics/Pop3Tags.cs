using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Pop3Tags
	{
		public const int Server = 0;

		public const int Session = 1;

		public const int FaultInjection = 2;

		public static Guid guid = new Guid("CE267B2B-B25F-4e73-BDDA-0C0734D8019B");
	}
}

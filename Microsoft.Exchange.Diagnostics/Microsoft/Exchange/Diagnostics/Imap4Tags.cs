using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Imap4Tags
	{
		public const int Server = 0;

		public const int Session = 1;

		public const int FaultInjection = 2;

		public static Guid guid = new Guid("B338D7C6-58F5-4523-B459-E387B7C956BA");
	}
}

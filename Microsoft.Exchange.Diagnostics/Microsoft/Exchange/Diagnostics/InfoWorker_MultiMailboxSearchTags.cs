using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InfoWorker_MultiMailboxSearchTags
	{
		public const int LocalSearch = 0;

		public const int MailboxGroupGenerator = 1;

		public const int General = 2;

		public const int AutoDiscover = 3;

		public static Guid guid = new Guid("6a7f7e5b-18a1-4e29-b0c0-2514adb49e41");
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_AdminInterfaceTags
	{
		public const int AdminRpc = 0;

		public const int MailboxSignature = 1;

		public const int MultiMailboxSearch = 2;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("40a87a6b-f69b-4c8e-b8c9-1835d09acfe3");
	}
}

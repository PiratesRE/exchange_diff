using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SeparatorDataContext : DataContext
	{
		private SeparatorDataContext()
		{
		}

		public override string ToString()
		{
			return "--------";
		}

		public static readonly SeparatorDataContext Separator = new SeparatorDataContext();
	}
}

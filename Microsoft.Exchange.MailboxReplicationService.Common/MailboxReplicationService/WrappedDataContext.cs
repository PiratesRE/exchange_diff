using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WrappedDataContext : DataContext
	{
		public WrappedDataContext(string ctxString)
		{
			this.ctxString = ctxString;
		}

		public override string ToString()
		{
			return this.ctxString;
		}

		private readonly string ctxString;
	}
}

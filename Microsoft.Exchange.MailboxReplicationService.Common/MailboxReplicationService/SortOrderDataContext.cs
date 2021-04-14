using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SortOrderDataContext : DataContext
	{
		public SortOrderDataContext(SortOrderData sortOrder)
		{
			this.sortOrder = sortOrder;
		}

		public override string ToString()
		{
			return string.Format("SortOrder: {0}", (this.sortOrder != null) ? this.sortOrder.ToString() : "(null)");
		}

		private SortOrderData sortOrder;
	}
}

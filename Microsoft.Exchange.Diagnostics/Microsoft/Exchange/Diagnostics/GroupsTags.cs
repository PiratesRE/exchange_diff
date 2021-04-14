using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct GroupsTags
	{
		public const int GroupNotificationStorage = 0;

		public const int UnseenItemsReader = 1;

		public const int COWGroupMessageEscalation = 2;

		public const int COWGroupMessageWSPublishing = 3;

		public static Guid guid = new Guid("1E4EC963-CD8B-4D26-A28B-832E3EA645CA");
	}
}

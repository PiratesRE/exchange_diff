using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MigrationWorkflowServiceTags
	{
		public const int MigrationWorkflowService = 0;

		public const int MailboxLoadBalance = 1;

		public const int InjectorService = 2;

		public static Guid guid = new Guid("d58c9a14-d24d-45c5-9aac-14e2678adff8");
	}
}

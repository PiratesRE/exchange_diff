using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct FfoMigrationTags
	{
		public const int PowershellProvider = 0;

		public const int MigrationWorkflow = 1;

		public const int FaultInjection = 2;

		public static Guid guid = new Guid("C7BDFB80-A905-4da5-B7AF-B36A79AD2182");
	}
}

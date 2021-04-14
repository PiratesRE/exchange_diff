using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct DiagnosticsModulesTags
	{
		public const int ErrorLoggingModule = 0;

		public const int ClientDiagnosticsModule = 1;

		public static Guid guid = new Guid("B79CCE07-AFC0-40CA-A6AD-4FB725D5770A");
	}
}

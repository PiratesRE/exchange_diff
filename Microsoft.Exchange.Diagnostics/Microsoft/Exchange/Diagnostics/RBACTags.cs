using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct RBACTags
	{
		public const int ADConfig = 0;

		public const int AccessDenied = 1;

		public const int RunspaceConfig = 2;

		public const int AccessCheck = 3;

		public const int PublicCreationAPI = 4;

		public const int PublicInstanceAPI = 5;

		public const int IssBuilder = 6;

		public const int PublicPluginAPI = 7;

		public const int IssBuilderDetail = 8;

		public const int FaultInjection = 9;

		public static Guid guid = new Guid("96825f4e-464a-44ef-af25-a76d1d0cec77");
	}
}

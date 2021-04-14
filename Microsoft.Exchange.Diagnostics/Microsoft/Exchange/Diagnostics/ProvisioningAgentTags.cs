using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ProvisioningAgentTags
	{
		public const int Rus = 0;

		public const int AdminAuditLog = 1;

		public const int FaultInjection = 2;

		public static Guid guid = new Guid("f0fd0248-ef90-4fad-8a53-a6a21ac5528c");
	}
}

using System;

namespace Microsoft.Exchange.Setup.Parser
{
	[Flags]
	public enum SetupRoles
	{
		None = 0,
		Mailbox = 1,
		Bridgehead = 2,
		ClientAccess = 4,
		UnifiedMessaging = 8,
		Gateway = 16,
		AdminTools = 32,
		Monitoring = 64,
		CentralAdmin = 128,
		CentralAdminDatabase = 256,
		Cafe = 512,
		FrontendTransport = 1024,
		OSP = 2048,
		CentralAdminFrontEnd = 4096,
		AllRoles = 8191
	}
}

using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Flags]
	internal enum EXORole
	{
		None = 0,
		HubTransportRole = 1,
		FrontendTransportRole = 2,
		ClientAccessRole = 4,
		CafeRole = 8,
		MailboxRole = 16,
		UnifiedMessagingRole = 32
	}
}

using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	[Flags]
	public enum SetupRole
	{
		None = 0,
		AdminTools = 1,
		Mailbox = 2,
		Bridgehead = 4,
		ClientAccess = 8,
		UnifiedMessaging = 16,
		Gateway = 32,
		Cafe = 64,
		Global = 128,
		LanguagePacks = 256,
		UmLanguagePack = 512,
		FrontendTransport = 1024,
		All = 2047
	}
}

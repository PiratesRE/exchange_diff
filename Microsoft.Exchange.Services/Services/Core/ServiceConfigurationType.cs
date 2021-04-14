using System;

namespace Microsoft.Exchange.Services.Core
{
	[Flags]
	public enum ServiceConfigurationType
	{
		None = 0,
		MailTips = 1,
		UnifiedMessagingConfiguration = 2,
		ProtectionRules = 4,
		PolicyNudges = 8
	}
}

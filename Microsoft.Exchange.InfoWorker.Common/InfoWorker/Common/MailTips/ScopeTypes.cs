using System;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	[Flags]
	public enum ScopeTypes
	{
		None = 0,
		Internal = 2,
		External = 4,
		ExternalPartner = 8,
		ExternalNonPartner = 16
	}
}

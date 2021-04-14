using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum DlpPolicyTipAction
	{
		NotifyOnly = 1,
		RejectUnlessSilentOverride,
		RejectUnlessExplicitOverride,
		RejectUnlessFalsePositiveOverride,
		RejectMessage
	}
}

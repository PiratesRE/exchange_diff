using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum PolicyTipMessageConfigAction
	{
		[LocDescription(CoreStrings.IDs.PolicyTipNotifyOnly)]
		NotifyOnly,
		[LocDescription(CoreStrings.IDs.PolicyTipRejectOverride)]
		RejectOverride,
		[LocDescription(CoreStrings.IDs.PolicyTipReject)]
		Reject,
		[LocDescription(CoreStrings.IDs.PolicyTipUrl)]
		Url
	}
}

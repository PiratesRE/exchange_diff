using System;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	public enum OptInStatus
	{
		[LocDescription(Strings.IDs.OptInNotConfigured)]
		NotConfigured,
		[LocDescription(Strings.IDs.OptInRequestDisabled)]
		RequestDisabled,
		[LocDescription(Strings.IDs.OptInRequestNotifyDownload)]
		RequestNotifyDownload,
		[LocDescription(Strings.IDs.OptInRequestNotifyInstall)]
		RequestNotifyInstall,
		[LocDescription(Strings.IDs.OptInRequestScheduled)]
		RequestScheduled,
		[LocDescription(Strings.IDs.OptInConfigured)]
		Configured
	}
}

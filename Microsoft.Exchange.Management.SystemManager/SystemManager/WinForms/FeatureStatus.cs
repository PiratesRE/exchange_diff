using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public enum FeatureStatus
	{
		[LocDescription(Strings.IDs.FeatureStatusNone)]
		None,
		[LocDescription(Strings.IDs.FeatureStatusEnabled)]
		Enabled,
		[LocDescription(Strings.IDs.FeatureStatusDisabled)]
		Disabled,
		[LocDescription(Strings.IDs.FeatureStatusUnknown)]
		Unknown
	}
}

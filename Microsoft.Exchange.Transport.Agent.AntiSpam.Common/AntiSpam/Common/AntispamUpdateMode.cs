using System;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	public enum AntispamUpdateMode
	{
		[LocDescription(Strings.IDs.UpdateModeDisabled)]
		Disabled,
		[LocDescription(Strings.IDs.UpdateModeManual)]
		Manual,
		[LocDescription(Strings.IDs.UpdateModeEnabled)]
		Automatic
	}
}

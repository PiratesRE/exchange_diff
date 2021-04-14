using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum UseDagDefaultOnOff
	{
		[LocDescription(Strings.IDs.UseDagDefaultOnOffNone)]
		UseDagDefault,
		[LocDescription(Strings.IDs.UseDagDefaultOnOffOff)]
		Off,
		[LocDescription(Strings.IDs.UseDagDefaultOnOffOn)]
		On
	}
}

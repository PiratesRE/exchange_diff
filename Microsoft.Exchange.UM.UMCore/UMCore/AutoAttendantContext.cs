using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AutoAttendantContext
	{
		public AutoAttendantContext(UMAutoAttendant aa, bool isBusinessHours)
		{
			this.AutoAttendant = aa;
			this.IsBusinessHours = isBusinessHours;
		}

		public UMAutoAttendant AutoAttendant { get; set; }

		public bool IsBusinessHours { get; set; }
	}
}

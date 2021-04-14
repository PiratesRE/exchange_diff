using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AutoAttendantLocationContext
	{
		public AutoAttendantLocationContext(UMAutoAttendant aa, string locationMenuDescription)
		{
			this.AutoAttendant = aa;
			this.LocationMenuDescription = locationMenuDescription;
		}

		public UMAutoAttendant AutoAttendant { get; private set; }

		public string LocationMenuDescription { get; private set; }
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class DialInRegions : Collection<DialInRegion>
	{
		public DialInRegions()
		{
		}

		public DialInRegions(IList<DialInRegion> list) : base(list)
		{
		}
	}
}

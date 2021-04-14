using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Forefront.Reporting.Common
{
	public static class Util
	{
		public static readonly string RegionTag = DatacenterRegistry.GetForefrontRegion();
	}
}

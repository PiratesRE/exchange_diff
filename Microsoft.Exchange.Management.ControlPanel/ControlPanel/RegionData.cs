using System;
using System.Globalization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class RegionData
	{
		public RegionInfo RegionInfo { private get; set; }

		public string ID { get; set; }

		public string Name
		{
			get
			{
				if (this.RegionInfo.EnglishName == this.RegionInfo.NativeName)
				{
					return this.RegionInfo.EnglishName;
				}
				return string.Format("{0} ({1})", this.RegionInfo.NativeName, this.RegionInfo.EnglishName);
			}
		}

		public string CountryCode { get; set; }

		public string[] CarrierIDs { get; set; }
	}
}

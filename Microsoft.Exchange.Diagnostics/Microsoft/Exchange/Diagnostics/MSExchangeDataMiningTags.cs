using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MSExchangeDataMiningTags
	{
		public const int Events = 0;

		public const int General = 1;

		public const int Configuration = 2;

		public const int ConfigurationService = 3;

		public const int Scheduler = 4;

		public const int Pumper = 5;

		public const int Uploader = 6;

		public static Guid guid = new Guid("{54300D03-CEA2-43CB-9522-2F6B1CD5DAA4}");
	}
}

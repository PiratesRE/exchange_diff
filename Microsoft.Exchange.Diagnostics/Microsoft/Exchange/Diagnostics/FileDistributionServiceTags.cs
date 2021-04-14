using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct FileDistributionServiceTags
	{
		public const int CustomCommand = 0;

		public const int FileReplication = 1;

		public const int ADRequests = 2;

		public const int FaultInjection = 3;

		public static Guid guid = new Guid("0f0a52f9-4d72-460d-9928-1da8215066d4");
	}
}

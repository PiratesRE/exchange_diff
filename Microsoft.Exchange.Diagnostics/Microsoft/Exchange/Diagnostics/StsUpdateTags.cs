using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct StsUpdateTags
	{
		public const int Factory = 0;

		public const int Database = 1;

		public const int Agent = 2;

		public const int OnDownload = 3;

		public const int OnRequest = 4;

		public static Guid guid = new Guid("C5F72F2A-EF44-4286-9AB2-14D106DFB8F1");
	}
}

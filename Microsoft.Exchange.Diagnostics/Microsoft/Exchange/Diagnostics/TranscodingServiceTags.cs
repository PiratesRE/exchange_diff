using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct TranscodingServiceTags
	{
		public const int tagTranscoder = 0;

		public const int tagConverter = 1;

		public const int tagRedirectIO = 2;

		public const int tagFunction = 3;

		public const int tagProgramFlow = 4;

		public static Guid guid = new Guid("2DEAC164-DDB1-4A89-9110-8258F5018258");
	}
}

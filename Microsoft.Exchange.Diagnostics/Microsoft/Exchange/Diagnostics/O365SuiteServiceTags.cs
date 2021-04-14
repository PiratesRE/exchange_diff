using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct O365SuiteServiceTags
	{
		public const int Brief = 0;

		public const int Verbose = 1;

		public const int FaultInjection = 2;

		public const int Exception = 3;

		public static Guid guid = new Guid("AF620BE4-41C6-4931-ABD7-B83FE584538D");
	}
}

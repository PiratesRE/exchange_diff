using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct EsnTags
	{
		public const int General = 0;

		public const int Data = 1;

		public const int PreProcessor = 2;

		public const int Composer = 3;

		public const int PostProcessor = 4;

		public const int MailSender = 5;

		public const int FaultInjection = 6;

		public static Guid guid = new Guid("A0D123B0-CF78-4BCA-AAC9-F892D98199F4");
	}
}

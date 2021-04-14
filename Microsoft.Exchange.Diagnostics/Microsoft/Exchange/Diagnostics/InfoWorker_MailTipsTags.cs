using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InfoWorker_MailTipsTags
	{
		public const int GetMailTipsConfiguration = 0;

		public const int GetMailTips = 1;

		public const int GroupMetrics = 2;

		public const int FaultInjection = 3;

		public static Guid guid = new Guid("EF265C98-7258-4d64-B449-75B576D9A678");
	}
}

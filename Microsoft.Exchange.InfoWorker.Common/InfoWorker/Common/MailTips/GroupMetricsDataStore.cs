using System;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public static class GroupMetricsDataStore
	{
		public static bool RenderingDisabled
		{
			get
			{
				return Utility.RenderingDisabled;
			}
		}
	}
}

using System;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class EventUtil
	{
		public static string TruncateStringInput(string str, int maxLength = 32766)
		{
			if (str != null && str.Length > maxLength)
			{
				return string.Format("{0}{1}", StringUtil.Truncate(str, maxLength - "...".Length), "...");
			}
			return str;
		}

		private const int MaxEventStringLength = 32766;

		private const string TruncatedStringIndicator = "...";

		public const int SoftLimitMaxEventStringLength = 16383;
	}
}

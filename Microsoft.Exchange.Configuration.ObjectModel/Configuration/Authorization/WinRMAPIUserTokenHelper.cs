using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class WinRMAPIUserTokenHelper
	{
		internal static string GetIdentityForWinRMAPI(string originalIdentity, WinRMInfo winRMInfo)
		{
			if (winRMInfo == null || string.IsNullOrEmpty(winRMInfo.FomattedSessionId))
			{
				return originalIdentity;
			}
			return string.Concat(new object[]
			{
				"908C5F03-5125-46BF-8746-A47994D0DA77",
				winRMInfo.FomattedSessionId,
				',',
				originalIdentity
			});
		}

		internal static string GetOriginalIdentityForWinRMAPI(string curIdentity, out string sessionId)
		{
			string result;
			if (!WinRMAPIUserTokenHelper.ContainsSessionId(curIdentity, out sessionId, out result))
			{
				return curIdentity;
			}
			return result;
		}

		private static bool ContainsSessionId(string curIdentity, out string sessionId, out string originalIdentity)
		{
			sessionId = null;
			originalIdentity = null;
			if (string.IsNullOrWhiteSpace(curIdentity))
			{
				return false;
			}
			if (!curIdentity.StartsWith("908C5F03-5125-46BF-8746-A47994D0DA77"))
			{
				return false;
			}
			int num = curIdentity.IndexOf(',');
			if (num < 0 || num <= WinRMAPIUserTokenHelper.PrefixForWinRMAPIIdentityLength || num >= curIdentity.Length - 1)
			{
				return false;
			}
			sessionId = curIdentity.Substring(WinRMAPIUserTokenHelper.PrefixForWinRMAPIIdentityLength, num - WinRMAPIUserTokenHelper.PrefixForWinRMAPIIdentityLength);
			Guid guid;
			if (!Guid.TryParse(sessionId, out guid))
			{
				sessionId = null;
				return false;
			}
			originalIdentity = curIdentity.Substring(num + 1);
			return true;
		}

		private const char SeparatorChar = ',';

		private const string PrefixForWinRMAPIIdentity = "908C5F03-5125-46BF-8746-A47994D0DA77";

		private static readonly int PrefixForWinRMAPIIdentityLength = "908C5F03-5125-46BF-8746-A47994D0DA77".Length;
	}
}

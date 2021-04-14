using System;
using Microsoft.Exchange.Net.AAD;

namespace Microsoft.Exchange.UnifiedGroups
{
	internal static class AADClientTestHooks
	{
		public static Func<IAadClient> GraphApi_GetAadClient { get; set; }

		public static Func<bool> IsUserMemberOfGroup { get; set; }
	}
}

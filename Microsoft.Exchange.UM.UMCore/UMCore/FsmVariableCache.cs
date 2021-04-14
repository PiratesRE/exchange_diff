using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FsmVariableCache
	{
		internal static Dictionary<string, object> Instance
		{
			get
			{
				return FsmVariableCache.delegateCache;
			}
		}

		private static Dictionary<string, object> delegateCache = new Dictionary<string, object>();
	}
}

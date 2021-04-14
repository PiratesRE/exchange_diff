using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class RopSummaryResolver
	{
		public static void Add(OperationType operationType, Func<byte, string> resolver)
		{
			RopSummaryResolver.resolvers[operationType] = resolver;
		}

		public static bool ContainsKey(OperationType operationType)
		{
			return RopSummaryResolver.resolvers.ContainsKey(operationType);
		}

		public static Func<byte, string> Get(OperationType operationType)
		{
			if (RopSummaryResolver.resolvers.ContainsKey(operationType))
			{
				return RopSummaryResolver.resolvers[operationType];
			}
			return null;
		}

		private static Dictionary<OperationType, Func<byte, string>> resolvers = new Dictionary<OperationType, Func<byte, string>>(5);
	}
}

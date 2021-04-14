using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal static class HelperExtension
	{
		internal static bool IsDebugOptionsEnabled(this AmConfig config)
		{
			bool result = false;
			if (!config.IsUnknown)
			{
				result = config.DbState.GetDebugOption<bool>(null, AmDebugOptions.Enabled, false);
			}
			return result;
		}

		internal static bool IsIgnoreServerDebugOptionEnabled(this AmConfig config, AmServerName serverName)
		{
			bool result = false;
			if (config.IsDebugOptionsEnabled())
			{
				result = config.DbState.GetDebugOption<bool>(serverName, AmDebugOptions.IgnoreServerFromAutomaticActions, false);
			}
			return result;
		}

		internal static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
		{
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.YieldBatchElements(batchSize - 1);
				}
			}
			yield break;
		}

		internal static IEnumerable<T> YieldBatchElements<T>(this IEnumerator<T> source, int batchSize)
		{
			yield return source.Current;
			int i = 0;
			while (i < batchSize && source.MoveNext())
			{
				yield return source.Current;
				i++;
			}
			yield break;
		}
	}
}

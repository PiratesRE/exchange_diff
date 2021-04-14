using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExtensionMethods
	{
		internal static void DisposeIfPresent(this WorkBuffer[] workBuffers)
		{
			if (workBuffers != null)
			{
				foreach (WorkBuffer disposable in workBuffers)
				{
					Util.DisposeIfPresent(disposable);
				}
			}
		}

		internal static TimeSpan Bound(this TimeSpan timeSpan, TimeSpan minTime, TimeSpan maxTime)
		{
			TimeSpan timeSpan2 = timeSpan;
			if (timeSpan2 < minTime)
			{
				timeSpan2 = minTime;
			}
			if (timeSpan2 > maxTime)
			{
				timeSpan2 = maxTime;
			}
			return timeSpan2;
		}

		internal static PropertyTag[] GetColumns(this PropertyValue[][] rowSet)
		{
			if (rowSet == null)
			{
				return null;
			}
			if (rowSet.Length > 0)
			{
				PropertyTag[] array = new PropertyTag[rowSet[0].Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = rowSet[0][i].PropertyTag;
					for (int j = 0; j < rowSet.Length; j++)
					{
						if (!rowSet[j][i].IsError)
						{
							array[i] = rowSet[j][i].PropertyTag;
							break;
						}
					}
				}
				return array;
			}
			return Array<PropertyTag>.Empty;
		}
	}
}

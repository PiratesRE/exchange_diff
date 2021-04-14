using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public static class RoutingExtensions
	{
		public static void AddIfNotNull<T>(this IList<T> list, T objectToAdd)
		{
			if (objectToAdd != null)
			{
				list.Add(objectToAdd);
			}
		}
	}
}

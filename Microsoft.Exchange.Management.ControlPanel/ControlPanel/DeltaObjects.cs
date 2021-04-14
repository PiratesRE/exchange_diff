using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class DeltaObjects
	{
		public static Delta<T> CalculateDelta<T>(this IEnumerable<T> originalCollection, IEnumerable<T> newCollection)
		{
			Delta<T> result = default(Delta<T>);
			result.AddedObjects = new List<T>();
			if (originalCollection == newCollection)
			{
				result.RemovedObjects = new List<T>();
				result.UnchangedObjects = new List<T>(originalCollection);
				return result;
			}
			List<T> list = new List<T>(originalCollection);
			List<T> list2 = new List<T>(newCollection);
			result.RemovedObjects = new List<T>(list);
			result.UnchangedObjects = new List<T>();
			using (List<T>.Enumerator enumerator = list2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T item = enumerator.Current;
					bool flag = list.Exists((T o) => o.Equals(item));
					if (flag)
					{
						result.UnchangedObjects.Add(item);
						result.RemovedObjects.Remove(item);
					}
					else
					{
						result.AddedObjects.Add(item);
					}
				}
			}
			return result;
		}
	}
}

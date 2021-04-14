using System;
using System.Collections;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class ArrayExtension
	{
		public static WorkUnit[] DeepCopy(this WorkUnit[] workUnitArray)
		{
			WorkUnit[] array = null;
			if (workUnitArray != null)
			{
				array = new WorkUnit[workUnitArray.Length];
				for (int i = 0; i < workUnitArray.Length; i++)
				{
					array[i] = new WorkUnit(workUnitArray[i].Text, workUnitArray[i].Icon, workUnitArray[i].Target);
				}
			}
			return array;
		}

		public static bool IsEmptyCollection(this ICollection collection)
		{
			return collection == null || collection.Count == 0;
		}
	}
}

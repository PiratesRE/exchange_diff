using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ActionOrderComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			ActionBase actionBase = x as ActionBase;
			ActionBase actionBase2 = y as ActionBase;
			if (actionBase == null || actionBase2 == null)
			{
				throw new ArgumentNullException();
			}
			if (actionBase.ActionOrder == actionBase2.ActionOrder)
			{
				return 0;
			}
			if (actionBase.ActionOrder <= actionBase2.ActionOrder)
			{
				return -1;
			}
			return 1;
		}
	}
}

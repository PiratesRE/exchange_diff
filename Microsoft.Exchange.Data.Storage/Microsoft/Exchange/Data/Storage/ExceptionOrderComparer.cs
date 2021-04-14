using System;
using System.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExceptionOrderComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			if (x == null)
			{
				throw new ArgumentNullException("x");
			}
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			Condition condition = x as Condition;
			Condition condition2 = y as Condition;
			if (condition == null)
			{
				throw new ArgumentException("x is not a condition");
			}
			if (condition2 == null)
			{
				throw new ArgumentException("y is not a condition");
			}
			if (condition.ExceptionOrder == condition2.ExceptionOrder)
			{
				return 0;
			}
			if (condition.ExceptionOrder <= condition2.ExceptionOrder)
			{
				return -1;
			}
			return 1;
		}
	}
}

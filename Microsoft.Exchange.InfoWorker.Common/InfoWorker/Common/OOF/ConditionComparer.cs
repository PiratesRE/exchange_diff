using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal static class ConditionComparer
	{
		internal static bool Equals(Restriction a, Restriction b)
		{
			if (a == null || b == null)
			{
				return a == b;
			}
			if (a.GetType() != b.GetType())
			{
				return false;
			}
			if (a is Restriction.PropertyRestriction)
			{
				return ConditionComparer.Equals((Restriction.PropertyRestriction)a, (Restriction.PropertyRestriction)b);
			}
			if (a is Restriction.ContentRestriction)
			{
				return ConditionComparer.Equals((Restriction.ContentRestriction)a, (Restriction.ContentRestriction)b);
			}
			return a is Restriction.AndRestriction && ConditionComparer.Equals((Restriction.AndRestriction)a, (Restriction.AndRestriction)b);
		}

		private static bool Equals(Restriction.PropertyRestriction a, Restriction.PropertyRestriction b)
		{
			return a.Op == b.Op && a.PropTag == b.PropTag && a.PropValue.Equals(b.PropValue);
		}

		private static bool Equals(Restriction.AndRestriction a, Restriction.AndRestriction b)
		{
			return ConditionComparer.Equals(a.Restrictions, b.Restrictions);
		}

		private static bool Equals(Restriction.OrRestriction a, Restriction.OrRestriction b)
		{
			return ConditionComparer.Equals(a.Restrictions, b.Restrictions);
		}

		private static bool Equals(Restriction[] aArray, Restriction[] bArray)
		{
			foreach (Restriction condition in aArray)
			{
				if (!ConditionComparer.Contains(condition, bArray))
				{
					return false;
				}
			}
			foreach (Restriction condition2 in bArray)
			{
				if (!ConditionComparer.Contains(condition2, aArray))
				{
					return false;
				}
			}
			return true;
		}

		private static bool Contains(Restriction condition, Restriction[] conditionArray)
		{
			foreach (Restriction b in conditionArray)
			{
				if (ConditionComparer.Equals(condition, b))
				{
					return true;
				}
			}
			return false;
		}
	}
}

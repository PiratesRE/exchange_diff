using System;
using System.Collections;

namespace System.Security.AccessControl
{
	public sealed class AuthorizationRuleCollection : ReadOnlyCollectionBase
	{
		public void AddRule(AuthorizationRule rule)
		{
			base.InnerList.Add(rule);
		}

		public void CopyTo(AuthorizationRule[] rules, int index)
		{
			((ICollection)this).CopyTo(rules, index);
		}

		public AuthorizationRule this[int index]
		{
			get
			{
				return base.InnerList[index] as AuthorizationRule;
			}
		}
	}
}

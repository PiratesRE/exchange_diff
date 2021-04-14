using System;
using System.Reflection;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class BifurcationInfoPredicate : TransportRulePredicate
	{
		internal static bool TryCreatePredicateFromBifInfo(TypeMapping[] mappings, RuleBifurcationInfo bifInfo1, RuleBifurcationInfo bifInfo2, out TransportRulePredicate predicate, out bool twoBifInfoConverted)
		{
			foreach (TypeMapping typeMapping in mappings)
			{
				if (typeMapping.Type.IsSubclassOf(typeof(BifurcationInfoPredicate)))
				{
					MethodInfo method = typeMapping.Type.GetMethod("CreatePredicateFromBifInfo", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(RuleBifurcationInfo)
					}, null);
					TransportRulePredicate transportRulePredicate;
					if (method != null)
					{
						transportRulePredicate = (TransportRulePredicate)method.Invoke(null, new object[]
						{
							bifInfo1
						});
						if (transportRulePredicate != null)
						{
							twoBifInfoConverted = false;
							predicate = transportRulePredicate;
							predicate.Initialize(mappings);
							return true;
						}
					}
					if (bifInfo2 == null)
					{
						goto IL_116;
					}
					method = typeMapping.Type.GetMethod("CreatePredicateFromBifInfo", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[]
					{
						typeof(RuleBifurcationInfo),
						typeof(RuleBifurcationInfo)
					}, null);
					if (!(method != null))
					{
						goto IL_116;
					}
					transportRulePredicate = (TransportRulePredicate)method.Invoke(null, new object[]
					{
						bifInfo1,
						bifInfo2
					});
					if (transportRulePredicate == null)
					{
						goto IL_116;
					}
					twoBifInfoConverted = true;
					predicate = transportRulePredicate;
					predicate.Initialize(mappings);
					return true;
				}
				IL_116:;
			}
			predicate = null;
			twoBifInfoConverted = false;
			return false;
		}

		internal abstract RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo);

		internal sealed override Condition ToInternalCondition()
		{
			throw new NotSupportedException();
		}
	}
}

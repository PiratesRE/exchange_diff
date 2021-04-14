using System;

namespace System.Security.Policy
{
	internal interface IReportMatchMembershipCondition : IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		bool Check(Evidence evidence, out object usedEvidence);
	}
}

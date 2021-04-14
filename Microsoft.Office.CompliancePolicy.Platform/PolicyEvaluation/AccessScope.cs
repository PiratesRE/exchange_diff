using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	[Flags]
	public enum AccessScope
	{
		[EnumMember]
		Internal = 1,
		[EnumMember]
		External = 2
	}
}

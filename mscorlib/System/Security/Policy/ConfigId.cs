using System;

namespace System.Security.Policy
{
	[Serializable]
	internal enum ConfigId
	{
		None,
		MachinePolicyLevel,
		UserPolicyLevel,
		EnterprisePolicyLevel
	}
}

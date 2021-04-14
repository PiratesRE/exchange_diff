using System;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public interface IMembershipCondition : ISecurityEncodable, ISecurityPolicyEncodable
	{
		bool Check(Evidence evidence);

		IMembershipCondition Copy();

		string ToString();

		bool Equals(object obj);
	}
}

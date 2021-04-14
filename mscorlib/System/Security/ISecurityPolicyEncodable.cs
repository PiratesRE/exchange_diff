using System;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace System.Security
{
	[ComVisible(true)]
	public interface ISecurityPolicyEncodable
	{
		SecurityElement ToXml(PolicyLevel level);

		void FromXml(SecurityElement e, PolicyLevel level);
	}
}

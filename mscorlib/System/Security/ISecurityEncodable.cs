using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	public interface ISecurityEncodable
	{
		SecurityElement ToXml();

		void FromXml(SecurityElement e);
	}
}

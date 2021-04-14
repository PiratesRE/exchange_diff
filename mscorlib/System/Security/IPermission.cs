using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	public interface IPermission : ISecurityEncodable
	{
		IPermission Copy();

		IPermission Intersect(IPermission target);

		IPermission Union(IPermission target);

		bool IsSubsetOf(IPermission target);

		void Demand();
	}
}

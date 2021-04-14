using System;
using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	public interface IStackWalk
	{
		void Assert();

		void Demand();

		void Deny();

		void PermitOnly();
	}
}

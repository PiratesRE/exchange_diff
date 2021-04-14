using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Lifetime
{
	[ComVisible(true)]
	public interface ISponsor
	{
		[SecurityCritical]
		TimeSpan Renewal(ILease lease);
	}
}

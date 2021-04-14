using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public interface IDynamicProperty
	{
		string Name { [SecurityCritical] get; }
	}
}

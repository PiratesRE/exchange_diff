using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Activation
{
	[ComVisible(true)]
	public interface IActivator
	{
		IActivator NextActivator { [SecurityCritical] get; [SecurityCritical] set; }

		[SecurityCritical]
		IConstructionReturnMessage Activate(IConstructionCallMessage msg);

		ActivatorLevel Level { [SecurityCritical] get; }
	}
}

using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Security;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public interface IContextPropertyActivator
	{
		[SecurityCritical]
		bool IsOKToActivate(IConstructionCallMessage msg);

		[SecurityCritical]
		void CollectFromClientContext(IConstructionCallMessage msg);

		[SecurityCritical]
		bool DeliverClientContextToServerContext(IConstructionCallMessage msg);

		[SecurityCritical]
		void CollectFromServerContext(IConstructionReturnMessage msg);

		[SecurityCritical]
		bool DeliverServerContextToClientContext(IConstructionReturnMessage msg);
	}
}

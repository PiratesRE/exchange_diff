using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace System.Runtime.Remoting.Activation
{
	[ComVisible(true)]
	public interface IConstructionCallMessage : IMethodCallMessage, IMethodMessage, IMessage
	{
		IActivator Activator { [SecurityCritical] get; [SecurityCritical] set; }

		object[] CallSiteActivationAttributes { [SecurityCritical] get; }

		string ActivationTypeName { [SecurityCritical] get; }

		Type ActivationType { [SecurityCritical] get; }

		IList ContextProperties { [SecurityCritical] get; }
	}
}

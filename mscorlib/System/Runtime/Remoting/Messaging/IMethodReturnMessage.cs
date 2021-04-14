using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public interface IMethodReturnMessage : IMethodMessage, IMessage
	{
		int OutArgCount { [SecurityCritical] get; }

		[SecurityCritical]
		string GetOutArgName(int index);

		[SecurityCritical]
		object GetOutArg(int argNum);

		object[] OutArgs { [SecurityCritical] get; }

		Exception Exception { [SecurityCritical] get; }

		object ReturnValue { [SecurityCritical] get; }
	}
}

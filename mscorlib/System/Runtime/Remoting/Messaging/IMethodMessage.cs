using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public interface IMethodMessage : IMessage
	{
		string Uri { [SecurityCritical] get; }

		string MethodName { [SecurityCritical] get; }

		string TypeName { [SecurityCritical] get; }

		object MethodSignature { [SecurityCritical] get; }

		int ArgCount { [SecurityCritical] get; }

		[SecurityCritical]
		string GetArgName(int index);

		[SecurityCritical]
		object GetArg(int argNum);

		object[] Args { [SecurityCritical] get; }

		bool HasVarArgs { [SecurityCritical] get; }

		LogicalCallContext LogicalCallContext { [SecurityCritical] get; }

		MethodBase MethodBase { [SecurityCritical] get; }
	}
}

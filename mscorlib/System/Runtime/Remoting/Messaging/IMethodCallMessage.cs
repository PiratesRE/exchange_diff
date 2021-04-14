using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public interface IMethodCallMessage : IMethodMessage, IMessage
	{
		int InArgCount { [SecurityCritical] get; }

		[SecurityCritical]
		string GetInArgName(int index);

		[SecurityCritical]
		object GetInArg(int argNum);

		object[] InArgs { [SecurityCritical] get; }
	}
}

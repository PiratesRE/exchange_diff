using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IIdentity
	{
		[__DynamicallyInvokable]
		string Name { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		string AuthenticationType { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool IsAuthenticated { [__DynamicallyInvokable] get; }
	}
}

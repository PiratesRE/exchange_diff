using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IPrincipal
	{
		[__DynamicallyInvokable]
		IIdentity Identity { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool IsInRole(string role);
	}
}

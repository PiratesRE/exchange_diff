using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("8AD3FC86-AFD3-477a-8FD5-146C291195BA")]
	[ComImport]
	internal interface IWindowClassEntry
	{
		WindowClassEntry AllData { [SecurityCritical] get; }

		string ClassName { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string HostDll { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		bool fVersioned { [SecurityCritical] get; }
	}
}

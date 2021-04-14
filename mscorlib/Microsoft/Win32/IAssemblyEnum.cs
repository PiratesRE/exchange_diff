using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
	[ComImport]
	internal interface IAssemblyEnum
	{
		[PreserveSig]
		int GetNextAssembly(out IApplicationContext ppAppCtx, out IAssemblyName ppName, uint dwFlags);

		[PreserveSig]
		int Reset();

		[PreserveSig]
		int Clone(out IAssemblyEnum ppEnum);
	}
}

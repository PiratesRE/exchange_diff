using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.Exchange.BITS
{
	[Guid("4991D34B-80A1-4291-83B6-3328366B9097")]
	[ClassInterface(ClassInterfaceType.None)]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	[ComImport]
	internal class BackgroundCopyManager
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern BackgroundCopyManager();
	}
}

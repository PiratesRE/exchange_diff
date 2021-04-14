using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	internal struct IStore_BindingResult_BoundVersion
	{
		[MarshalAs(UnmanagedType.U2)]
		public ushort Revision;

		[MarshalAs(UnmanagedType.U2)]
		public ushort Build;

		[MarshalAs(UnmanagedType.U2)]
		public ushort Minor;

		[MarshalAs(UnmanagedType.U2)]
		public ushort Major;
	}
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System.Reflection
{
	internal struct SecurityContextFrame
	{
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Push(RuntimeAssembly assembly);

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Pop();

		private IntPtr m_GSCookie;

		private IntPtr __VFN_table;

		private IntPtr m_Next;

		private IntPtr m_Assembly;
	}
}

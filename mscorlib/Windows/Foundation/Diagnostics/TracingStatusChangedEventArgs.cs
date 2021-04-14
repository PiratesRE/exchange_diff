using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.Foundation.Diagnostics
{
	[Guid("410B7711-FF3B-477F-9C9A-D2EFDA302DC3")]
	[ComImport]
	internal sealed class TracingStatusChangedEventArgs : ITracingStatusChangedEventArgs
	{
		public extern bool Enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public extern CausalityTraceLevel TraceLevel { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern TracingStatusChangedEventArgs();
	}
}

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.BITS
{
	[Guid("97EA99C7-0186-4AD4-8DF9-C5B4E0ED6B22")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IBackgroundCopyCallback
	{
		void JobTransferred([MarshalAs(UnmanagedType.Interface)] [In] IBackgroundCopyJob pJob);

		void JobError([MarshalAs(UnmanagedType.Interface)] [In] IBackgroundCopyJob pJob, [MarshalAs(UnmanagedType.Interface)] [In] IBackgroundCopyError pError);

		void JobModification([MarshalAs(UnmanagedType.Interface)] [In] IBackgroundCopyJob pJob, [In] uint dwReserved);
	}
}

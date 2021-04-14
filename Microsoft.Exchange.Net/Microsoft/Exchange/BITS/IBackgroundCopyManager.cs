using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.BITS
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("5CE34C0D-0DC9-4C1F-897C-DAA1B78CEE7C")]
	[ComImport]
	internal interface IBackgroundCopyManager
	{
		void CreateJob([MarshalAs(UnmanagedType.LPWStr)] string DisplayName, BG_JOB_TYPE Type, out Guid pJobId, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob ppJob);

		void GetJob(ref Guid jobID, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob ppJob);

		void EnumJobs(uint dwFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyJobs ppenum);

		void GetErrorDescription([MarshalAs(UnmanagedType.Error)] int hResult, uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pErrorDescription);
	}
}

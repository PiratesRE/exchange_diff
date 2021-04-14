using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Audio
{
	internal class ACMNativeMethods
	{
		[DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr OpenDriver([In] string szDriverName, [In] string szSectionName, [In] int lParam2);

		[DllImport("winmm.dll", SetLastError = true)]
		internal static extern IntPtr GetDriverModuleHandle([In] IntPtr hdrvr);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr GetProcAddress([In] IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] [In] string lpProcName);

		[DllImport("msacm32.dll", EntryPoint = "acmDriverAdd")]
		internal static extern int AcmDriverAdd(out IntPtr phadid, [In] IntPtr hModule, [In] IntPtr lParam, [In] int dwPriority, [In] int fdwAdd);

		[DllImport("msacm32.dll", EntryPoint = "acmDriverOpen")]
		internal static extern int AcmDriverOpen(out ACMNativeMethods.AcmInstanceHandle phad, [In] IntPtr hadid, [In] int fdwOpen);

		[DllImport("msacm32.dll", EntryPoint = "acmDriverClose")]
		internal static extern int AcmDriverClose([In] IntPtr phad, [In] int fdwClose);

		[DllImport("msacm32.dll", EntryPoint = "acmStreamOpen")]
		internal static extern int AcmStreamOpen(out ACMNativeMethods.SafeStreamHandle phStream, ACMNativeMethods.AcmInstanceHandle hStream, WaveFormat pwfxSrc, WaveFormat pwfxDst, IntPtr pwfltr, IntPtr dwCallback, IntPtr dwInstance, int fdwOpen);

		[DllImport("msacm32.dll", EntryPoint = "acmStreamSize")]
		internal static extern int AcmStreamSize(ACMNativeMethods.SafeStreamHandle hStream, int cbInput, out int cbOutput, int fdwSize);

		[DllImport("msacm32.dll", EntryPoint = "acmStreamPrepareHeader")]
		internal static extern int AcmStreamPrepareHeader(ACMNativeMethods.SafeStreamHandle hStream, [In] [Out] ref ACMNativeMethods.ACMSTREAMHEADER streamHeader, int fdwPrepare);

		[DllImport("msacm32.dll", EntryPoint = "acmStreamConvert")]
		internal static extern int AcmStreamConvert(ACMNativeMethods.SafeStreamHandle hStream, [In] [Out] ref ACMNativeMethods.ACMSTREAMHEADER streamHeader, int fdwConvert);

		[DllImport("msacm32.dll", EntryPoint = "acmStreamUnprepareHeader")]
		internal static extern int AcmStreamUnprepareHeader(ACMNativeMethods.SafeStreamHandle hStream, [In] [Out] ref ACMNativeMethods.ACMSTREAMHEADER pash, int fdwUnprepare);

		[DllImport("msacm32.dll", EntryPoint = "acmStreamClose")]
		internal static extern int AcmStreamClose(IntPtr hStream, int fdwClose);

		internal const int ACM_STREAMSIZEF_SOURCE = 0;

		internal const int ACM_STREAMSIZEF_DESTINATION = 1;

		internal const int ACM_STREAMOPENF_QUERY = 1;

		internal const int ACM_STREAMOPENF_ASYNC = 2;

		internal const int ACM_STREAMOPENF_NONREALTIME = 4;

		internal const int ACM_STREAMCONVERTF_BLOCKALIGN = 4;

		internal const int ACM_STREAMCONVERTF_END = 32;

		internal const int ACM_STREAMCONVERTF_START = 16;

		internal const int ACMSTREAMHEADER_STATUSF_PREPARED = 131072;

		internal const int WAVE_FORMAT_GSM610 = 49;

		internal const int WAVE_FORMAT_PCM = 1;

		internal const string MP3CODEC = "l3codecp.acm";

		internal const string DRIVER_ENTRY_POINT = "DriverProc";

		private const string MSACM32 = "msacm32.dll";

		private const string WINMM = "winmm.dll";

		private const string KERNEL32 = "kernel32.dll";

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct ACMSTREAMHEADER
		{
			public void Zero()
			{
				this.cbStruct = 0;
				this.fdwStatus = 0;
				this.dwUser = IntPtr.Zero;
				this.pbSrc = IntPtr.Zero;
				this.cbSrcLength = 0;
				this.cbSrcLengthUsed = 0;
				this.dwSrcUser = IntPtr.Zero;
				this.pbDst = IntPtr.Zero;
				this.cbDstLength = 0;
				this.cbDstLengthUsed = 0;
				this.dwDstUser = IntPtr.Zero;
			}

			public int cbStruct;

			public int fdwStatus;

			public IntPtr dwUser;

			public IntPtr pbSrc;

			public int cbSrcLength;

			public int cbSrcLengthUsed;

			public IntPtr dwSrcUser;

			public IntPtr pbDst;

			public int cbDstLength;

			public int cbDstLengthUsed;

			public IntPtr dwDstUser;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
			public int[] dwReservedDriver;
		}

		internal sealed class SafeStreamHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			internal SafeStreamHandle(IntPtr preexistingHandle, bool ownsHandle) : base(ownsHandle)
			{
				base.SetHandle(preexistingHandle);
			}

			private SafeStreamHandle() : base(true)
			{
			}

			protected override bool ReleaseHandle()
			{
				return 0 == ACMNativeMethods.AcmStreamClose(this.handle, 0);
			}
		}

		internal sealed class AcmInstanceHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			internal AcmInstanceHandle(IntPtr preexistingHandle, bool ownsHandle) : base(ownsHandle)
			{
				base.SetHandle(preexistingHandle);
			}

			private AcmInstanceHandle() : base(true)
			{
			}

			protected override bool ReleaseHandle()
			{
				return 0 == ACMNativeMethods.AcmDriverClose(this.handle, 0);
			}
		}
	}
}

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	internal static class WindowsMediaNativeMethods
	{
		internal static IWMSyncReader CreateSyncReader()
		{
			IWMSyncReader result = null;
			Marshal.ThrowExceptionForHR(WindowsMediaNativeMethods.WMCreateSyncReader(IntPtr.Zero, 0U, out result));
			return result;
		}

		internal static IWMWriter CreateWriter()
		{
			IWMWriter result = null;
			Marshal.ThrowExceptionForHR(WindowsMediaNativeMethods.WMCreateWriter(IntPtr.Zero, out result));
			return result;
		}

		internal static IWMProfileManager CreateProfileManager()
		{
			IWMProfileManager result = null;
			Marshal.ThrowExceptionForHR(WindowsMediaNativeMethods.WMCreateProfileManager(out result));
			return result;
		}

		[DllImport("WMVCore.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern int WMCreateSyncReader(IntPtr pUnkCert, uint dwRights, [MarshalAs(UnmanagedType.Interface)] out IWMSyncReader ppSyncReader);

		[DllImport("WMVCore.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern int WMCreateWriter(IntPtr pUnkReserved, [MarshalAs(UnmanagedType.Interface)] out IWMWriter ppWriter);

		[DllImport("WMVCore.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern int WMCreateProfileManager([MarshalAs(UnmanagedType.Interface)] out IWMProfileManager ppProfileManager);

		private const string WMVCoreDll = "WMVCore.dll";

		private const CallingConvention WMCallingConvention = CallingConvention.StdCall;

		private const CharSet WMCharSet = CharSet.Unicode;

		internal enum WMT_VERSION
		{
			WMT_VER_4_0 = 262144,
			WMT_VER_7_0 = 458752,
			WMT_VER_8_0 = 524288,
			WMT_VER_9_0 = 589824
		}

		internal enum WMT_ATTR_DATATYPE
		{
			WMT_TYPE_DWORD,
			WMT_TYPE_STRING,
			WMT_TYPE_BINARY,
			WMT_TYPE_BOOL,
			WMT_TYPE_QWORD,
			WMT_TYPE_WORD,
			WMT_TYPE_GUID
		}

		internal struct WM_MEDIA_TYPE
		{
			internal Guid majortype;

			internal Guid subtype;

			[MarshalAs(UnmanagedType.Bool)]
			internal bool bFixedSizeSamples;

			[MarshalAs(UnmanagedType.Bool)]
			internal bool bTemporalCompression;

			internal uint lSampleSize;

			internal Guid formattype;

			internal IntPtr pUnk;

			internal uint cbFormat;

			internal IntPtr pbFormat;
		}

		internal static class MediaTypes
		{
			internal static Guid WMFORMAT_WaveFormatEx
			{
				get
				{
					return new Guid("05589F81-C356-11CE-BF01-00AA0055595A");
				}
			}

			internal static Guid WMMEDIASUBTYPE_PCM
			{
				get
				{
					return new Guid("00000001-0000-0010-8000-00AA00389B71");
				}
			}

			internal static Guid WMMEDIATYPE_Audio
			{
				get
				{
					return new Guid("73647561-0000-0010-8000-00AA00389B71");
				}
			}
		}
	}
}

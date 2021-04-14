using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Common.HA
{
	internal static class ExDbFailureItemApi
	{
		[DllImport("ExDbFailureItemApi.dll", EntryPoint = "HaPublishDbFailureItem")]
		internal static extern int PublishFailureItem([In] ref ExDbFailureItemApi.HaDbFailureItem failureItem);

		[DllImport("ExDbFailureItemApi.dll", EntryPoint = "HaPublishDbFailureItemEx")]
		internal static extern int PublishFailureItemEx([In] bool isDebug, [In] ref ExDbFailureItemApi.HaDbFailureItem failureItem);

		internal const int ERROR_SUCCESS = 0;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct HaDbIoErrorInfo
		{
			internal int CbSize;

			internal IoErrorCategory Category;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string FileName;

			internal long Offset;

			internal long Size;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct HaDbNotificationEventInfo
		{
			internal int CbSize;

			internal int EventId;

			internal int NumParameters;

			internal IntPtr Parameters;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct HaDbFailureItem
		{
			internal int CbSize;

			internal FailureNameSpace NameSpace;

			internal FailureTag Tag;

			internal Guid Guid;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string InstanceName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string ComponentName;

			internal IntPtr IoError;

			internal IntPtr NotificationEventInfo;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string Message;
		}
	}
}

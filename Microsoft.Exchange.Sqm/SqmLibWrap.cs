using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Sqm
{
	internal static class SqmLibWrap
	{
		[DllImport("sqmapi.dll")]
		public static extern void SqmAddToAverage(uint hSession, uint dwId, uint dwVal);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmAddToStreamDWord(uint hSession, uint dwId, uint cTuple, uint dwVal);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmAddToStreamString(uint hSession, uint dwId, uint cTuple, [MarshalAs(UnmanagedType.LPWStr)] string pwszVal);

		[DllImport("sqmapi.dll")]
		public static extern void SqmClearFlags(uint hSession, ref uint dwFlags);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmCreateNewId(out Guid pGuid);

		[DllImport("sqmapi.dll")]
		public static extern void SqmEndSession(uint hSession, [MarshalAs(UnmanagedType.LPWStr)] string pszPattern, uint dwMaxFilesToQueue, uint dwFlags);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmGetEnabled(uint hSession);

		[DllImport("sqmapi.dll")]
		public static extern void SqmGetFlags(uint hSession, out uint dwFlags);

		[DllImport("sqmapi.dll")]
		public static extern void SqmGetMachineId(uint hSession, out Guid guid);

		[DllImport("sqmapi.dll")]
		public static extern uint SqmGetSession([MarshalAs(UnmanagedType.LPWStr)] string pszSessionIdentifier, uint cbMaxSessionSize, uint dwFlags);

		[DllImport("sqmapi.dll")]
		public static extern System.Runtime.InteropServices.ComTypes.FILETIME SqmGetSessionStartTime(uint hSession);

		[DllImport("sqmapi.dll")]
		public static extern void SqmGetUserId(uint hSession, out Guid guid);

		[DllImport("sqmapi.dll")]
		public static extern void SqmIncrement(uint hSession, uint dwId, uint dwInc);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmIsWindowsOptedIn();

		[DllImport("sqmapi.dll")]
		public static extern bool SqmReadSharedMachineId(out Guid pGuid);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmReadSharedUserId(out Guid guid);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSet(uint hSession, uint dwId, uint dwVal);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmSetAppId(uint hSession, uint dwAppId);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetAppVersion(uint hSession, uint dwVersionHigh, uint dwVersionLow);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetBits(uint hSession, uint dwId, uint dwOrBits);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetBool(uint hSession, uint dwId, uint dwVal);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetCurrentTimeAsUploadTime([MarshalAs(UnmanagedType.LPWStr)] string pszSqmFileName);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetEnabled(uint hSession, bool fEnabled);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetFlags(uint hSession, uint dwFlags);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetIfMax(uint hSession, uint dwId, uint dwOrBits);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetIfMin(uint hSession, uint dwId, uint dwOrBits);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetMachineId(uint hSession, ref Guid pGuid);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmSetString(uint hSession, uint dwId, [MarshalAs(UnmanagedType.LPWStr)] string pwszVal);

		[DllImport("sqmapi.dll")]
		public static extern void SqmSetUserId(uint hSession, ref Guid pGuid);

		[DllImport("sqmapi.dll")]
		public static extern void SqmStartSession(uint hSession);

		[DllImport("sqmapi.dll", SetLastError = true)]
		public static extern uint SqmStartUpload([MarshalAs(UnmanagedType.LPWStr)] string szPattern, [MarshalAs(UnmanagedType.LPWStr)] string szUrl, [MarshalAs(UnmanagedType.LPWStr)] string szSecureUrl, uint dwFlags, SqmLibWrap.SqmUploadCallback pfnCallback);

		[DllImport("sqmapi.dll")]
		public static extern void SqmTimerAccumulate(uint hSession, uint dwId);

		[DllImport("sqmapi.dll")]
		public static extern void SqmTimerAddToAverage(uint hSession, uint dwId);

		[DllImport("sqmapi.dll")]
		public static extern void SqmTimerRecord(uint hSession, uint dwId);

		[DllImport("sqmapi.dll")]
		public static extern void SqmTimerStart(uint hSession, uint dwId);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmWaitForUploadComplete(uint dwTimeoutMilliseconds, uint dwFlags);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmWriteSharedMachineId(ref Guid pGuid);

		[DllImport("sqmapi.dll")]
		public static extern bool SqmWriteSharedUserId(ref Guid pGuid);

		public const uint SqmSessionCreateNew = 1U;

		public const uint SqmWaitForUploadCurrentFileInQueue = 1U;

		public const uint SqmWaitForUploadAllFilesInQueue = 2U;

		public const uint SqmUploadAllFiles = 2U;

		public const uint SqmIgnoreWindowsOptin = 4U;

		public const uint SqmOverwriteOldestFile = 2U;

		public const uint SqmReleaseSession = 8U;

		public delegate bool SqmUploadCallback(uint hr, [MarshalAs(UnmanagedType.LPWStr)] string filePath, uint dwHttpResponse);
	}
}

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PeopleStringUtils
	{
		public static string ComputeSortKey(CultureInfo culture, string value)
		{
			if (value == null)
			{
				return null;
			}
			SortKey sortKey = culture.CompareInfo.GetSortKey(value, PeopleStringUtils.StringCompareOptions);
			byte[] keyData = sortKey.KeyData;
			StringBuilder stringBuilder = new StringBuilder(200);
			int num = 0;
			while (num < keyData.Length && num < 100)
			{
				stringBuilder.Append(keyData[num].ToString("X2"));
				num++;
			}
			return stringBuilder.ToString();
		}

		public static string ComputeSortVersion(CultureInfo culture)
		{
			PeopleStringUtils.NLSVERSIONINFO nlsversioninfo;
			nlsversioninfo.dwNLSVersionInfoSize = (uint)Marshal.SizeOf(typeof(PeopleStringUtils.NLSVERSIONINFO));
			if (!PeopleStringUtils.GetNLSVersion(PeopleStringUtils.NLS_FUNCTION.COMPARE_STRING, (uint)culture.LCID, out nlsversioninfo))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			return string.Format("{0:X8},{1:X8}", nlsversioninfo.dwDefinedVersion, nlsversioninfo.dwNLSVersion);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetNLSVersion(PeopleStringUtils.NLS_FUNCTION function, uint locale, out PeopleStringUtils.NLSVERSIONINFO lpVersionInformation);

		public static readonly CompareOptions StringCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

		internal enum NLS_FUNCTION : uint
		{
			COMPARE_STRING = 1U
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct NLSVERSIONINFO
		{
			public uint dwNLSVersionInfoSize;

			public uint dwNLSVersion;

			public uint dwDefinedVersion;
		}
	}
}

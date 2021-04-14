using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.Isam.Esent.Interop
{
	internal static class LibraryHelpers
	{
		public static Encoding EncodingASCII
		{
			get
			{
				return Encoding.ASCII;
			}
		}

		public static Encoding NewEncodingASCII
		{
			get
			{
				return new ASCIIEncoding();
			}
		}

		public static CultureInfo CreateCultureInfoByLcid(int lcid)
		{
			return new CultureInfo(lcid);
		}

		public static IntPtr MarshalAllocHGlobal(int size)
		{
			return Marshal.AllocHGlobal(size);
		}

		public static void MarshalFreeHGlobal(IntPtr buffer)
		{
			Marshal.FreeHGlobal(buffer);
		}

		public static IntPtr MarshalStringToHGlobalUni(string managedString)
		{
			return Marshal.StringToHGlobalUni(managedString);
		}

		public static int GetCurrentManagedThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		public static void ThreadResetAbort()
		{
			Thread.ResetAbort();
		}

		public static DateTime FromOADate(double d)
		{
			return DateTime.FromOADate(d);
		}

		public static readonly char DirectorySeparatorChar = '\\';

		public static readonly char AltDirectorySeparatorChar = '/';
	}
}

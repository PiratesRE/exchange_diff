using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa.Core.Internal
{
	internal static class SafeNativeMethods
	{
		[DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint NetUserChangePassword(string domainname, string username, IntPtr oldpassword, IntPtr newpassword);

		[DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int MsiOpenDatabase(string szDatabasePath, int szPersist, out SafeMsiHandle hDatabase);

		[DllImport("msi.dll", SetLastError = true)]
		public static extern int MsiDatabaseOpenView(SafeMsiHandle hDatabase, string szQuery, out SafeMsiHandle hView);

		[DllImport("msi.dll", SetLastError = true)]
		public static extern int MsiViewExecute(SafeMsiHandle hView, SafeMsiHandle hRecord);

		[DllImport("msi.dll", SetLastError = true)]
		public static extern int MsiViewFetch(SafeMsiHandle hView, out SafeMsiHandle hRecord);

		[DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int MsiRecordGetString(SafeMsiHandle hRecord, int iField, StringBuilder szValueBuf, ref int pcchValueBuf);

		[DllImport("msi.dll", ExactSpelling = true)]
		public static extern int MsiCloseHandle(IntPtr hAny);

		[DllImport("msi.dll", ExactSpelling = true)]
		public static extern SafeMsiHandle MsiGetLastErrorRecord();

		[DllImport("msi.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int MsiFormatRecord(SafeMsiHandle hInstall, SafeMsiHandle hRecord, StringBuilder szValueBuf, ref int pcchValueBuf);

		private const string MSIDLL = "msi.dll";
	}
}

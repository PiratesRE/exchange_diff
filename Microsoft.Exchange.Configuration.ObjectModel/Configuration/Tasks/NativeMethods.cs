using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal static class NativeMethods
	{
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr OpenSCManager(string machineName, string databaseName, ServiceControlManagerAccessFlags desiredAccess);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr OpenService(IntPtr serviceControllerManager, string serviceName, ServiceAccessFlags desiredAccess);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool CloseServiceHandle(IntPtr serviceControlHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ChangeServiceConfig2(IntPtr serviceController, ServiceConfigInfoLevels infoLevel, ref ServiceFailureActions failureActions);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ChangeServiceConfig2(IntPtr serviceController, ServiceConfigInfoLevels infoLevel, ref ServiceFailureActionsFlag failureActionsFlag);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ChangeServiceConfig2(IntPtr serviceController, ServiceConfigInfoLevels infoLevel, ref ServiceSidActions serviceSidActions);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ChangeServiceConfig(IntPtr serviceController, uint serviceType, uint startType, uint errorControl, string binaryPathName, string loadOrderGroup, string tagId, string dependencies, string serviceStartName, string password, string displayName);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int QueryServiceStatus(SafeHandle handle, ref SERVICE_STATUS ss);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool QueryServiceObjectSecurity(IntPtr serviceHandle, SecurityInfos secInfo, IntPtr securityDescriptorUnmanagedBuffer, int bufSize, out int bufSizeNeeded);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool SetServiceObjectSecurity(IntPtr hService, SecurityInfos dwSecurityInformation, IntPtr lpSecurityDescriptor);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint QueryServiceStatusEx(IntPtr hService, ServiceQueryStatus infoLevel, ref SERVICE_STATUS_EX lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool QueryServiceConfig(IntPtr serviceHandle, IntPtr buffer, uint cbBufSize, out uint cbBytesNeeded);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern uint GetShortPathName(string lpszLongPath, StringBuilder lpszShortPath, uint cchBuffer);

		[DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint NetUserChangePassword(string domainname, string username, IntPtr oldpassword, IntPtr newpassword);

		internal const uint SERVICE_NO_CHANGE = 4294967295U;
	}
}

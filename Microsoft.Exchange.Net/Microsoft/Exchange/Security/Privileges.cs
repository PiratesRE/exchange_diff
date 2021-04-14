using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security
{
	internal static class Privileges
	{
		public static int RemoveAllExcept(string[] privilegesToKeep)
		{
			int result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = Privileges.RemoveAllExcept(currentProcess, privilegesToKeep, string.Empty);
			}
			return result;
		}

		public static int RemoveAllExcept(string[] privilegesToKeep, string processLabel)
		{
			int result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = Privileges.RemoveAllExcept(currentProcess, privilegesToKeep, processLabel);
			}
			return result;
		}

		public static int RemoveAllExcept(Process managedProcessHandle, string[] privilegesToKeep)
		{
			return Privileges.RemoveAllExcept(managedProcessHandle, privilegesToKeep, string.Empty);
		}

		public static int RemoveAllExcept(Process managedProcessHandle, string[] privilegesToKeep, string processLabel)
		{
			int num = -1;
			SafeUserTokenHandle safeUserTokenHandle = null;
			SafeHGlobalHandle safeHGlobalHandle = null;
			try
			{
				safeUserTokenHandle = new SafeUserTokenHandle(IntPtr.Zero);
				if (NativeMethods.OpenProcessToken(managedProcessHandle.Handle, 40U, ref safeUserTokenHandle))
				{
					num = Privileges.GetTokenInformation(safeUserTokenHandle, NativeMethods.TOKEN_INFORMATION_CLASS.TokenPrivileges, ref safeHGlobalHandle);
					if (num == 0 && safeHGlobalHandle != null && !safeHGlobalHandle.IsInvalid)
					{
						num = Privileges.RemoveUnnecessaryPrivileges(ref safeUserTokenHandle, ref safeHGlobalHandle, privilegesToKeep);
					}
				}
				else
				{
					num = Marshal.GetLastWin32Error();
				}
			}
			finally
			{
				if (safeHGlobalHandle != null)
				{
					safeHGlobalHandle.Close();
				}
				if (safeUserTokenHandle != null)
				{
					safeUserTokenHandle.Close();
				}
				if (num != 0)
				{
					ExEventLog exEventLog = new ExEventLog(Privileges.componentGuid, "MSExchange Common");
					object[] array = new object[4];
					array[0] = managedProcessHandle.MainModule.FileName;
					array[1] = managedProcessHandle.Id;
					array[2] = processLabel;
					long num2 = (num < 0) ? ((long)num) : (((long)num & 65535L) | (long)((ulong)-2147024896));
					array[3] = string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", new object[]
					{
						num2
					});
					exEventLog.LogEvent(CommonEventLogConstants.Tuple_PrivilegeRemovalFailure, null, array);
				}
			}
			return num;
		}

		private static int GetTokenInformation(SafeUserTokenHandle tokenHandle, NativeMethods.TOKEN_INFORMATION_CLASS tokenInformationClass, ref SafeHGlobalHandle processTokenInfoHandle)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			SafeHGlobalHandle safeHGlobalHandle = new SafeHGlobalHandle(IntPtr.Zero);
			if (!NativeMethods.GetTokenInformation(tokenHandle, tokenInformationClass, safeHGlobalHandle, num2, ref num3))
			{
				num = Marshal.GetLastWin32Error();
				if (num == 122)
				{
					num = 0;
					num2 = num3;
					processTokenInfoHandle = NativeMethods.AllocHGlobal(num2);
					if (!NativeMethods.GetTokenInformation(tokenHandle, tokenInformationClass, processTokenInfoHandle, num2, ref num3))
					{
						num = Marshal.GetLastWin32Error();
						if (processTokenInfoHandle != null)
						{
							processTokenInfoHandle.Close();
							processTokenInfoHandle = null;
						}
					}
				}
			}
			safeHGlobalHandle.Close();
			return num;
		}

		private unsafe static int RemoveUnnecessaryPrivileges(ref SafeUserTokenHandle tokenHandle, ref SafeHGlobalHandle processTokenInfoHandle, string[] privilegesToKeep)
		{
			int num = 0;
			Hashtable hashtable = new Hashtable(privilegesToKeep.Length);
			SafeHGlobalHandle safeHGlobalHandle = null;
			NativeMethods.LUID luid;
			luid.LowPart = 0U;
			luid.HighPart = 0U;
			foreach (string lpName in privilegesToKeep)
			{
				if (!NativeMethods.LookupPrivilegeValue(null, lpName, ref luid))
				{
					num = Marshal.GetLastWin32Error();
					break;
				}
				hashtable.Add(luid, null);
			}
			if (num == 0)
			{
				try
				{
					uint* ptr = (uint*)processTokenInfoHandle.DangerousGetHandle().ToPointer();
					NativeMethods.LUID_AND_ATTRIBUTES* ptr2 = (NativeMethods.LUID_AND_ATTRIBUTES*)(ptr + 1);
					safeHGlobalHandle = NativeMethods.AllocHGlobal(Marshal.SizeOf(typeof(uint)) + (int)((long)Marshal.SizeOf(typeof(NativeMethods.LUID_AND_ATTRIBUTES)) * (long)((ulong)(*ptr))));
					uint* ptr3 = (uint*)safeHGlobalHandle.DangerousGetHandle().ToPointer();
					*ptr3 = 0U;
					NativeMethods.LUID_AND_ATTRIBUTES* ptr4 = (NativeMethods.LUID_AND_ATTRIBUTES*)(ptr3 + 1);
					int num2 = 0;
					while ((long)num2 < (long)((ulong)(*ptr)))
					{
						if (!hashtable.Contains(ptr2->Luid))
						{
							*ptr3 += 1U;
							ptr4->Luid = ptr2->Luid;
							ptr4->Attributes = 4U;
							ptr4++;
						}
						ptr2++;
						num2++;
					}
					if (!NativeMethods.AdjustTokenPrivileges(tokenHandle, false, safeHGlobalHandle, 0U, IntPtr.Zero, IntPtr.Zero))
					{
						num = Marshal.GetLastWin32Error();
					}
				}
				finally
				{
					if (safeHGlobalHandle != null)
					{
						safeHGlobalHandle.Close();
					}
				}
			}
			return num;
		}

		private static RawSecurityDescriptor GetTokenSecurityDescriptor(SafeHandle tokenHandle)
		{
			uint num = 0U;
			if (NativeMethods.GetKernelObjectSecurity(tokenHandle, SecurityInfos.DiscretionaryAcl, IntPtr.Zero, 0U, out num) || Marshal.GetLastWin32Error() != 122)
			{
				return null;
			}
			IntPtr intPtr = Marshal.AllocHGlobal((int)num);
			try
			{
				if (NativeMethods.GetKernelObjectSecurity(tokenHandle, SecurityInfos.DiscretionaryAcl, intPtr, num, out num))
				{
					byte[] array = new byte[num];
					Marshal.Copy(intPtr, array, 0, (int)num);
					return new RawSecurityDescriptor(array, 0);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return null;
		}

		private static bool SetTokenSecurityDescriptor(SafeHandle tokenHandle, RawSecurityDescriptor sd)
		{
			int binaryLength = sd.BinaryLength;
			byte[] array = new byte[binaryLength];
			sd.GetBinaryForm(array, 0);
			IntPtr intPtr = Marshal.AllocHGlobal(binaryLength);
			bool result;
			try
			{
				Marshal.Copy(array, 0, intPtr, binaryLength);
				result = NativeMethods.SetKernelObjectSecurity(tokenHandle, SecurityInfos.DiscretionaryAcl, intPtr);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		public static bool UpdateProcessDacl(Process process, Privileges.UpdateDacl customUpdate)
		{
			SafeUserTokenHandle safeUserTokenHandle = new SafeUserTokenHandle(IntPtr.Zero);
			if (!NativeMethods.OpenProcessToken(process.Handle, 393216U, ref safeUserTokenHandle))
			{
				return false;
			}
			bool result;
			try
			{
				RawSecurityDescriptor tokenSecurityDescriptor = Privileges.GetTokenSecurityDescriptor(safeUserTokenHandle);
				if (tokenSecurityDescriptor == null)
				{
					result = false;
				}
				else
				{
					Privileges.UpdateDaclFromSD(tokenSecurityDescriptor, customUpdate);
					result = Privileges.SetTokenSecurityDescriptor(safeUserTokenHandle, tokenSecurityDescriptor);
				}
			}
			finally
			{
				safeUserTokenHandle.Close();
			}
			return result;
		}

		private static void UpdateDaclFromSD(RawSecurityDescriptor sd, Privileges.UpdateDacl customUpdate)
		{
			DiscretionaryAcl discretionaryAcl = new DiscretionaryAcl(false, false, sd.DiscretionaryAcl);
			customUpdate(discretionaryAcl);
			byte[] binaryForm = new byte[discretionaryAcl.BinaryLength];
			discretionaryAcl.GetBinaryForm(binaryForm, 0);
			sd.DiscretionaryAcl = new RawAcl(binaryForm, 0);
		}

		public const string SeAssignPrimaryTokenPrivilege = "SeAssignPrimaryTokenPrivilege";

		public const string SeAuditPrivilege = "SeAuditPrivilege";

		public const string SeBackupPrivilege = "SeBackupPrivilege";

		public const string SeChangeNotifyPrivilege = "SeChangeNotifyPrivilege";

		public const string SeCreateGlobalPrivilege = "SeCreateGlobalPrivilege";

		public const string SeCreatePagefilePrivilege = "SeCreatePagefilePrivilege";

		public const string SeCreatePermanentPrivilege = "SeCreatePermanentPrivilege";

		public const string SeCreateSymbolicLinkPrivilege = "SeCreateSymbolicLinkPrivilege";

		public const string SeCreateTokenPrivilege = "SeCreateTokenPrivilege";

		public const string SeDebugPrivilege = "SeDebugPrivilege";

		public const string SeEnableDelegationPrivilege = "SeEnableDelegationPrivilege";

		public const string SeImpersonatePrivilege = "SeImpersonatePrivilege";

		public const string SeIncreaseBasePriorityPrivilege = "SeIncreaseBasePriorityPrivilege";

		public const string SeIncreaseQuotaPrivilege = "SeIncreaseQuotaPrivilege";

		public const string SeLoadDriverPrivilege = "SeLoadDriverPrivilege";

		public const string SeLockMemoryPrivilege = "SeLockMemoryPrivilege";

		public const string SeMachineAccountPrivilege = "SeMachineAccountPrivilege";

		public const string SeManageVolumePrivilege = "SeManageVolumePrivilege";

		public const string SeProfileSingleProcessPrivilege = "SeProfileSingleProcessPrivilege";

		public const string SeRemoteShutdownPrivilege = "SeRemoteShutdownPrivilege";

		public const string SeRestorePrivilege = "SeRestorePrivilege";

		public const string SeSecurityPrivilege = "SeSecurityPrivilege";

		public const string SeShutdownPrivilege = "SeShutdownPrivilege";

		public const string SeSyncAgentPrivilege = "SeSyncAgentPrivilege";

		public const string SeSystemEnvironmentPrivilege = "SeSystemEnvironmentPrivilege";

		public const string SeSystemProfilePrivilege = "SeSystemProfilePrivilege";

		public const string SeSystemtimePrivilege = "SeSystemtimePrivilege";

		public const string SeTakeOwnershipPrivilege = "SeTakeOwnershipPrivilege";

		public const string SeTcbPrivilege = "SeTcbPrivilege";

		public const string SeUndockPrivilege = "SeUndockPrivilege";

		public const string SeUnsolicitedInputPrivilege = "SeUnsolicitedInputPrivilege";

		private static Guid componentGuid = new Guid("{995740FC-0735-4d83-8DDA-5CB1D427560D}");

		public delegate void UpdateDacl(DiscretionaryAcl dacl);
	}
}

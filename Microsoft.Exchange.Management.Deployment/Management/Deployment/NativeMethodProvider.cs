using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.Setup;
using Microsoft.Exchange.Setup.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class NativeMethodProvider : INativeMethodProvider
	{
		public string GetSiteName(string server)
		{
			IntPtr zero = IntPtr.Zero;
			int num = NativeMethodProvider.DsGetSiteName(server, ref zero);
			string result = string.Empty;
			if (num == 0 && zero != IntPtr.Zero)
			{
				result = Marshal.PtrToStringUni(zero);
			}
			if (zero != IntPtr.Zero)
			{
				NativeMethodProvider.NetApiBufferFree(zero);
			}
			return result;
		}

		public uint GetAccessCheck(string ntsdString, string listString)
		{
			byte[] ntsd = NativeMethodProvider.ExtFormatByteArrayFromString(ntsdString);
			return this.GetAccessCheck(ntsd, listString);
		}

		public uint GetAccessCheck(byte[] ntsd, string listString)
		{
			uint desiredAccess = 33554432U;
			NativeMethodProvider.GENERIC_MAPPING genericMapping = NativeMethodProvider.GenericMapping(string.Empty);
			NativeMethodProvider.OBJECT_TYPE_LIST[] list = NativeMethodProvider.ObjectTypeList(string.Empty);
			return this.AccessCheck(desiredAccess, ntsd, genericMapping, list);
		}

		public bool TokenMembershipCheck(string sid)
		{
			bool result = false;
			IntPtr zero = IntPtr.Zero;
			try
			{
				if (!NativeMethodProvider.ConvertStringSidToSid(sid, out zero))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				if (!NativeMethodProvider.CheckTokenMembership(IntPtr.Zero, zero, ref result))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
			catch (Exception e)
			{
				SetupLogger.LogError(e);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					NativeMethodProvider.LocalFree(zero);
				}
			}
			return result;
		}

		public bool IsCoreServer()
		{
			int num;
			bool flag = NativeMethodProvider.GetProductInfo(Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor, 0, 0, out num);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2332437821U, ref num);
			if (flag)
			{
				flag = ((long)num == 12L || (long)num == 39L || (long)num == 13L || (long)num == 40L || (long)num == 14L || (long)num == 41L);
			}
			return flag;
		}

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool ConvertSidToStringSid(IntPtr sid, [MarshalAs(UnmanagedType.LPTStr)] [In] [Out] ref string pStringSid);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool ConvertStringSidToSid([MarshalAs(UnmanagedType.LPTStr)] [In] string stringSid, out IntPtr sid);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CheckTokenMembership(IntPtr TokenHandle, IntPtr SidToCheck, ref bool IsMember);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LocalFree(IntPtr hMem);

		[DllImport("NetApi32.dll", CharSet = CharSet.Unicode)]
		private static extern int DsGetSiteName(string server, ref IntPtr siteName);

		[DllImport("NetApi32.dll", CharSet = CharSet.Unicode)]
		private static extern int NetApiBufferFree(IntPtr ptr);

		[DllImport("kernel32.dll")]
		private static extern bool GetProductInfo(int dwOSMajorVersion, int dwOSMinorVersion, int dwSpMajorVersion, int dwSpMinorVersion, out int pdwReturnedProductType);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentThread();

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool ImpersonateSelf(int impersonationLevel);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool OpenThreadToken(IntPtr ProcessHandle, uint DesiredAccess, bool openAsSelf, ref IntPtr TokenHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, ref IntPtr TokenHandle);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CloseHandle(IntPtr handle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool RevertToSelf();

		private static byte[] ExtFormatByteArrayFromString(string valString)
		{
			byte[] array = new byte[valString.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToByte(valString.Substring(2 * i, 2), 16);
			}
			return array;
		}

		private static uint AccessMask(string maskString)
		{
			uint num = 0U;
			string[] array = maskString.Split(new char[]
			{
				'|'
			});
			string[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				string text = array2[i];
				string a;
				if ((a = text) == null)
				{
					goto IL_74;
				}
				if (!(a == "STANDARD_RIGHTS_ALL"))
				{
					if (!(a == "SPECIFIC_RIGHTS_ALL"))
					{
						if (!(a == "MAXIMUM_ALLOWED"))
						{
							goto IL_74;
						}
						num |= 33554432U;
					}
					else
					{
						num |= 65535U;
					}
				}
				else
				{
					num |= 2031616U;
				}
				IL_7B:
				i++;
				continue;
				IL_74:
				num = uint.Parse(text);
				goto IL_7B;
			}
			return num;
		}

		private static NativeMethodProvider.OBJECT_TYPE_LIST[] ObjectTypeList(string listString)
		{
			if (listString.Length == 0)
			{
				return new NativeMethodProvider.OBJECT_TYPE_LIST[0];
			}
			string[] array = listString.Split(new char[]
			{
				'|'
			});
			NativeMethodProvider.OBJECT_TYPE_LIST[] array2 = new NativeMethodProvider.OBJECT_TYPE_LIST[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i][1] != ';')
				{
					throw new ArgumentException();
				}
				short level = short.Parse(array[i].Substring(0, 1));
				Guid objectType = new Guid(array[i].Substring(2));
				array2[i] = new NativeMethodProvider.OBJECT_TYPE_LIST(level, 0, objectType);
			}
			return array2;
		}

		[DllImport("advapi32.dll")]
		private static extern void MapGenericMask(ref uint accessMask, ref NativeMethodProvider.GENERIC_MAPPING genericMapping);

		private static NativeMethodProvider.GENERIC_MAPPING GenericMapping(string mapping)
		{
			NativeMethodProvider.GENERIC_MAPPING result = default(NativeMethodProvider.GENERIC_MAPPING);
			if (mapping.Length > 0)
			{
				string[] array = mapping.Split(new char[]
				{
					','
				});
				if (array.Length != 4)
				{
					throw new ArgumentException();
				}
				result.GenericRead = Convert.ToUInt32(array[0], 16);
				result.GenericWrite = Convert.ToUInt32(array[1], 16);
				result.GenericExecute = Convert.ToUInt32(array[2], 16);
				result.GenericAll = Convert.ToUInt32(array[3], 16);
			}
			else
			{
				result.GenericRead = 2147483648U;
				result.GenericWrite = 1073741824U;
				result.GenericExecute = 536870912U;
				result.GenericAll = 268435456U;
			}
			return result;
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool AccessCheckByType([MarshalAs(UnmanagedType.LPArray)] byte[] pSecurityDescriptor, IntPtr principalSelfSid, IntPtr clientToken, uint DesiredAccess, IntPtr objectTypeList, int ObjectTypeListLength, ref NativeMethodProvider.GENERIC_MAPPING GenericMapping, IntPtr PrivilegeSet, ref int PrivilegeSetLength, ref uint GrantedAccess, ref int AccessStatus);

		private IntPtr GetTokenHandle()
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			int num = 0;
			if (!NativeMethodProvider.OpenThreadToken(NativeMethodProvider.GetCurrentThread(), 8U, true, ref zero))
			{
				num = Marshal.GetLastWin32Error();
				if (num == 1008)
				{
					num = 0;
					if (!NativeMethodProvider.OpenProcessToken(NativeMethodProvider.GetCurrentProcess(), 8U, ref zero))
					{
						num = Marshal.GetLastWin32Error();
					}
				}
			}
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
			return zero;
		}

		private uint AccessCheck(uint desiredAccess, byte[] ntsd, NativeMethodProvider.GENERIC_MAPPING genericMapping, NativeMethodProvider.OBJECT_TYPE_LIST[] list)
		{
			uint result = 0U;
			int num = 0;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr intPtr3 = IntPtr.Zero;
			bool flag = false;
			try
			{
				intPtr2 = Marshal.AllocHGlobal(1024);
				int num2 = 1024;
				NativeMethodProvider.MapGenericMask(ref desiredAccess, ref genericMapping);
				if (!NativeMethodProvider.ImpersonateSelf(2))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw new Win32Exception(lastWin32Error);
				}
				flag = true;
				intPtr = this.GetTokenHandle();
				intPtr3 = NativeMethodProvider.OBJECT_TYPE_LIST.NativeStruct(list);
				if (!NativeMethodProvider.AccessCheckByType(ntsd, IntPtr.Zero, intPtr, desiredAccess, intPtr3, list.Length, ref genericMapping, intPtr2, ref num2, ref result, ref num))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					SetupLogger.LogError(new Win32Exception(lastWin32Error));
				}
			}
			finally
			{
				if (intPtr3 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr3);
				}
				if (intPtr != IntPtr.Zero)
				{
					NativeMethodProvider.CloseHandle(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
				if (flag)
				{
					NativeMethodProvider.RevertToSelf();
				}
			}
			return result;
		}

		private const int ERRORNOTOKEN = 1008;

		private const int SecurityImpersonation = 2;

		private const uint TOKENQUERY = 8U;

		private const uint MaximumAllowed = 33554432U;

		private const uint ProductDatacenterServerCore = 12U;

		private const uint ProductDatacenterServerCoreV = 39U;

		private const uint ProductStandardServerCore = 13U;

		private const uint ProductStandardServerCoreV = 40U;

		private const uint ProductEnterpriseServerCore = 14U;

		private const uint ProductEnterpriseServerCoreV = 41U;

		private struct OBJECT_TYPE_LIST_NATIVE
		{
			internal short Level;

			internal short Sbz;

			internal IntPtr ObjectType;
		}

		private struct OBJECT_TYPE_LIST
		{
			public OBJECT_TYPE_LIST(short Level, short Sbz, Guid ObjectType)
			{
				this.Level = Level;
				this.Sbz = Sbz;
				this.ObjectType = ObjectType;
			}

			public static IntPtr NativeStruct(NativeMethodProvider.OBJECT_TYPE_LIST[] list)
			{
				if (list.Length == 0)
				{
					return IntPtr.Zero;
				}
				NativeMethodProvider.OBJECT_TYPE_LIST_NATIVE object_TYPE_LIST_NATIVE = default(NativeMethodProvider.OBJECT_TYPE_LIST_NATIVE);
				int num = Marshal.SizeOf(typeof(Guid));
				int num2 = Marshal.SizeOf(typeof(NativeMethodProvider.OBJECT_TYPE_LIST_NATIVE));
				IntPtr intPtr = Marshal.AllocHGlobal((num + num2) * list.Length);
				IntPtr intPtr2 = intPtr;
				IntPtr intPtr3 = (IntPtr)(((long)intPtr2 + (long)num2) * (long)list.Length);
				foreach (NativeMethodProvider.OBJECT_TYPE_LIST object_TYPE_LIST in list)
				{
					object_TYPE_LIST_NATIVE.Level = object_TYPE_LIST.Level;
					object_TYPE_LIST_NATIVE.Sbz = object_TYPE_LIST.Sbz;
					object_TYPE_LIST_NATIVE.ObjectType = intPtr3;
					Marshal.StructureToPtr(object_TYPE_LIST_NATIVE, intPtr2, false);
					Guid objectType = object_TYPE_LIST.ObjectType;
					Marshal.Copy(objectType.ToByteArray(), 0, intPtr3, num);
					intPtr2 = (IntPtr)((long)intPtr2 + (long)num2);
					intPtr3 = (IntPtr)((long)intPtr3 + (long)num);
				}
				return intPtr;
			}

			internal short Level;

			internal short Sbz;

			internal Guid ObjectType;
		}

		private struct GENERIC_MAPPING
		{
			public uint GenericRead;

			public uint GenericWrite;

			public uint GenericExecute;

			public uint GenericAll;
		}
	}
}

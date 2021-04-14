using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.Exchange.Win32
{
	internal static class NetworkShare
	{
		internal static void Create(string server, string path, string name, string description, DirectorySecurity securityDescriptor)
		{
			NetworkShare.NetShareNativeMethods.SHARE_INFO_502 share_INFO_ = default(NetworkShare.NetShareNativeMethods.SHARE_INFO_502);
			share_INFO_.netname = name;
			share_INFO_.type = 0U;
			share_INFO_.remark = description;
			share_INFO_.permissions = 0;
			share_INFO_.max_uses = -1;
			share_INFO_.current_uses = 0;
			share_INFO_.path = path;
			share_INFO_.passwd = null;
			share_INFO_.reserved = 0;
			byte[] securityDescriptorBinaryForm = securityDescriptor.GetSecurityDescriptorBinaryForm();
			GCHandle gchandle = GCHandle.Alloc(securityDescriptorBinaryForm, GCHandleType.Pinned);
			try
			{
				share_INFO_.security_descriptor = Marshal.UnsafeAddrOfPinnedArrayElement(securityDescriptorBinaryForm, 0);
				uint num2;
				int num = NetworkShare.NetShareNativeMethods.NetworkShareAdd(server, 502, ref share_INFO_, out num2);
				if (num == 2118)
				{
					num = NetworkShare.NetShareNativeMethods.NetworkShareSetInfo(server, name, 502, ref share_INFO_, out num2);
				}
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
			}
			finally
			{
				gchandle.Free();
			}
		}

		internal static void Delete(string server, string name)
		{
			int num = NetworkShare.NetShareNativeMethods.NetworkShareDel(server, name, 0);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		internal static bool AccessCheck(WindowsIdentity user, DirectorySecurity securityDescriptor, FileAccess fileAccess)
		{
			bool result = false;
			uint num = 256U;
			byte[] privilegeSet = new byte[num];
			NativeMethods.GENERIC_MAPPING generic_MAPPING;
			generic_MAPPING.GenericAll = 268435456U;
			generic_MAPPING.GenericRead = 2147483648U;
			generic_MAPPING.GenericWrite = 1073741824U;
			generic_MAPPING.GenericExecute = 536870912U;
			uint desiredAccess = 0U;
			uint num2 = 0U;
			switch (fileAccess)
			{
			case FileAccess.Read:
				desiredAccess = 1U;
				break;
			case FileAccess.Write:
				desiredAccess = 2U;
				break;
			case FileAccess.ReadWrite:
				desiredAccess = 3U;
				break;
			}
			byte[] securityDescriptorBinaryForm = securityDescriptor.GetSecurityDescriptorBinaryForm();
			NativeMethods.MapGenericMask(ref desiredAccess, ref generic_MAPPING);
			if (NetworkShare.NetShareNativeMethods.AccessCheck(securityDescriptorBinaryForm, user.Token, desiredAccess, ref generic_MAPPING, privilegeSet, ref num, out num2, out result) == 0)
			{
				throw new Win32Exception();
			}
			return result;
		}

		private static class NetShareNativeMethods
		{
			[DllImport("netapi32.dll", EntryPoint = "NetShareAdd")]
			internal static extern int NetworkShareAdd([MarshalAs(UnmanagedType.LPWStr)] string wszServer, int dwLevel, ref NetworkShare.NetShareNativeMethods.SHARE_INFO_502 shareInfo, out uint parm_err);

			[DllImport("netapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "NetShareSetInfo")]
			internal static extern int NetworkShareSetInfo(string wszServer, string strNetName, int dwLevel, ref NetworkShare.NetShareNativeMethods.SHARE_INFO_502 shareInfo, out uint parm_err);

			[DllImport("netapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "NetShareDel")]
			internal static extern int NetworkShareDel(string strServer, string strNetName, int reserved);

			[DllImport("advapi32.dll", SetLastError = true)]
			internal static extern int AccessCheck([MarshalAs(UnmanagedType.LPArray)] byte[] pSecurityDescriptor, IntPtr ClientToken, uint DesiredAccess, ref NativeMethods.GENERIC_MAPPING GenericMapping, [MarshalAs(UnmanagedType.LPArray)] byte[] PrivilegeSet, ref uint PrivilegeSetLength, out uint GrantedAccess, [MarshalAs(UnmanagedType.Bool)] out bool AccessStatus);

			private const string NETAPI32 = "netapi32.dll";

			private const string ADVAPI32 = "advapi32.dll";

			public const int FILE_READ_DATA = 1;

			public const int FILE_WRITE_DATA = 2;

			internal const int NERR_BASE = 2100;

			internal const int NERR_DuplicateShare = 2118;

			internal struct SHARE_INFO_502
			{
				[MarshalAs(UnmanagedType.LPWStr)]
				internal string netname;

				internal uint type;

				[MarshalAs(UnmanagedType.LPWStr)]
				internal string remark;

				internal int permissions;

				internal int max_uses;

				internal int current_uses;

				[MarshalAs(UnmanagedType.LPWStr)]
				internal string path;

				[MarshalAs(UnmanagedType.LPWStr)]
				internal string passwd;

				internal int reserved;

				internal IntPtr security_descriptor;
			}
		}
	}
}

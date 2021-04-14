using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SecurityPackageInfo
	{
		public SecurityPackageInfo(IntPtr buffer)
		{
			SecurityPackageInfo securityPackageInfo = (SecurityPackageInfo)Marshal.PtrToStructure(buffer, typeof(SecurityPackageInfo));
			this.Capabilities = securityPackageInfo.Capabilities;
			this.Version = securityPackageInfo.Version;
			this.RPCID = securityPackageInfo.RPCID;
			this.MaxToken = securityPackageInfo.MaxToken;
			this.Name = securityPackageInfo.Name;
			this.Comment = securityPackageInfo.Comment;
		}

		internal unsafe SecurityPackageInfo(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				IntPtr ptr2 = new IntPtr((void*)ptr);
				using (SafeContextBuffer safeContextBuffer = new SafeContextBuffer(Marshal.ReadIntPtr(ptr2, 0)))
				{
					SecurityPackageInfo securityPackageInfo = (SecurityPackageInfo)Marshal.PtrToStructure(safeContextBuffer.DangerousGetHandle(), typeof(SecurityPackageInfo));
					this.Capabilities = securityPackageInfo.Capabilities;
					this.Version = securityPackageInfo.Version;
					this.RPCID = securityPackageInfo.RPCID;
					this.MaxToken = securityPackageInfo.MaxToken;
					this.Name = securityPackageInfo.Name;
					this.Comment = securityPackageInfo.Comment;
				}
			}
		}

		internal SecurityPackageInfo.CapabilityFlags Capabilities;

		internal short Version;

		internal short RPCID;

		internal int MaxToken;

		internal string Name;

		internal string Comment;

		internal static readonly int Size = Marshal.SizeOf(typeof(SecurityPackageInfo));

		[Flags]
		internal enum CapabilityFlags
		{
			Integrity = 1,
			Privacy = 2,
			TokenOnly = 4,
			Datagram = 8,
			Connection = 16,
			MultiLegRequired = 32,
			ClientOnly = 64,
			ExtendedError = 128,
			Impersonation = 256,
			AcceptsWin32Names = 512,
			Stream = 1024,
			Negotiable = 2048,
			GssCompatible = 4096,
			Logon = 8192,
			AsciiBuffers = 16384,
			Fragment = 32768,
			MutualAuth = 65536,
			Delegation = 131072,
			ReadonlyWithChecksum = 262144
		}
	}
}

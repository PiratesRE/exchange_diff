using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.Exchange.Rpc.Nspi
{
	public class SafeSRowHandle : SafeRpcMemoryHandle
	{
		public SafeSRowHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeSRowHandle()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[return: MarshalAs(UnmanagedType.U1)]
		protected unsafe override bool ReleaseHandle()
		{
			if (!this.IsInvalid)
			{
				ulong num = (ulong)(*(long*)((byte*)this.handle.ToPointer() + 8L));
				if (num != 0UL)
				{
					<Module>.MIDL_user_free(num);
				}
				<Module>.MIDL_user_free(this.handle.ToPointer());
			}
			return true;
		}
	}
}

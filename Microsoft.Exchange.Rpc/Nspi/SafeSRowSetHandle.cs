using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.Exchange.Rpc.Nspi
{
	public class SafeSRowSetHandle : SafeRpcMemoryHandle
	{
		public SafeSRowSetHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeSRowSetHandle()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[return: MarshalAs(UnmanagedType.U1)]
		protected unsafe override bool ReleaseHandle()
		{
			if (!this.IsInvalid)
			{
				_SRowSet_r* ptr = (_SRowSet_r*)this.handle.ToPointer();
				uint num = 0;
				if (0 < *(int*)ptr)
				{
					do
					{
						<Module>.MIDL_user_free(((num + 1UL / 8UL) * 16L)[ptr / 8]);
						num++;
					}
					while (num < *(int*)ptr);
				}
				<Module>.MIDL_user_free(this.handle.ToPointer());
			}
			return true;
		}
	}
}

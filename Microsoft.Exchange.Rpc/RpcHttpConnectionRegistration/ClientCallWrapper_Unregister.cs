using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal class ClientCallWrapper_Unregister : ClientCallWrapper, IDisposable
	{
		protected override string Name()
		{
			return "RpcHttpConnectionRegistration::Unregister";
		}

		protected unsafe override int InternalExecute()
		{
			return <Module>.cli_RpcHttpConnectionRegistration_Unregister(base.HBinding, (_GUID*)this.m_pAssociationGroupId.ToPointer(), (_GUID*)this.m_pRequestId.ToPointer());
		}

		protected override void InternalCleanup()
		{
			if (this.m_pAssociationGroupId != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pAssociationGroupId);
				this.m_pAssociationGroupId = IntPtr.Zero;
			}
			if (this.m_pRequestId != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pRequestId);
				this.m_pRequestId = IntPtr.Zero;
			}
		}

		public unsafe ClientCallWrapper_Unregister(void* hBinding, Guid associationGroupId, Guid requestId) : base(hBinding)
		{
			this.m_pAssociationGroupId = IntPtr.Zero;
			this.m_pRequestId = IntPtr.Zero;
			bool flag = false;
			try
			{
				IntPtr pAssociationGroupId = Marshal.AllocHGlobal(16);
				this.m_pAssociationGroupId = pAssociationGroupId;
				Marshal.Copy(associationGroupId.ToByteArray(), 0, this.m_pAssociationGroupId, 16);
				IntPtr pRequestId = Marshal.AllocHGlobal(16);
				this.m_pRequestId = pRequestId;
				Marshal.Copy(requestId.ToByteArray(), 0, this.m_pRequestId, 16);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.InternalCleanup();
				}
			}
		}

		private void ~ClientCallWrapper_Unregister()
		{
			this.InternalCleanup();
		}

		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				this.InternalCleanup();
			}
			else
			{
				base.Finalize();
			}
		}

		public sealed void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private IntPtr m_pAssociationGroupId;

		private IntPtr m_pRequestId;
	}
}

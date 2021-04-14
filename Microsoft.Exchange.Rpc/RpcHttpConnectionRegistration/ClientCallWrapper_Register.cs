using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal class ClientCallWrapper_Register : ClientCallWrapper, IDisposable
	{
		protected override string Name()
		{
			return "RpcHttpConnectionRegistration::Register";
		}

		protected unsafe override int InternalExecute()
		{
			byte* ptr = null;
			byte* ptr2 = null;
			int result;
			try
			{
				int num = <Module>.cli_RpcHttpConnectionRegistration_Register(base.HBinding, (_GUID*)this.m_pAssociationGroupId.ToPointer(), (byte*)this.m_pToken.ToPointer(), (byte*)this.m_pServerTarget.ToPointer(), (byte*)this.m_pSessionCookie.ToPointer(), (byte*)this.m_pClientIp.ToPointer(), (_GUID*)this.m_pRequestId.ToPointer(), &ptr, &ptr2);
				if (ptr != null)
				{
					IntPtr ptr3 = new IntPtr((void*)ptr);
					this.m_failureMessage = Marshal.PtrToStringAnsi(ptr3);
				}
				if (ptr2 != null)
				{
					IntPtr ptr4 = new IntPtr((void*)ptr2);
					this.m_failureDetails = Marshal.PtrToStringAnsi(ptr4);
				}
				result = num;
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
					ptr = null;
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
					ptr2 = null;
				}
			}
			return result;
		}

		protected override void InternalCleanup()
		{
			if (this.m_pAssociationGroupId != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pAssociationGroupId);
				this.m_pAssociationGroupId = IntPtr.Zero;
			}
			if (this.m_pToken != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pToken);
				this.m_pToken = IntPtr.Zero;
			}
			if (this.m_pServerTarget != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pServerTarget);
				this.m_pServerTarget = IntPtr.Zero;
			}
			if (this.m_pSessionCookie != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pSessionCookie);
				this.m_pSessionCookie = IntPtr.Zero;
			}
			if (this.m_pClientIp != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pClientIp);
				this.m_pClientIp = IntPtr.Zero;
			}
			if (this.m_pRequestId != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_pRequestId);
				this.m_pRequestId = IntPtr.Zero;
			}
		}

		public unsafe ClientCallWrapper_Register(void* hBinding, Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId) : base(hBinding)
		{
			this.m_pAssociationGroupId = IntPtr.Zero;
			this.m_pToken = IntPtr.Zero;
			this.m_pServerTarget = IntPtr.Zero;
			this.m_pSessionCookie = IntPtr.Zero;
			this.m_pClientIp = IntPtr.Zero;
			this.m_pRequestId = IntPtr.Zero;
			bool flag = false;
			try
			{
				IntPtr pAssociationGroupId = Marshal.AllocHGlobal(16);
				this.m_pAssociationGroupId = pAssociationGroupId;
				Marshal.Copy(associationGroupId.ToByteArray(), 0, this.m_pAssociationGroupId, 16);
				IntPtr pToken = Marshal.StringToHGlobalAnsi(token);
				this.m_pToken = pToken;
				IntPtr pServerTarget = Marshal.StringToHGlobalAnsi(serverTarget);
				this.m_pServerTarget = pServerTarget;
				IntPtr pSessionCookie = Marshal.StringToHGlobalAnsi(sessionCookie);
				this.m_pSessionCookie = pSessionCookie;
				IntPtr pClientIp = Marshal.StringToHGlobalAnsi(clientIp);
				this.m_pClientIp = pClientIp;
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

		public string FailureMessage
		{
			get
			{
				return this.m_failureMessage;
			}
		}

		public string FailureDetails
		{
			get
			{
				return this.m_failureDetails;
			}
		}

		private void ~ClientCallWrapper_Register()
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

		private IntPtr m_pToken;

		private IntPtr m_pServerTarget;

		private IntPtr m_pSessionCookie;

		private IntPtr m_pClientIp;

		private IntPtr m_pRequestId;

		private string m_failureMessage;

		private string m_failureDetails;
	}
}

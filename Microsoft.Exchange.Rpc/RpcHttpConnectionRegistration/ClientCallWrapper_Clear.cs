using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal class ClientCallWrapper_Clear : ClientCallWrapper, IDisposable
	{
		protected override string Name()
		{
			return "RpcHttpConnectionRegistration::Clear";
		}

		protected override int InternalExecute()
		{
			return <Module>.cli_RpcHttpConnectionRegistration_Clear(base.HBinding);
		}

		protected override void InternalCleanup()
		{
		}

		public unsafe ClientCallWrapper_Clear(void* hBinding) : base(hBinding)
		{
		}

		private void ~ClientCallWrapper_Clear()
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
	}
}

using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal abstract class ClientCallWrapper
	{
		[HandleProcessCorruptedStateExceptions]
		private int WrappedExecute()
		{
			int num = 0;
			int result = 0;
			<Module>.RpcBindingReset(this.hBinding);
			try
			{
				result = this.InternalExecute();
			}
			catch when (endfilter(true))
			{
				num = Marshal.GetExceptionCode();
			}
			if (num != null)
			{
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, this.Name());
			}
			return result;
		}

		protected unsafe ClientCallWrapper(void* hBinding)
		{
			this.hBinding = hBinding;
		}

		protected unsafe void* HBinding
		{
			get
			{
				return this.hBinding;
			}
		}

		protected abstract string Name();

		protected abstract int InternalExecute();

		protected abstract void InternalCleanup();

		public int Execute()
		{
			int result;
			try
			{
				result = this.WrappedExecute();
			}
			finally
			{
				this.InternalCleanup();
			}
			return result;
		}

		private unsafe void* hBinding;
	}
}

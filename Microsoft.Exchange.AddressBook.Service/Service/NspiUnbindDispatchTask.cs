using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiUnbindDispatchTask : NspiDispatchTask
	{
		public NspiUnbindDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiUnbindFlags flags) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
		}

		public NspiStatus End(out int contextHandle)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			if (base.Status == NspiStatus.Success)
			{
				base.IsContextRundown = true;
				contextHandle = 0;
			}
			else
			{
				contextHandle = base.ContextHandle;
			}
			if (base.Status == NspiStatus.Success)
			{
				return NspiStatus.UnbindSuccess;
			}
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiUnbind";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiUnbindFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			base.Context.Unbind(false);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiUnbindDispatchTask>(this);
		}

		private const string Name = "NspiUnbind";

		private readonly NspiUnbindFlags flags;
	}
}

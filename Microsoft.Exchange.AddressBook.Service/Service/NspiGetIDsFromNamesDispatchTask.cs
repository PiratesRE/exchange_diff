using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiGetIDsFromNamesDispatchTask : NspiDispatchTask
	{
		public NspiGetIDsFromNamesDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiGetIDsFromNamesFlags flags, int mapiFlags, int nameCount, IntPtr names) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.mapiFlags = mapiFlags;
		}

		public NspiStatus End(out PropertyTag[] propertyTags)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			propertyTags = null;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiGetIDsFromNames";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiGetIDsFromNamesFlags, int>((long)base.ContextHandle, "{0} params: flags={1}, mapiFlags={2}", this.TaskName, this.flags, this.mapiFlags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			base.NspiContextCallWrapper("GetIDsFromNames", () => NspiStatus.NotSupported);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetIDsFromNamesDispatchTask>(this);
		}

		private const string Name = "NspiGetIDsFromNames";

		private readonly NspiGetIDsFromNamesFlags flags;

		private readonly int mapiFlags;
	}
}

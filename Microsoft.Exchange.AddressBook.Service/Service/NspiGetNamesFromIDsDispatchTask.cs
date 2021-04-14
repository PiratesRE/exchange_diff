using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiGetNamesFromIDsDispatchTask : NspiDispatchTask
	{
		public NspiGetNamesFromIDsDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiGetNamesFromIDsFlags flags, Guid? guid, PropertyTag[] propertyTags) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.guid = guid;
		}

		public NspiStatus End(out PropertyTag[] propertyTags, out SafeRpcMemoryHandle namesHandle)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			propertyTags = null;
			namesHandle = null;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiGetNamesFromIDs";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiGetNamesFromIDsFlags, string>((long)base.ContextHandle, "{0} params: flags={1}, guid={2}", this.TaskName, this.flags, (this.guid != null) ? this.guid.Value.ToString() : "<null>");
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			base.NspiContextCallWrapper("GetNamesFromIDs", () => NspiStatus.NotSupported);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetNamesFromIDsDispatchTask>(this);
		}

		private const string Name = "NspiGetNamesFromIDs";

		private readonly NspiGetNamesFromIDsFlags flags;

		private readonly Guid? guid;
	}
}

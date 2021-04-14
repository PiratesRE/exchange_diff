using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiQueryColumnsDispatchTask : NspiDispatchTask
	{
		public NspiQueryColumnsDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiQueryColumnsFlags flags, NspiQueryColumnsMapiFlags mapiFlags) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.mapiFlags = mapiFlags;
		}

		public NspiStatus End(out PropertyTag[] columns)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			columns = this.returnColumns;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiQueryColumns";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiQueryColumnsFlags, NspiQueryColumnsMapiFlags>((long)base.ContextHandle, "{0} params: flags={1}, mapiFlags={2}", this.TaskName, this.flags, this.mapiFlags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			IList<PropTag> mapiPropTagList = null;
			base.NspiContextCallWrapper("QueryColumns", () => this.Context.QueryColumns(this.mapiFlags, out mapiPropTagList));
			if (base.Status == NspiStatus.Success && mapiPropTagList != null)
			{
				this.returnColumns = ConvertHelper.ConvertFromMapiPropTags(mapiPropTagList.ToArray<PropTag>());
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiQueryColumnsDispatchTask>(this);
		}

		private const string Name = "NspiQueryColumns";

		private readonly NspiQueryColumnsFlags flags;

		private readonly NspiQueryColumnsMapiFlags mapiFlags;

		private PropertyTag[] returnColumns;
	}
}

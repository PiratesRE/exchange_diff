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
	internal sealed class NspiGetPropListDispatchTask : NspiDispatchTask
	{
		public NspiGetPropListDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiGetPropListFlags flags, int mid, int codePage) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.mid = mid;
			this.codePage = codePage;
		}

		public NspiStatus End(out PropertyTag[] propertyTags)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			propertyTags = this.returnPropertyTags;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiGetPropList";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug((long)base.ContextHandle, "{0} params: flags={1}, mid={2}, codePage={3}", new object[]
			{
				this.TaskName,
				this.flags,
				this.mid,
				this.codePage
			});
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			IList<PropTag> mapiPropTagList = null;
			base.NspiContextCallWrapper("GetPropList", () => this.Context.GetPropList(this.flags, this.mid, (uint)this.codePage, out mapiPropTagList));
			if (base.Status == NspiStatus.Success && mapiPropTagList != null)
			{
				this.returnPropertyTags = ConvertHelper.ConvertFromMapiPropTags(mapiPropTagList.ToArray<PropTag>());
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetPropListDispatchTask>(this);
		}

		private const string Name = "NspiGetPropList";

		private readonly NspiGetPropListFlags flags;

		private readonly int mid;

		private readonly int codePage;

		private PropertyTag[] returnPropertyTags;
	}
}

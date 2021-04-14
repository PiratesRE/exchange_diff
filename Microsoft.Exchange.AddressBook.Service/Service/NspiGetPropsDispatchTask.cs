using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiGetPropsDispatchTask : NspiStateDispatchTask
	{
		public NspiGetPropsDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiGetPropsFlags flags, NspiState state, PropertyTag[] propertyTags) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.propertyTags = propertyTags;
		}

		public NspiStatus End(out int codePage, out PropertyValue[] row)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			codePage = this.returnCodePage;
			row = this.returnRow;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiGetProps";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiGetPropsFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			PropRow mapiRow = null;
			PropTag[] mapiPropTags = ConvertHelper.ConvertToMapiPropTags(this.propertyTags);
			base.NspiContextCallWrapper("GetProps", () => this.Context.GetProps(this.flags, this.NspiState, mapiPropTags, out mapiRow));
			if (base.Status == NspiStatus.Success || base.Status == NspiStatus.ErrorsReturned)
			{
				this.returnCodePage = base.NspiState.CodePage;
				this.returnRow = ConvertHelper.ConvertFromMapiPropRow(mapiRow, this.returnCodePage);
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetPropsDispatchTask>(this);
		}

		private const string Name = "NspiGetProps";

		private readonly NspiGetPropsFlags flags;

		private readonly PropertyTag[] propertyTags;

		private int returnCodePage;

		private PropertyValue[] returnRow;
	}
}

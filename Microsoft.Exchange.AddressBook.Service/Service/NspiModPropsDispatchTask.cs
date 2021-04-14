using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiModPropsDispatchTask : NspiStateDispatchTask
	{
		public NspiModPropsDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiModPropsFlags flags, NspiState state, PropertyTag[] propertyTags, PropertyValue[] row) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.propertyTags = propertyTags;
			this.row = row;
		}

		public NspiStatus End()
		{
			base.CheckDisposed();
			base.CheckCompletion();
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiModProps";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiModPropsFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			PropRow mapiRow = ConvertHelper.ConvertToMapiPropRow(this.row);
			PropTag[] mapiPropTags = ConvertHelper.ConvertToMapiPropTags(this.propertyTags);
			base.NspiContextCallWrapper("ModProps", () => this.Context.ModProps(this.NspiState, mapiPropTags, mapiRow));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiModPropsDispatchTask>(this);
		}

		private const string Name = "NspiModProps";

		private readonly NspiModPropsFlags flags;

		private readonly PropertyTag[] propertyTags;

		private readonly PropertyValue[] row;
	}
}

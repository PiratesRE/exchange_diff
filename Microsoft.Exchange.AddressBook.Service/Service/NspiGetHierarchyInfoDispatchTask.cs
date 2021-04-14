using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiGetHierarchyInfoDispatchTask : NspiStateDispatchTask
	{
		public NspiGetHierarchyInfoDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiGetHierarchyInfoFlags flags, NspiState state, int version) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.version = version;
		}

		public NspiStatus End(out int codePage, out int version, out PropertyValue[][] rowset)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			codePage = this.returnCodePage;
			version = this.returnVersion;
			rowset = this.returnRowset;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiGetHierarchyInfo";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiGetHierarchyInfoFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			PropRowSet mapiRowset = null;
			uint localVersion = (uint)this.version;
			base.NspiContextCallWrapper("GetHierarchyInfo", () => this.Context.GetHierarchyInfo(this.flags, this.NspiState, ref localVersion, out mapiRowset));
			this.returnVersion = (int)localVersion;
			if (base.Status == NspiStatus.Success)
			{
				this.returnCodePage = base.NspiState.CodePage;
				this.returnRowset = ConvertHelper.ConvertFromMapiPropRowSet(mapiRowset, this.returnCodePage);
				NspiDispatchTask.NspiTracer.TraceDebug<int>((long)base.ContextHandle, "Rows returned: {0}", (mapiRowset == null) ? 0 : mapiRowset.Rows.Count);
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetHierarchyInfoDispatchTask>(this);
		}

		private const string Name = "NspiGetHierarchyInfo";

		private readonly NspiGetHierarchyInfoFlags flags;

		private readonly int version;

		private int returnCodePage;

		private int returnVersion;

		private PropertyValue[][] returnRowset;
	}
}

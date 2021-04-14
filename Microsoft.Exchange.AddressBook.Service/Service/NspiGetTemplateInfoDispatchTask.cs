using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiGetTemplateInfoDispatchTask : NspiDispatchTask
	{
		public NspiGetTemplateInfoDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiGetTemplateInfoFlags flags, int type, string legacyDN, int codePage, int locale) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.type = type;
			this.legacyDN = legacyDN;
			this.codePage = codePage;
			this.locale = locale;
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
				return "NspiGetTemplateInfo";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug((long)base.ContextHandle, "{0} params: flags={1}, type={2}, legacyDN={3}, codePage={4}, locale={5}", new object[]
			{
				this.TaskName,
				this.flags,
				this.type,
				this.legacyDN,
				this.codePage,
				this.locale
			});
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			PropRow mapiRow = null;
			base.NspiContextCallWrapper("GetTemplateInfo", () => this.Context.GetTemplateInfo(this.flags, (uint)this.type, this.legacyDN, (uint)this.codePage, (uint)this.locale, out mapiRow));
			if (base.Status == NspiStatus.Success)
			{
				this.returnCodePage = this.codePage;
				this.returnRow = ConvertHelper.ConvertFromMapiPropRow(mapiRow, this.returnCodePage);
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetTemplateInfoDispatchTask>(this);
		}

		private const string Name = "NspiGetTemplateInfo";

		private readonly NspiGetTemplateInfoFlags flags;

		private readonly int type;

		private readonly string legacyDN;

		private readonly int codePage;

		private readonly int locale;

		private int returnCodePage;

		private PropertyValue[] returnRow;
	}
}

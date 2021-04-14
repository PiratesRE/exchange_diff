using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal abstract class FastTransferServerObject<TContext> : ServerObject, WatsonHelper.IProvideWatsonReportData where TContext : BaseObject
	{
		protected TContext Context
		{
			get
			{
				return this.context;
			}
		}

		protected FastTransferServerObject(TContext context, Logon logon) : base(logon)
		{
			this.context = context;
		}

		protected virtual WatsonReportActionType WatsonReportActionType
		{
			get
			{
				return WatsonReportActionType.FastTransferState;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferServerObject<TContext>>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.context);
			base.InternalDispose();
		}

		string WatsonHelper.IProvideWatsonReportData.GetWatsonReportString()
		{
			base.CheckDisposed();
			return string.Format("FastTransferContext: {0}", this.Context);
		}

		private TContext context;
	}
}

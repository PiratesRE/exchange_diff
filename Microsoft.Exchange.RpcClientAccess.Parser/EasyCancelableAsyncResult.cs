using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EasyCancelableAsyncResult : EasyAsyncResultBase, ICancelableAsyncResult, IAsyncResult
	{
		public EasyCancelableAsyncResult(CancelableAsyncCallback asyncCallback, object asyncState) : base(asyncState)
		{
			this.asyncCallback = asyncCallback;
		}

		public bool IsCanceled
		{
			get
			{
				return this.isCanceled;
			}
		}

		public void Cancel()
		{
			if (this.isCanceled)
			{
				return;
			}
			lock (base.CompletionLock)
			{
				if (this.isCanceled)
				{
					return;
				}
				this.isCanceled = true;
			}
			if (!base.IsCompleted)
			{
				this.InternalCancel();
				return;
			}
		}

		protected virtual void InternalCancel()
		{
		}

		protected override void InternalCallback()
		{
			if (this.asyncCallback != null)
			{
				this.asyncCallback(this);
			}
		}

		private readonly CancelableAsyncCallback asyncCallback;

		private bool isCanceled;
	}
}

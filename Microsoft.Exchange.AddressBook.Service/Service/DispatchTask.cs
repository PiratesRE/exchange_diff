using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal abstract class DispatchTask : BaseObject
	{
		public DispatchTask(CancelableAsyncCallback asyncCallback, object asyncState)
		{
			this.asyncResult = new DispatchTaskAsyncResult(this, asyncCallback, asyncState);
		}

		public DispatchTaskAsyncResult AsyncResult
		{
			get
			{
				base.CheckDisposed();
				return this.asyncResult;
			}
		}

		protected abstract string TaskName { get; }

		protected void Completion()
		{
			this.asyncResult.InvokeCallback();
		}

		protected void CheckCompletion()
		{
			this.asyncResult.WaitForCompletion();
		}

		private readonly DispatchTaskAsyncResult asyncResult;
	}
}

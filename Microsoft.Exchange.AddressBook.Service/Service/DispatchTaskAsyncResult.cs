using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class DispatchTaskAsyncResult : EasyCancelableAsyncResult
	{
		public DispatchTaskAsyncResult(DispatchTask dispatchTask, CancelableAsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.dispatchTask = dispatchTask;
		}

		public DispatchTask DispatchTask
		{
			get
			{
				return this.dispatchTask;
			}
		}

		protected override void InternalCancel()
		{
		}

		private readonly DispatchTask dispatchTask;
	}
}

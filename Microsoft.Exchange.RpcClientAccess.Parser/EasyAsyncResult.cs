using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EasyAsyncResult : EasyAsyncResultBase
	{
		public EasyAsyncResult(AsyncCallback asyncCallback, object asyncState) : base(asyncState)
		{
			this.asyncCallback = asyncCallback;
		}

		protected sealed override void InternalCallback()
		{
			if (this.asyncCallback != null)
			{
				this.asyncCallback(this);
			}
		}

		private readonly AsyncCallback asyncCallback;
	}
}

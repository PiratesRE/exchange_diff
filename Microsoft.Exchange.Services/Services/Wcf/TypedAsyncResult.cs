using System;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class TypedAsyncResult<T> : AsyncResultBase
	{
		protected TypedAsyncResult(AsyncCallback callback, object state) : base(callback, state)
		{
		}

		public T Data
		{
			get
			{
				return this.data;
			}
		}

		protected void Complete(T data, bool completedSynchronously)
		{
			this.data = data;
			base.Complete(completedSynchronously);
		}

		public static T End(IAsyncResult result)
		{
			TypedAsyncResult<T> typedAsyncResult = AsyncResultBase.End<TypedAsyncResult<T>>(result);
			return typedAsyncResult.Data;
		}

		private T data;
	}
}

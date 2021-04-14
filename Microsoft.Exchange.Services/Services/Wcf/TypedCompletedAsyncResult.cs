using System;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class TypedCompletedAsyncResult<T> : TypedAsyncResult<T>
	{
		public TypedCompletedAsyncResult(T data, AsyncCallback callback, object state) : base(callback, state)
		{
			base.Complete(data, true);
		}

		public new static T End(IAsyncResult result)
		{
			TypedCompletedAsyncResult<T> typedCompletedAsyncResult = result as TypedCompletedAsyncResult<T>;
			if (typedCompletedAsyncResult == null)
			{
				throw new ArgumentException("Invalid async result.", "result");
			}
			return TypedAsyncResult<T>.End(typedCompletedAsyncResult);
		}
	}
}

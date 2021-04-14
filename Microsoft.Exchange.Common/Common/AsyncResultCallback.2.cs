using System;

namespace Microsoft.Exchange.Common
{
	public delegate void AsyncResultCallback<T>(AsyncResult<T> asyncResult);
}

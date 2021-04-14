using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AsyncOperationResult<TData>
	{
		public AsyncOperationResult(TData data, Exception exception)
		{
			this.data = data;
			this.exception = exception;
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public TData Data
		{
			get
			{
				return this.data;
			}
		}

		public bool IsSucceeded
		{
			get
			{
				return this.exception == null;
			}
		}

		public virtual bool IsTransientException
		{
			get
			{
				return this.exception is TransientException;
			}
		}

		public override string ToString()
		{
			if (this.IsSucceeded)
			{
				return "Success";
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
			{
				this.exception.GetType().FullName,
				this.exception.Message
			});
		}

		private readonly TData data;

		private readonly Exception exception;
	}
}

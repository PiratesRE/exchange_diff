using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AsyncOperationResult<TData>
	{
		public AsyncOperationResult(Exception exception)
		{
			this.exception = exception;
		}

		public AsyncOperationResult(TData data)
		{
			this.data = data;
		}

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

		public bool IsCanceled
		{
			get
			{
				return this.exception is OperationCanceledException;
			}
		}

		public bool IsSucceeded
		{
			get
			{
				return this.exception == null;
			}
		}

		public bool IsRetryable
		{
			get
			{
				return this.exception is TransientException;
			}
		}

		public override string ToString()
		{
			if (this.IsCanceled)
			{
				return "Canceled";
			}
			if (this.IsSucceeded)
			{
				return "Success";
			}
			return this.exception.ToString();
		}

		public static readonly Exception CanceledException = new OperationCanceledException();

		private readonly TData data;

		private readonly Exception exception;
	}
}

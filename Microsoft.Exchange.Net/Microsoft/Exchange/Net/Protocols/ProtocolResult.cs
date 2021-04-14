using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.Protocols
{
	internal sealed class ProtocolResult
	{
		internal ProtocolResult(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			this.exception = exception;
		}

		internal ProtocolResult(ResultData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.data = data;
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public ResultData Data
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
			return this.exception.GetType().FullName;
		}

		private readonly ResultData data;

		private readonly Exception exception;
	}
}

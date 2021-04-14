using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Net.LiveIDAuthentication
{
	internal sealed class AuthenticationResult : ResultData
	{
		internal AuthenticationResult(BaseAuthenticationToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			this.token = token;
			this.exception = null;
		}

		internal AuthenticationResult(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			this.exception = exception;
			this.token = null;
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public AuthenticationToken Token
		{
			get
			{
				return this.token as AuthenticationToken;
			}
		}

		public SamlAuthenticationToken SamlToken
		{
			get
			{
				return this.token as SamlAuthenticationToken;
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
				return this.exception is TransientException || this.IsCanceled;
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

		private readonly BaseAuthenticationToken token;

		private readonly Exception exception;
	}
}

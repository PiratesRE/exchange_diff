using System;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class InvalidOAuthTokenException : Exception
	{
		public InvalidOAuthTokenException(OAuthErrors error, object[] args = null, Exception innerException = null) : base((args == null) ? OAuthErrorsUtil.GetDescription(error) : string.Format(OAuthErrorsUtil.GetDescription(error), args), innerException)
		{
			this.ErrorCode = error;
		}

		public OAuthErrorCategory ErrorCategory
		{
			get
			{
				return OAuthErrorsUtil.GetErrorCategory(this.ErrorCode);
			}
		}

		public OAuthErrors ErrorCode { get; private set; }

		public string ExtraData { get; set; }

		public bool LogEvent { get; set; }

		public string LogPeriodicKey { get; set; }

		public static Lazy<InvalidOAuthTokenException> OAuthRequestProxyToDownLevelException = new Lazy<InvalidOAuthTokenException>(() => new InvalidOAuthTokenException(OAuthErrors.UserOAuthNotSupported, null, null));
	}
}

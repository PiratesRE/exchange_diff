using System;

namespace Microsoft.Exchange.Security.OAuth
{
	public class OAuthTokenRequestFailedException : Exception
	{
		public OAuthOutboundErrorCodes ErrorCode { get; private set; }

		public OAuthTokenRequestFailedException(OAuthOutboundErrorCodes error, object[] args, Exception innerException = null) : base(OAuthOutboundErrorsUtil.GetDescription(error, args), innerException)
		{
			this.ErrorCode = error;
		}

		public OAuthTokenRequestFailedException(OAuthOutboundErrorCodes error, string args = null, Exception innerException = null) : base(OAuthOutboundErrorsUtil.GetDescription(error, args), innerException)
		{
			this.ErrorCode = error;
		}

		public string GetKeyForErrorCode()
		{
			return this.ErrorCode.ToString();
		}
	}
}

using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public class ErrorEwsResponseException : LocalizedException
	{
		public ResponseCodeType ResponseCodeType { get; private set; }

		public ErrorEwsResponseException(ResponseCodeType responseCodeType) : base(Strings.descErrorEwsResponse((int)responseCodeType))
		{
			this.ResponseCodeType = responseCodeType;
		}
	}
}

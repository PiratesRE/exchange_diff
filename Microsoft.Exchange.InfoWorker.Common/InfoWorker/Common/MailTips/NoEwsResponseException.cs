using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public class NoEwsResponseException : LocalizedException
	{
		public NoEwsResponseException() : base(Strings.descNoEwsResponse)
		{
		}
	}
}

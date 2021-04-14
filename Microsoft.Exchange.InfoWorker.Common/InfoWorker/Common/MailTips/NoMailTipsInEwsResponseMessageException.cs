using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public class NoMailTipsInEwsResponseMessageException : LocalizedException
	{
		public NoMailTipsInEwsResponseMessageException() : base(Strings.descNoMailTipsInEwsResponseMessage)
		{
		}
	}
}

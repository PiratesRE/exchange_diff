using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal sealed class MailTipsQueryResult : BaseQueryResult
	{
		public MailTips MailTips
		{
			get
			{
				return this.mailTips;
			}
		}

		internal MailTipsQueryResult(MailTips mailTips)
		{
			this.mailTips = mailTips;
		}

		internal MailTipsQueryResult(LocalizedException exception) : base(exception)
		{
		}

		private MailTips mailTips;
	}
}

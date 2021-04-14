using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public class QueryGenerationNotRequiredException : LocalizedException
	{
		public QueryGenerationNotRequiredException() : base(Strings.descQueryGenerationNotRequired)
		{
		}
	}
}

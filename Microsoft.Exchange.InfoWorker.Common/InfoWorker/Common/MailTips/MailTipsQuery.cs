using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal sealed class MailTipsQuery : BaseQuery
	{
		public new MailTipsQueryResult Result
		{
			get
			{
				return (MailTipsQueryResult)base.Result;
			}
		}

		internal Dictionary<string, long> LatencyTracker { get; set; }

		internal MailTipsPermission Permission { get; set; }

		public static MailTipsQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return new MailTipsQuery(recipientData, new MailTipsQueryResult(exception));
		}

		public static MailTipsQuery CreateFromIndividual(RecipientData recipientData)
		{
			return new MailTipsQuery(recipientData, null);
		}

		public static MailTipsQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return new MailTipsQuery(recipientData, new MailTipsQueryResult(exception));
		}

		private MailTipsQuery(RecipientData recipientData, MailTipsQueryResult result) : base(recipientData, result)
		{
		}
	}
}

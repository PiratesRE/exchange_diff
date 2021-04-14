using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "FindMailboxStatisticsByKeywordsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindMailboxStatisticsByKeywordsResponseMessage : ResponseMessage
	{
		[XmlElement("MailboxStatisticsSearchResult", IsNullable = false)]
		public MailboxStatisticsSearchResult MailboxStatisticsSearchResult
		{
			get
			{
				return this.mailboxStatisticsSearchResult;
			}
			set
			{
				this.mailboxStatisticsSearchResult = value;
			}
		}

		public FindMailboxStatisticsByKeywordsResponseMessage()
		{
		}

		internal FindMailboxStatisticsByKeywordsResponseMessage(ServiceResultCode code, ServiceError error, UserMailbox userMailbox, KeywordStatisticsSearchResult keywordStatisticsSearchResult) : base(code, error)
		{
			this.mailboxStatisticsSearchResult.UserMailbox = userMailbox;
			this.mailboxStatisticsSearchResult.KeywordStatisticsSearchResult = ((keywordStatisticsSearchResult == null) ? new KeywordStatisticsSearchResult() : keywordStatisticsSearchResult);
		}

		private MailboxStatisticsSearchResult mailboxStatisticsSearchResult = new MailboxStatisticsSearchResult();
	}
}

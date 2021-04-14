using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "MailboxStatisticsSearchResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MailboxStatisticsSearchResult
	{
		[XmlElement("UserMailbox")]
		public UserMailbox UserMailbox
		{
			get
			{
				return this.userMailboxField;
			}
			set
			{
				this.userMailboxField = value;
			}
		}

		[XmlElement("KeywordStatisticsSearchResult")]
		public KeywordStatisticsSearchResult KeywordStatisticsSearchResult
		{
			get
			{
				return this.keywordStatisticsSearchResultField;
			}
			set
			{
				this.keywordStatisticsSearchResultField = value;
			}
		}

		private UserMailbox userMailboxField;

		private KeywordStatisticsSearchResult keywordStatisticsSearchResultField;
	}
}

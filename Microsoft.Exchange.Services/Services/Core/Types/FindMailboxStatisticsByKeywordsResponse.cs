using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "FindMailboxStatisticsByKeywordsResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindMailboxStatisticsByKeywordsResponse : BaseResponseMessage
	{
		public FindMailboxStatisticsByKeywordsResponse() : base(ResponseType.FindMailboxStatisticsByKeywordsResponseMessage)
		{
		}

		internal void AddResponses(UserMailbox userMailbox, ServiceResult<KeywordStatisticsSearchResult>[] serviceResults)
		{
			ServiceResult<KeywordStatisticsSearchResult>.ProcessServiceResults(serviceResults, delegate(ServiceResult<KeywordStatisticsSearchResult> result)
			{
				if (result != null)
				{
					this.AddResponse(new FindMailboxStatisticsByKeywordsResponseMessage(result.Code, result.Error, userMailbox, result.Value));
				}
			});
		}
	}
}

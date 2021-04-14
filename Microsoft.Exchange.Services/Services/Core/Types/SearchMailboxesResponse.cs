using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "SearchMailboxesResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SearchMailboxesResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SearchMailboxesResponse : BaseResponseMessage
	{
		public SearchMailboxesResponse() : base(ResponseType.SearchMailboxesResponseMessage)
		{
		}

		internal void AddResponses(ServiceResult<SearchMailboxesResult>[] serviceResults)
		{
			ServiceResult<SearchMailboxesResult>.ProcessServiceResults(serviceResults, delegate(ServiceResult<SearchMailboxesResult> result)
			{
				if (result != null)
				{
					base.AddResponse(new SearchMailboxesResponseMessage(result.Code, result.Error, result.Value));
				}
			});
		}
	}
}

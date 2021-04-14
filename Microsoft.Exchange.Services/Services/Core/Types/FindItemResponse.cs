using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("FindItemResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class FindItemResponse : BaseResponseMessage
	{
		public FindItemResponse() : base(ResponseType.FindItemResponseMessage)
		{
		}

		internal static FindItemResponse CreateResponse()
		{
			return new FindItemResponse();
		}

		internal static BaseResponseMessage CreateResponseForFindItem(ServiceResult<FindItemParentWrapper>[] serviceResults, FindItemResponse.CreateFindItemResponse createResponse, HighlightTermType[] highlightTerms, bool isSearchInProgress, FolderId searchFolderId)
		{
			FindItemResponse findItemResponse = createResponse();
			findItemResponse.highlightTerms = highlightTerms;
			findItemResponse.isSearchInProgress = isSearchInProgress;
			findItemResponse.searchFolderId = searchFolderId;
			ServiceResult<FindItemParentWrapper>.ProcessServiceResults(serviceResults, new ProcessServiceResult<FindItemParentWrapper>(findItemResponse.ProcessServiceResult));
			return findItemResponse;
		}

		internal void ProcessServiceResult(ServiceResult<FindItemParentWrapper> result)
		{
			base.AddResponse(new FindItemResponseMessage(result.Code, result.Error, result.Value, this.highlightTerms, this.isSearchInProgress, this.searchFolderId));
		}

		private HighlightTermType[] highlightTerms;

		private bool isSearchInProgress;

		private FolderId searchFolderId;

		internal delegate FindItemResponse CreateFindItemResponse();
	}
}

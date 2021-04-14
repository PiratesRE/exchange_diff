using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("FindFolderResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindFolderResponse : BaseResponseMessage
	{
		public FindFolderResponse() : base(ResponseType.FindFolderResponseMessage)
		{
		}

		internal static FindFolderResponse CreateResponse()
		{
			return new FindFolderResponse();
		}

		internal static BaseResponseMessage CreateResponseForFindFolder(ServiceResult<FindFolderParentWrapper>[] serviceResults, FindFolderResponse.CreateFindFolderResponse createResponse)
		{
			FindFolderResponse findFolderResponse = createResponse();
			ServiceResult<FindFolderParentWrapper>.ProcessServiceResults(serviceResults, new ProcessServiceResult<FindFolderParentWrapper>(findFolderResponse.ProcessServiceResult));
			return findFolderResponse;
		}

		internal void ProcessServiceResult(ServiceResult<FindFolderParentWrapper> result)
		{
			base.AddResponse(new FindFolderResponseMessage(result.Code, result.Error, result.Value));
		}

		internal delegate FindFolderResponse CreateFindFolderResponse();
	}
}

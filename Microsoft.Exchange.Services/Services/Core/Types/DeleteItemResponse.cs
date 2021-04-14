using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DeleteItemResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DeleteItemResponse : BaseResponseMessage
	{
		public DeleteItemResponse() : base(ResponseType.DeleteItemResponseMessage)
		{
		}

		internal void AddResponses(ServiceResult<DeleteItemResponseMessage>[] results)
		{
			ServiceResult<DeleteItemResponseMessage>.ProcessServiceResults(results, delegate(ServiceResult<DeleteItemResponseMessage> result)
			{
				base.AddResponse(result.Value ?? new DeleteItemResponseMessage(result.Code, result.Error));
			});
		}
	}
}

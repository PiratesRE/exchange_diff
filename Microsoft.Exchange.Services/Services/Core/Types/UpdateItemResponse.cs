using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("UpdateItemResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UpdateItemResponse : ItemInfoResponse
	{
		public UpdateItemResponse() : base(ResponseType.UpdateItemResponseMessage)
		{
		}

		public UpdateItemResponse(ResponseType responseType) : base(responseType)
		{
		}

		internal void BuildForUpdateItemResults(ServiceResult<UpdateItemResponseWrapper>[] serviceResults)
		{
			ServiceResult<UpdateItemResponseWrapper>.ProcessServiceResults(serviceResults, new ProcessServiceResult<UpdateItemResponseWrapper>(this.ProcessServiceResult));
		}

		internal virtual void ProcessServiceResult(ServiceResult<UpdateItemResponseWrapper> result)
		{
			base.AddResponse(this.CreateUpdateItemResponseMessage(result.Code, result.Error, result.Value));
		}

		internal ResponseMessage CreateUpdateItemResponseMessage(ServiceResultCode code, ServiceError error, UpdateItemResponseWrapper value)
		{
			ConflictResults conflictResults = null;
			ItemType item = null;
			if (value != null)
			{
				item = value.Item;
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
				{
					conflictResults = value.ConflictResults;
				}
			}
			return new UpdateItemResponseMessage(code, error, item, conflictResults);
		}
	}
}

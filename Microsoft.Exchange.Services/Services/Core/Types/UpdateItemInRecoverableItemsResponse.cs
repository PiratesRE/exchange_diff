using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UpdateItemInRecoverableItemsResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateItemInRecoverableItemsResponse : ItemInfoResponse
	{
		public UpdateItemInRecoverableItemsResponse() : base(ResponseType.UpdateItemInRecoverableItemsResponseMessage)
		{
		}

		internal void BuildForUpdateItemInRecoverableItemsResults(ServiceResult<UpdateItemInRecoverableItemsResponseWrapper>[] serviceResults)
		{
			ServiceResult<UpdateItemInRecoverableItemsResponseWrapper>.ProcessServiceResults(serviceResults, new ProcessServiceResult<UpdateItemInRecoverableItemsResponseWrapper>(this.ProcessServiceResult));
		}

		internal virtual void ProcessServiceResult(ServiceResult<UpdateItemInRecoverableItemsResponseWrapper> result)
		{
			base.AddResponse(this.CreateUpdateItemInRecoverableItemsResponseMessage(result.Code, result.Error, result.Value));
		}

		internal ResponseMessage CreateUpdateItemInRecoverableItemsResponseMessage(ServiceResultCode code, ServiceError error, UpdateItemInRecoverableItemsResponseWrapper value)
		{
			ConflictResults conflictResults = null;
			ItemType item = null;
			AttachmentType[] attachments = null;
			if (value != null)
			{
				item = value.Item;
				attachments = value.Attachments;
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
				{
					conflictResults = value.ConflictResults;
				}
			}
			return new UpdateItemInRecoverableItemsResponseMessage(code, error, item, attachments, conflictResults);
		}
	}
}

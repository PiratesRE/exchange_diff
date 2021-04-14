using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ExportItemsResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ExportItemsResponse : BaseInfoResponse
	{
		public ExportItemsResponse() : base(ResponseType.ExportItemsResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue value)
		{
			return new ExportItemsResponseMessage(code, error, value as XmlNode);
		}

		internal void BuildForExportItemsResults(ServiceResult<ExportItemsResponseMessage>[] serviceResults)
		{
			ServiceResult<ExportItemsResponseMessage>.ProcessServiceResults(serviceResults, delegate(ServiceResult<ExportItemsResponseMessage> serviceResult)
			{
				if (serviceResult.Value == null)
				{
					base.AddResponse(this.CreateResponseMessage<ExportItemsResponseMessage>(serviceResult.Code, serviceResult.Error, null));
					return;
				}
				base.AddResponse(serviceResult.Value);
			});
		}
	}
}

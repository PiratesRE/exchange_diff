using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("CreateAttachmentResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateAttachmentResponse : AttachmentInfoResponse
	{
		public CreateAttachmentResponse() : base(ResponseType.CreateAttachmentResponseMessage)
		{
		}

		internal void AddResponses(ServiceResult<AttachmentType>[] serviceResults)
		{
			ServiceResult<AttachmentType>.ProcessServiceResults(serviceResults, delegate(ServiceResult<AttachmentType> result)
			{
				base.AddResponse(new AttachmentInfoResponseMessage(result.Code, result.Error, result.Value));
			});
		}
	}
}

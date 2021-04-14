using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetAttachmentResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetAttachmentResponse : AttachmentInfoResponse
	{
		public GetAttachmentResponse() : base(ResponseType.GetAttachmentResponseMessage)
		{
		}

		internal void BuildForGetAttachmentResults(ServiceResult<AttachmentInfoResponseMessage>[] serviceResults)
		{
			ServiceResult<AttachmentInfoResponseMessage>.ProcessServiceResults(serviceResults, delegate(ServiceResult<AttachmentInfoResponseMessage> serviceResult)
			{
				if (serviceResult.Value == null)
				{
					base.AddResponse(new AttachmentInfoResponseMessage(serviceResult.Code, serviceResult.Error, null));
					return;
				}
				base.AddResponse(serviceResult.Value);
			});
		}
	}
}

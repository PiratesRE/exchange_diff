using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateAttachmentResponse : CreateEntityResponse<Attachment>
	{
		public CreateAttachmentResponse(CreateAttachmentRequest request) : base(request)
		{
		}
	}
}

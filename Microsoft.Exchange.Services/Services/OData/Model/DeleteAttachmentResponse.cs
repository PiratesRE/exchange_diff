using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteAttachmentResponse : DeleteEntityResponse<Attachment>
	{
		public DeleteAttachmentResponse(DeleteAttachmentRequest request) : base(request)
		{
		}
	}
}

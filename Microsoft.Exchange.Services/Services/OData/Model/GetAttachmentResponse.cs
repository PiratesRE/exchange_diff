using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetAttachmentResponse : GetEntityResponse<Attachment>
	{
		public GetAttachmentResponse(GetAttachmentRequest request) : base(request)
		{
		}
	}
}

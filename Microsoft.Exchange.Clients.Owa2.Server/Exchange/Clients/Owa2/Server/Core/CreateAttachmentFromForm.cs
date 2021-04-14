using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateAttachmentFromForm : ServiceCommand<CreateAttachmentResponse>
	{
		public CreateAttachmentFromForm(CallContext callContext, HttpRequest request) : base(callContext)
		{
			HttpPostedFile httpPostedFile = request.Files[0];
			string fileName = Path.GetFileName(httpPostedFile.FileName);
			this.translatedRequest = CreateAttachmentHelper.CreateAttachmentRequest(new ItemId(request.Form["parentItemId"], request.Form["parentChangeKey"]), fileName, httpPostedFile.ContentLength, httpPostedFile.ContentType, CreateAttachmentHelper.GetContentBytes(httpPostedFile.InputStream), bool.Parse(request.Form["isInline"]), request.Form["cancellationId"]);
			this.fileSize = httpPostedFile.ContentLength;
			this.contentType = httpPostedFile.ContentType;
		}

		protected override CreateAttachmentResponse InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			CreateAttachmentResponse createAttachmentResponse;
			if (this.translatedRequest.CancellationId != null && userContext.CancelAttachmentManager.OnCreateAttachment(this.translatedRequest.CancellationId, null))
			{
				createAttachmentResponse = CreateAttachmentHelper.BuildCreateAttachmentResponseForCancelled();
			}
			else
			{
				createAttachmentResponse = CreateAttachmentHelper.CreateAttachment(base.CallContext, this.translatedRequest);
				if (createAttachmentResponse != null && createAttachmentResponse.ResponseMessages != null && createAttachmentResponse.ResponseMessages.Items != null && createAttachmentResponse.ResponseMessages.Items.Length == 1 && createAttachmentResponse.ResponseMessages.Items[0].ResponseCode == ResponseCodeType.NoError)
				{
					((AttachmentInfoResponseMessage)createAttachmentResponse.ResponseMessages.Items[0]).Attachments[0].Size = this.fileSize;
					((AttachmentInfoResponseMessage)createAttachmentResponse.ResponseMessages.Items[0]).Attachments[0].ContentType = this.contentType;
				}
				if (this.translatedRequest.CancellationId != null)
				{
					AttachmentIdType attachmentIdFromCreateAttachmentResponse = CreateAttachmentHelper.GetAttachmentIdFromCreateAttachmentResponse(createAttachmentResponse);
					userContext.CancelAttachmentManager.CreateAttachmentCompleted(this.translatedRequest.CancellationId, attachmentIdFromCreateAttachmentResponse);
				}
			}
			return createAttachmentResponse;
		}

		private const string IsInline = "isInline";

		private readonly int fileSize;

		private readonly string contentType;

		private CreateAttachmentRequest translatedRequest;
	}
}

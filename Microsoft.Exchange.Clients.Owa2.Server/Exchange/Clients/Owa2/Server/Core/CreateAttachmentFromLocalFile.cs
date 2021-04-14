using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateAttachmentFromLocalFile : ServiceCommand<CreateAttachmentResponse>
	{
		public CreateAttachmentFromLocalFile(CallContext callContext, CreateAttachmentRequest request) : base(callContext)
		{
			this.request = request;
		}

		protected override CreateAttachmentResponse InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			CreateAttachmentResponse createAttachmentResponse;
			if (this.request.CancellationId != null && userContext.CancelAttachmentManager.OnCreateAttachment(this.request.CancellationId, null))
			{
				createAttachmentResponse = CreateAttachmentHelper.BuildCreateAttachmentResponseForCancelled();
			}
			else
			{
				createAttachmentResponse = CreateAttachmentFromLocalFile.CreateAttachment(this.request);
				if (this.request.CancellationId != null)
				{
					AttachmentIdType attachmentIdFromCreateAttachmentResponse = CreateAttachmentHelper.GetAttachmentIdFromCreateAttachmentResponse(createAttachmentResponse);
					userContext.CancelAttachmentManager.CreateAttachmentCompleted(this.request.CancellationId, attachmentIdFromCreateAttachmentResponse);
				}
			}
			return createAttachmentResponse;
		}

		public static CreateAttachmentResponse CreateAttachment(CreateAttachmentRequest request)
		{
			CreateAttachmentJsonRequest createAttachmentJsonRequest = new CreateAttachmentJsonRequest();
			createAttachmentJsonRequest.Body = request;
			OWAService owaservice = new OWAService();
			IAsyncResult asyncResult = owaservice.BeginCreateAttachment(createAttachmentJsonRequest, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			return owaservice.EndCreateAttachment(asyncResult).Body;
		}

		private CreateAttachmentRequest request;
	}
}

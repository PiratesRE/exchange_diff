using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetWacIframeUrl : ServiceCommand<string>
	{
		public GetWacIframeUrl(CallContext callContext, string attachmentId) : base(callContext)
		{
			this.attachmentId = attachmentId;
		}

		protected override string InternalExecute()
		{
			string wacUrl;
			using (AttachmentHandler.IAttachmentRetriever attachmentRetriever = AttachmentRetriever.CreateInstance(this.attachmentId, base.CallContext))
			{
				StoreSession session = attachmentRetriever.RootItem.Session;
				Item rootItem = attachmentRetriever.RootItem;
				Attachment attachment = attachmentRetriever.Attachment;
				WacAttachmentType wacAttachmentType = GetWacAttachmentInfo.Execute(base.CallContext, session, rootItem, attachment, null, this.attachmentId, false);
				if (wacAttachmentType == null)
				{
					throw new OwaInvalidOperationException("There is no reason known for code to reach here without throwing an unhandled exception elsewhere");
				}
				wacUrl = wacAttachmentType.WacUrl;
			}
			return wacUrl;
		}

		private readonly string attachmentId;
	}
}

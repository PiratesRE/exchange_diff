using System;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAllAttachmentsAsZip : ServiceCommand<Stream>
	{
		public GetAllAttachmentsAsZip(CallContext callContext, string id) : base(callContext)
		{
			if (callContext.HttpContext == null)
			{
				throw new ArgumentNullException("callContext.HttpContext cannot be null");
			}
			this.webOperationContext = new AttachmentWebOperationContext(callContext.HttpContext, callContext.CreateWebResponseContext());
			this.id = id;
		}

		protected override Stream InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(HttpContext.Current, CallContext.Current.EffectiveCaller, true);
			ConfigurationContext configurationContext = new ConfigurationContext(userContext);
			AttachmentHandler attachmentHandler = new AttachmentHandler(this.id, this.webOperationContext, base.CallContext, configurationContext);
			Stream result;
			try
			{
				using (AttachmentHandler.IAttachmentRetriever attachmentRetriever = AttachmentRetriever.CreateInstance(this.id, base.CallContext))
				{
					Stream attachmentStream = attachmentHandler.GetAllAttachmentsAsZipStream(attachmentRetriever);
					base.CallContext.OnDisposed += delegate(object sender, EventArgs args)
					{
						if (attachmentStream != null)
						{
							attachmentStream.Dispose();
						}
					};
					result = attachmentStream;
				}
			}
			catch (CannotOpenFileAttachmentException)
			{
				this.webOperationContext.StatusCode = HttpStatusCode.NotFound;
				result = null;
			}
			catch (ObjectNotFoundException)
			{
				this.webOperationContext.StatusCode = HttpStatusCode.NotFound;
				result = null;
			}
			return result;
		}

		private readonly string id;

		private AttachmentWebOperationContext webOperationContext;
	}
}

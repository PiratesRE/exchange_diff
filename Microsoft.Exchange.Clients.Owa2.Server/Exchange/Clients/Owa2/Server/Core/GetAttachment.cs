using System;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAttachment : ServiceCommand<Stream>
	{
		public GetAttachment(CallContext callContext, string id, bool isImagePreview, bool asDataUri) : base(callContext)
		{
			if (callContext.HttpContext == null)
			{
				throw new ArgumentNullException("callContext.HttpContext cannot be null");
			}
			this.webOperationContext = new AttachmentWebOperationContext(callContext.HttpContext, callContext.CreateWebResponseContext());
			this.id = id;
			this.isImagePreview = isImagePreview;
			this.asDataUri = asDataUri;
			OwsLogRegistry.Register("GetFileAttachment", typeof(GetAttachmentMetadata), new Type[0]);
		}

		protected override Stream InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(HttpContext.Current, CallContext.Current.EffectiveCaller, true);
			ConfigurationContext configurationContext = new ConfigurationContext(userContext);
			string mailboxSmtpAddress = userContext.MailboxIdentity.PrimarySmtpAddress.ToString();
			CobaltStoreSaver cobaltStoreSaver;
			if (WacUtilities.ShouldUpdateAttachment(mailboxSmtpAddress, this.id, out cobaltStoreSaver))
			{
				base.CallContext.ProtocolLog.Set(GetAttachmentMetadata.Updated, true);
				cobaltStoreSaver.SaveAndLogExceptions(base.CallContext.ProtocolLog);
			}
			else
			{
				base.CallContext.ProtocolLog.Set(GetAttachmentMetadata.Updated, false);
			}
			AttachmentHandler attachmentHandler = new AttachmentHandler(this.id, this.webOperationContext, base.CallContext, configurationContext);
			attachmentHandler.IsImagePreview = this.isImagePreview;
			Stream result;
			try
			{
				using (AttachmentHandler.IAttachmentRetriever attachmentRetriever = AttachmentRetriever.CreateInstance(this.id, base.CallContext))
				{
					AttachmentHandler.IAttachmentPolicyChecker policyChecker = AttachmentPolicyChecker.CreateInstance(configurationContext.AttachmentPolicy);
					Stream attachmentStream = attachmentHandler.GetAttachmentStream(attachmentRetriever, policyChecker, this.asDataUri);
					GetAttachment.EliminateGzFileDoubleCompression(attachmentRetriever);
					base.CallContext.OnDisposed += delegate(object sender, EventArgs args)
					{
						if (attachmentStream != null)
						{
							attachmentStream.Dispose();
						}
					};
					if (attachmentRetriever.Attachment != null)
					{
						base.CallContext.ProtocolLog.Set(GetAttachmentMetadata.Extension, attachmentRetriever.Attachment.FileExtension);
						base.CallContext.ProtocolLog.Set(GetAttachmentMetadata.Length, attachmentRetriever.Attachment.Size);
					}
					result = attachmentStream;
				}
			}
			catch (InvalidStoreIdException innerException)
			{
				throw new OwaInvalidRequestException("Invalid ID, " + this.GetParametersForLogging(), innerException);
			}
			catch (InvalidIdMalformedException innerException2)
			{
				throw new OwaInvalidRequestException("Malformed ID, " + this.GetParametersForLogging(), innerException2);
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

		private static void EliminateGzFileDoubleCompression(AttachmentHandler.IAttachmentRetriever attachmentRetriever)
		{
			if (attachmentRetriever != null && attachmentRetriever.Attachment != null && attachmentRetriever.Attachment.FileExtension != null && attachmentRetriever.Attachment.FileExtension.ToLower().Equals(".gz") && attachmentRetriever.Attachment.ContentType != null && attachmentRetriever.Attachment.ContentType.ToLower().Contains("application/x-gzip"))
			{
				string text = HttpContext.Current.Request.Headers["Accept-Encoding"];
				if (!string.IsNullOrEmpty(text))
				{
					HttpContext.Current.Request.Headers["Accept-Encoding"] = text.Replace("gzip", "");
				}
			}
		}

		private string GetParametersForLogging()
		{
			string text;
			int num;
			if (this.id == null)
			{
				text = "(null)";
				num = 0;
			}
			else if (this.id == string.Empty)
			{
				text = "(empty)";
				num = 0;
			}
			else
			{
				text = "'" + this.id + "'";
				try
				{
					byte[] array = Convert.FromBase64String(this.id);
					text = BitConverter.ToString(array);
					num = array.Length;
				}
				catch (FormatException ex)
				{
					text = text + " " + ex.Message;
					num = this.id.Length;
				}
			}
			return string.Format("Preview: {0}, DataUri: {1}, ID: {2}, Length={3}", new object[]
			{
				this.isImagePreview,
				this.asDataUri,
				text,
				num
			});
		}

		private const string GetAttachmentActionName = "GetFileAttachment";

		private readonly string id;

		private readonly bool isImagePreview;

		private readonly bool asDataUri;

		private AttachmentWebOperationContext webOperationContext;
	}
}

using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class UploadPhotoFromForm : ServiceCommand<UploadPhotoResponse>
	{
		public UploadPhotoFromForm(CallContext callContext, HttpRequest request) : base(callContext)
		{
			this.ValidateRequest(request);
			this.fileStream = request.Files[0].InputStream;
		}

		protected override UploadPhotoResponse InternalExecute()
		{
			PhotoRequest request = new PhotoRequest
			{
				TargetPrimarySmtpAddress = base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(),
				UploadTo = base.CallContext.AccessingPrincipal.ObjectId,
				Preview = true,
				UploadCommand = UploadCommand.Upload,
				RawUploadedPhoto = this.fileStream
			};
			new PhotoUploadPipeline(UploadPhotoFromForm.PhotosConfiguration, base.MailboxIdentityMailboxSession, base.CallContext.ADRecipientSessionContext.GetADRecipientSession(), ExTraceGlobals.UserPhotosTracer).Upload(request, Stream.Null);
			return new UploadPhotoResponse();
		}

		private void ValidateRequest(HttpRequest request)
		{
			UploadPhotoCommand uploadPhotoCommand;
			if (!Enum.TryParse<UploadPhotoCommand>(request.Form["UploadPhotoCommand"], true, out uploadPhotoCommand))
			{
				throw FaultExceptionUtilities.CreateFault(new OwaInvalidRequestException(), FaultParty.Sender);
			}
			if (uploadPhotoCommand != UploadPhotoCommand.UploadPreview || request.Files.Count == 0 || request.Files[0].InputStream == null)
			{
				throw FaultExceptionUtilities.CreateFault(new OwaInvalidRequestException(), FaultParty.Sender);
			}
		}

		private const string UploadPhotoCommandFieldName = "UploadPhotoCommand";

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private readonly Stream fileStream;
	}
}

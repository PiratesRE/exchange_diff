using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "UserPhoto", DefaultParameterSetName = "UploadPhotoData", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetUserPhoto : SetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "UploadPhotoStream", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "SavePhoto", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "UploadPreview", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "CancelPhoto", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "UploadPhotoData", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UploadPhotoStream")]
		[Parameter(ParameterSetName = "UploadPreview")]
		[ValidateNotNull]
		public Stream PictureStream
		{
			get
			{
				return (Stream)base.Fields["PictureStream"];
			}
			set
			{
				base.Fields["PictureStream"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(ParameterSetName = "UploadPreview")]
		[Parameter(Mandatory = true, ParameterSetName = "UploadPhotoData")]
		public byte[] PictureData
		{
			get
			{
				return (byte[])base.Fields["PictureData"];
			}
			set
			{
				base.Fields["PictureData"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CancelPhoto")]
		public SwitchParameter Cancel
		{
			get
			{
				return (SwitchParameter)(base.Fields["Cancel"] ?? false);
			}
			set
			{
				base.Fields["Cancel"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SavePhoto")]
		public SwitchParameter Save
		{
			get
			{
				return (SwitchParameter)(base.Fields["Save"] ?? false);
			}
			set
			{
				base.Fields["Save"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UploadPreview")]
		public SwitchParameter Preview
		{
			get
			{
				return (SwitchParameter)(base.Fields["Preview"] ?? false);
			}
			set
			{
				base.Fields["Preview"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.Save)
				{
					return Strings.ConfirmationMessageSaveUserPhoto(this.Identity.ToString());
				}
				if (this.Cancel)
				{
					return Strings.ConfirmationMessageCancelUserPhoto(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageUploadUserPhoto(this.Identity.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			this.tracer = new PhotoCmdletTracer(base.IsVerboseOn);
			base.InternalBeginProcessing();
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable configurable = base.PrepareDataObject();
			CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, (ADUser)configurable, false, this.ConfirmationMessage, null);
			return configurable;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(this.DataObject, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Set-UserPhoto"))
				{
					PhotoRequest request = this.CreateRequest(exchangePrincipal);
					PhotoUploadPipeline photoUploadPipeline = new PhotoUploadPipeline(SetUserPhoto.PhotosConfiguration, mailboxSession, (IRecipientSession)base.DataSession, this.tracer);
					photoUploadPipeline.Upload(request, Stream.Null);
					if (!this.Save && !this.Cancel && !this.Preview)
					{
						photoUploadPipeline.Upload(this.CreateSavePreviewRequest(exchangePrincipal), Stream.Null);
					}
				}
			}
			catch (WrongServerException ex)
			{
				this.LogFailedToUploadPhotoEvent(ex);
				this.WriteError(new CannotModifyPhotoBecauseMailboxIsInTransitException(ex), ExchangeErrorCategory.ServerTransient, this.DataObject, true);
			}
			catch (Exception e)
			{
				this.LogFailedToUploadPhotoEvent(e);
				throw;
			}
			finally
			{
				this.tracer.Dump(new PhotoRequestLogWriter(SetUserPhoto.RequestLog, SetUserPhoto.GenerateRequestId()));
			}
			TaskLogger.LogExit();
		}

		public override object GetDynamicParameters()
		{
			return null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(CannotMapInvalidSmtpAddressToPhotoFileException).IsInstanceOfType(exception) || typeof(UserPhotoProcessingException).IsInstanceOfType(exception) || typeof(UserPhotoNotFoundException).IsInstanceOfType(exception) || typeof(ObjectNotFoundException).IsInstanceOfType(exception) || typeof(IOException).IsInstanceOfType(exception) || typeof(UnauthorizedAccessException).IsInstanceOfType(exception) || typeof(Win32Exception).IsInstanceOfType(exception) || typeof(ADNoSuchObjectException).IsInstanceOfType(exception) || typeof(ADOperationException).IsInstanceOfType(exception) || typeof(StoragePermanentException).IsInstanceOfType(exception) || typeof(StorageTransientException).IsInstanceOfType(exception);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!this.Save && !this.Cancel)
			{
				bool flag = this.PictureStream == null || this.PictureStream.Length == 0L;
				bool flag2 = this.PictureData == null || this.PictureData.Length == 0;
				if (flag && flag2)
				{
					this.WriteError(new PhotoMustNotBeBlankException(), ExchangeErrorCategory.Client, this.Identity, false);
				}
			}
		}

		private PhotoRequest CreateRequest(ExchangePrincipal principal)
		{
			if (this.Save)
			{
				return this.CreateSavePreviewRequest(principal);
			}
			if (this.Cancel)
			{
				return new PhotoRequest
				{
					TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
					UploadTo = principal.ObjectId,
					Preview = true,
					UploadCommand = UploadCommand.Clear
				};
			}
			return new PhotoRequest
			{
				TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
				UploadTo = principal.ObjectId,
				Preview = true,
				UploadCommand = UploadCommand.Upload,
				RawUploadedPhoto = ((this.PictureStream == null) ? new MemoryStream(this.PictureData) : this.PictureStream)
			};
		}

		private PhotoRequest CreateSavePreviewRequest(ExchangePrincipal principal)
		{
			return new PhotoRequest
			{
				TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
				UploadTo = principal.ObjectId,
				Preview = false,
				UploadCommand = UploadCommand.Upload
			};
		}

		private void LogFailedToUploadPhotoEvent(Exception e)
		{
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_FailedToUploadPhoto, new string[]
			{
				this.DataObject.ToString(),
				this.DataObject.UserPrincipalName,
				e.ToString()
			});
		}

		private static string GenerateRequestId()
		{
			return RandomPhotoRequestIdGenerator.Generate();
		}

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static readonly PhotoRequestLog RequestLog = new PhotoRequestLogFactory(SetUserPhoto.PhotosConfiguration, ExchangeSetupContext.InstalledVersion.ToString()).Create();

		private PhotoCmdletTracer tracer;

		private static class ParameterSet
		{
			internal const string CancelPhoto = "CancelPhoto";

			internal const string SavePhoto = "SavePhoto";

			internal const string UploadPhotoData = "UploadPhotoData";

			internal const string UploadPhotoStream = "UploadPhotoStream";

			internal const string UploadPreview = "UploadPreview";
		}
	}
}

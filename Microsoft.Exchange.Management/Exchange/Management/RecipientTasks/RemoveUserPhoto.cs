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
	[Cmdlet("Remove", "UserPhoto", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveUserPhoto : RemoveRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemovePhoto(this.Identity.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			this.tracer = new PhotoCmdletTracer(base.IsVerboseOn);
			base.InternalBeginProcessing();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, base.DataObject, false, this.ConfirmationMessage, null);
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(base.DataObject, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Remove-UserPhoto"))
				{
					new PhotoRemovalPipeline(RemoveUserPhoto.PhotosConfiguration, mailboxSession, (IRecipientSession)base.DataSession, this.tracer).Upload(this.CreateRemovePhotoRequest(exchangePrincipal), Stream.Null);
				}
			}
			catch (WrongServerException ex)
			{
				this.LogFailedToRemovePhotoEvent(ex);
				this.WriteError(new CannotModifyPhotoBecauseMailboxIsInTransitException(ex), ExchangeErrorCategory.ServerTransient, base.DataObject, true);
			}
			catch (Exception e)
			{
				this.LogFailedToRemovePhotoEvent(e);
				throw;
			}
			finally
			{
				this.tracer.Dump(new PhotoRequestLogWriter(RemoveUserPhoto.RequestLog, RemoveUserPhoto.GenerateRequestId()));
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(CannotMapInvalidSmtpAddressToPhotoFileException).IsInstanceOfType(exception) || typeof(AggregateOperationFailedException).IsInstanceOfType(exception) || typeof(ObjectNotFoundException).IsInstanceOfType(exception) || typeof(IOException).IsInstanceOfType(exception) || typeof(UnauthorizedAccessException).IsInstanceOfType(exception) || typeof(Win32Exception).IsInstanceOfType(exception) || typeof(ADNoSuchObjectException).IsInstanceOfType(exception) || typeof(ADOperationException).IsInstanceOfType(exception) || typeof(StoragePermanentException).IsInstanceOfType(exception) || typeof(StorageTransientException).IsInstanceOfType(exception);
		}

		private PhotoRequest CreateRemovePhotoRequest(ExchangePrincipal principal)
		{
			return new PhotoRequest
			{
				Preview = false,
				UploadCommand = UploadCommand.Clear,
				UploadTo = principal.ObjectId,
				TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString()
			};
		}

		private void LogFailedToRemovePhotoEvent(Exception e)
		{
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_FailedToRemovePhoto, new string[]
			{
				base.DataObject.ToString(),
				base.DataObject.UserPrincipalName,
				e.ToString()
			});
		}

		private static string GenerateRequestId()
		{
			return RandomPhotoRequestIdGenerator.Generate();
		}

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static readonly PhotoRequestLog RequestLog = new PhotoRequestLogFactory(RemoveUserPhoto.PhotosConfiguration, ExchangeSetupContext.InstalledVersion.ToString()).Create();

		private PhotoCmdletTracer tracer;
	}
}

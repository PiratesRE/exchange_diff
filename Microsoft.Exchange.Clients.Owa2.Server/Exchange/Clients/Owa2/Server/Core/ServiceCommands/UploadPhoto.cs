using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class UploadPhoto : ServiceCommand<UploadPhotoResponse>
	{
		public UploadPhoto(CallContext callContext, UploadPhotoRequest uploadPhotoRequest) : base(callContext)
		{
			this.uploadPhotoRequest = uploadPhotoRequest;
			if (string.IsNullOrEmpty(this.uploadPhotoRequest.EmailAddress))
			{
				throw FaultExceptionUtilities.CreateFault(new OwaInvalidRequestException(), FaultParty.Sender);
			}
			this.adRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId), 60, ".ctor", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\ServiceCommands\\UploadPhoto.cs");
		}

		protected override UploadPhotoResponse InternalExecute()
		{
			DisposeGuard disposeGuard = default(DisposeGuard);
			Action action = null;
			ExchangePrincipal exchangePrincipal;
			MailboxSession mailboxSession;
			if (this.IsRequestForCurrentUser())
			{
				exchangePrincipal = base.CallContext.AccessingPrincipal;
				mailboxSession = base.CallContext.SessionCache.GetMailboxSessionBySmtpAddress(this.uploadPhotoRequest.EmailAddress);
			}
			else
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse(this.uploadPhotoRequest.EmailAddress);
				ADUser groupAdUser = this.adRecipientSession.FindByProxyAddress(proxyAddress) as ADUser;
				if (groupAdUser == null)
				{
					throw FaultExceptionUtilities.CreateFault(new OwaInvalidRequestException(), FaultParty.Sender);
				}
				if (groupAdUser.RecipientTypeDetails != RecipientTypeDetails.GroupMailbox || !this.IsOwnedModernGroup(groupAdUser))
				{
					OwaInvalidOperationException exception = new OwaInvalidOperationException(string.Format("User does not have sufficient privileges on {0}", this.uploadPhotoRequest.EmailAddress));
					throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
				}
				if (groupAdUser.IsCached)
				{
					this.adRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(groupAdUser.OriginatingServer, false, ConsistencyMode.IgnoreInvalid, this.adRecipientSession.SessionSettings, 102, "InternalExecute", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\ServiceCommands\\UploadPhoto.cs");
				}
				exchangePrincipal = ExchangePrincipal.FromADUser(groupAdUser, null);
				mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, base.CallContext.ClientCulture, "Client=OWA;Action=GroupPhotoUpload");
				action = delegate()
				{
					DirectorySessionFactory.Default.GetTenantOrRootRecipientReadOnlySession(this.adRecipientSession, groupAdUser.OriginatingServer, 125, "InternalExecute", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\ServiceCommands\\UploadPhoto.cs").FindByProxyAddress(proxyAddress);
				};
				disposeGuard.Add<MailboxSession>(mailboxSession);
			}
			using (disposeGuard)
			{
				PhotoRequest request = this.CreateRequest(exchangePrincipal);
				new PhotoUploadPipeline(UploadPhoto.PhotosConfiguration, mailboxSession, this.adRecipientSession, ExTraceGlobals.UserPhotosTracer).Upload(request, Stream.Null);
				if (action != null)
				{
					action();
				}
			}
			return new UploadPhotoResponse();
		}

		private bool IsRequestForCurrentUser()
		{
			return StringComparer.OrdinalIgnoreCase.Compare(this.uploadPhotoRequest.EmailAddress, base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString()) == 0;
		}

		private bool IsOwnedModernGroup(ADUser groupMailboxAdUser)
		{
			return groupMailboxAdUser.Owners.Contains(base.CallContext.AccessingPrincipal.ObjectId);
		}

		private PhotoRequest CreateRequest(IExchangePrincipal principal)
		{
			switch (this.uploadPhotoRequest.Command)
			{
			case UploadPhotoCommand.UploadPhoto:
				return new PhotoRequest
				{
					TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
					UploadTo = principal.ObjectId,
					Preview = false,
					UploadCommand = UploadCommand.Upload
				};
			case UploadPhotoCommand.UploadPreview:
				return new PhotoRequest
				{
					TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
					UploadTo = principal.ObjectId,
					Preview = true,
					UploadCommand = UploadCommand.Upload,
					RawUploadedPhoto = new MemoryStream(Convert.FromBase64String(this.uploadPhotoRequest.Content))
				};
			case UploadPhotoCommand.ClearPhoto:
				return new PhotoRequest
				{
					Preview = false,
					UploadCommand = UploadCommand.Clear,
					UploadTo = principal.ObjectId,
					TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString()
				};
			case UploadPhotoCommand.ClearPreview:
				return new PhotoRequest
				{
					TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
					UploadTo = principal.ObjectId,
					Preview = true,
					UploadCommand = UploadCommand.Clear
				};
			default:
				throw FaultExceptionUtilities.CreateFault(new OwaInvalidRequestException(), FaultParty.Sender);
			}
		}

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private readonly UploadPhotoRequest uploadPhotoRequest;

		private IRecipientSession adRecipientSession;
	}
}

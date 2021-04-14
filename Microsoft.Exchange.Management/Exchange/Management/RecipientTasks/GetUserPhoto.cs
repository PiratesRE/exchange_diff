using System;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "UserPhoto", DefaultParameterSetName = "Identity")]
	public sealed class GetUserPhoto : GetRecipientBase<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = false)]
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

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return RecipientConstants.GetMailboxOrSyncMailbox_AllowedRecipientTypeDetails;
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetUserPhoto.SortPropertiesArray;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return new MailboxSchema();
			}
		}

		protected override void InternalBeginProcessing()
		{
			this.tracer = new PhotoCmdletTracer(base.IsVerboseOn);
			base.InternalBeginProcessing();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser aduser = (ADUser)dataObject;
			UserPhotoConfiguration userPhotoConfiguration = new UserPhotoConfiguration(dataObject.Identity, Stream.Null, null);
			if (CmdletProxy.TryToProxyOutputObject(userPhotoConfiguration, base.CurrentTaskContext, aduser, this.Identity == null, this.ConfirmationMessage, CmdletProxy.AppendIdentityToProxyCmdlet(aduser)))
			{
				return userPhotoConfiguration;
			}
			IConfigurable result;
			try
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(aduser, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Get-UserPhoto"))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						PhotoManagementRetrievalPipeline photoManagementRetrievalPipeline = new PhotoManagementRetrievalPipeline(GetUserPhoto.PhotosConfiguration, mailboxSession, (IRecipientSession)base.DataSession, this.tracer);
						PhotoResponse photoResponse = photoManagementRetrievalPipeline.Retrieve(this.CreateRetrievePhotoRequest(exchangePrincipal), memoryStream);
						HttpStatusCode status = photoResponse.Status;
						if (status != HttpStatusCode.OK && status == HttpStatusCode.NotFound)
						{
							this.WriteError(new UserPhotoNotFoundException(this.Preview), ExchangeErrorCategory.Client, this.Identity, true);
							throw new InvalidOperationException();
						}
						memoryStream.Seek(0L, SeekOrigin.Begin);
						result = new UserPhotoConfiguration(dataObject.Identity, memoryStream, photoResponse.Thumbprint);
					}
				}
			}
			catch (UserPhotoNotFoundException)
			{
				throw;
			}
			catch (Exception ex)
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_FailedToRetrievePhoto, new string[]
				{
					dataObject.ToString(),
					((ADUser)dataObject).UserPrincipalName,
					ex.ToString()
				});
				throw;
			}
			finally
			{
				this.tracer.Dump(new PhotoRequestLogWriter(GetUserPhoto.RequestLog, GetUserPhoto.GenerateRequestId()));
			}
			return result;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ADNoSuchObjectException).IsInstanceOfType(exception) || typeof(ADOperationException).IsInstanceOfType(exception) || typeof(IOException).IsInstanceOfType(exception) || typeof(UserPhotoNotFoundException).IsInstanceOfType(exception) || typeof(StoragePermanentException).IsInstanceOfType(exception) || typeof(StorageTransientException).IsInstanceOfType(exception);
		}

		private PhotoRequest CreateRetrievePhotoRequest(ExchangePrincipal principal)
		{
			return new PhotoRequest
			{
				Preview = this.Preview,
				Size = UserPhotoSize.HR240x240,
				UploadTo = principal.ObjectId
			};
		}

		private static string GenerateRequestId()
		{
			return RandomPhotoRequestIdGenerator.Generate();
		}

		private const UserPhotoSize SizeToReturn = UserPhotoSize.HR240x240;

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Id
		};

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static readonly PhotoRequestLog RequestLog = new PhotoRequestLogFactory(GetUserPhoto.PhotosConfiguration, ExchangeSetupContext.InstalledVersion.ToString()).Create();

		private PhotoCmdletTracer tracer;
	}
}

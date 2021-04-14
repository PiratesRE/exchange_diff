using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.InfoWorker.Common.UserPhotos;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class GetPersonaPhoto : ServiceCommand<Stream>
	{
		static GetPersonaPhoto()
		{
			CertificateValidationManager.RegisterCallback("GetPersonaPhoto", new RemoteCertificateValidationCallback(CommonCertificateValidationCallbacks.InternalServerToServer));
		}

		public GetPersonaPhoto(CallContext callContext, string personIdParam, string adObjectIdParam, string email, string singleSourceId, Microsoft.Exchange.Services.Core.Types.UserPhotoSize size) : base(callContext)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			this.InitializeRequestTrackingInformation();
			this.InitializeTracers();
			OwsLogRegistry.Register("GetPersonaPhoto", typeof(PhotosLoggingMetadata), new Type[0]);
			this.perfLogger = new PhotoRequestPerformanceLogger(base.CallContext.ProtocolLog, this.tracer);
			this.responseContext = callContext.CreateWebResponseContext();
			this.size = size;
			this.tracer.TraceDebug<Microsoft.Exchange.Services.Core.Types.UserPhotoSize>((long)this.GetHashCode(), "size = {0}", this.size);
			this.email = email;
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "email = {0}", this.email);
			if (!string.IsNullOrEmpty(personIdParam))
			{
				try
				{
					if (IdConverter.EwsIdIsConversationId(personIdParam))
					{
						this.personId = IdConverter.EwsIdToPersonId(personIdParam);
						this.tracer.TraceDebug<PersonId>((long)this.GetHashCode(), "personId = {0}", this.personId);
					}
				}
				catch (InvalidStoreIdException arg)
				{
					this.tracer.TraceError<string, InvalidStoreIdException>((long)this.GetHashCode(), "Exception: InvalidStoreIdException. personIdParam = {0}. Exception: {1}", personIdParam, arg);
				}
			}
			Guid empty = Guid.Empty;
			if (!string.IsNullOrEmpty(adObjectIdParam) && Guid.TryParse(adObjectIdParam, out empty))
			{
				this.adObjectId = new ADObjectId(empty);
				this.tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "adObjectId = {0}", this.adObjectId);
			}
			if (!string.IsNullOrEmpty(singleSourceId))
			{
				if (IdConverter.EwsIdIsActiveDirectoryObject(singleSourceId))
				{
					this.adObjectId = IdConverter.EwsIdToADObjectId(singleSourceId);
					this.tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "adObjectId = {0}", this.adObjectId);
					return;
				}
				this.contactId = IdConverter.EwsIdToMessageStoreObjectId(singleSourceId);
				this.tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "contactId = {0}", this.contactId);
			}
		}

		protected override Stream InternalExecute()
		{
			Stream result;
			try
			{
				using (new StopwatchPerformanceTracker("GetPersonaPhotoTotal", this.perfLogger))
				{
					using (new ADPerformanceTracker("GetPersonaPhotoTotal", this.perfLogger))
					{
						using (new StorePerformanceTracker("GetPersonaPhotoTotal", this.perfLogger))
						{
							result = this.ExecuteGetPersonaPhoto();
						}
					}
				}
			}
			catch (Exception arg)
			{
				this.tracer.TraceError<Exception>((long)this.GetHashCode(), "GetPersonaPhoto.InternalExecute: exception: {0}", arg);
				base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.GetPersonaPhotoFailed, true);
				throw;
			}
			return result;
		}

		protected override void LogTracesForCurrentRequest()
		{
			WcfServiceCommandBase.TraceLoggerFactory.Create(this.responseContext.Headers).LogTraces(this.requestTracer);
			if (!NullTracer.Instance.Equals(this.requestTracer))
			{
				((InMemoryTracer)this.requestTracer).Dump(new PhotoRequestLogWriter(GetPersonaPhoto.RequestLog, this.requestId));
			}
		}

		private static string ComputeExpiresHeader(HttpStatusCode responseStatus)
		{
			return UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, responseStatus, GetPersonaPhoto.PhotosConfiguration);
		}

		private void InitializeTracers()
		{
			ITracer tracer;
			if (!base.IsRequestTracingEnabled)
			{
				ITracer instance = NullTracer.Instance;
				tracer = instance;
			}
			else
			{
				tracer = new InMemoryTracer(ExTraceGlobals.GetPersonaPhotoTracer.Category, ExTraceGlobals.GetPersonaPhotoTracer.TraceTag);
			}
			this.requestTracer = tracer;
			this.tracer = ExTraceGlobals.GetPersonaPhotoTracer.Compose(this.requestTracer);
		}

		private Stream ExecuteGetPersonaPhoto()
		{
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.ExecutedV2Implementation, true);
			Stream result;
			try
			{
				PhotoRequest request = this.AssembleRequest();
				MemoryStream memoryStream = new MemoryStream();
				using (DisposeGuard disposeGuard = memoryStream.Guard())
				{
					IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
					PhotoResponse photoResponse = new OwaPhotoRetrievalPipeline(GetPersonaPhoto.PhotosConfiguration, "GetPersonaPhoto", base.MailboxIdentityMailboxSession.ClientInfoString, adrecipientSession, this.GetProxyProviderForOutboundRequest(), this.GetRemoteForestPipelineFactory(adrecipientSession), XSOFactory.Default, this.requestTracer).Retrieve(request, memoryStream);
					this.responseContext.StatusCode = photoResponse.Status;
					this.responseContext.ETag = photoResponse.ETag;
					if (string.IsNullOrEmpty(photoResponse.ContentType))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "GetPersonaPhoto: ContentType of response is null or empty");
					}
					base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.ResponseContentType, photoResponse.ContentType);
					this.responseContext.ContentType = photoResponse.ContentType;
					this.responseContext.Headers["Location"] = (photoResponse.PhotoUrl ?? string.Empty);
					string text = string.IsNullOrEmpty(photoResponse.HttpExpiresHeader) ? GetPersonaPhoto.ComputeExpiresHeader(photoResponse.Status) : photoResponse.HttpExpiresHeader;
					this.responseContext.Headers["Expires"] = text;
					memoryStream.Seek(0L, SeekOrigin.Begin);
					this.tracer.TraceDebug((long)this.GetHashCode(), "GetPersonaPhoto: request completed.  Status: {0};  ETag: {1};  Content-Type: {2};  Expires: '{3}';  Redirect: '{4}'", new object[]
					{
						photoResponse.Status,
						photoResponse.ETag,
						photoResponse.ContentType,
						text,
						photoResponse.PhotoUrl
					});
					disposeGuard.Success();
					result = memoryStream;
				}
			}
			catch (IOException caughtException)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException);
			}
			catch (Win32Exception caughtException2)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException2);
			}
			catch (UnauthorizedAccessException caughtException3)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException3);
			}
			catch (TimeoutException caughtException4)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException4);
			}
			catch (StorageTransientException caughtException5)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException5);
			}
			catch (StoragePermanentException caughtException6)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException6);
			}
			catch (TransientException caughtException7)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException7);
			}
			catch (ADOperationException caughtException8)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException8);
			}
			catch (ServicePermanentException caughtException9)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.responseContext, caughtException9);
			}
			return result;
		}

		private PhotoRequest AssembleRequest()
		{
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.PhotoSize, this.size);
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.TargetEmailAddress, this.email);
			PhotoRequest photoRequest = new PhotoRequest
			{
				Requestor = new PhotoPrincipal
				{
					EmailAddresses = base.CallContext.AccessingPrincipal.GetAllEmailAddresses(),
					OrganizationId = base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId
				},
				RequestorMailboxSession = base.MailboxIdentityMailboxSession,
				ETag = base.CallContext.HttpContext.Request.Headers["If-None-Match"],
				Preview = false,
				Size = ServicePhotoSizeToStoragePhotoSizeConverter.Convert(this.size),
				TargetSmtpAddress = this.email,
				TargetAdObjectId = this.adObjectId,
				TargetPersonId = this.personId,
				TargetContactId = this.contactId,
				PerformanceLogger = this.perfLogger,
				HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(GetPersonaPhoto.NoHostOwnedMailboxSession),
				HandlersToSkip = this.GetHandlersToSkip(),
				Trace = base.IsRequestTracingEnabled,
				ClientRequestId = this.clientRequestId,
				ClientContextForRemoteForestRequests = this.CreateClientContext()
			};
			if (this.IsSelfPhotoRequest())
			{
				base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.IsSelfPhoto, true);
				photoRequest.HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(this.GetMailboxSessionOfRequestor);
				photoRequest.TargetPrincipal = base.CallContext.AccessingPrincipal;
				photoRequest.Self = new bool?(true);
			}
			return photoRequest;
		}

		private IMailboxSession GetMailboxSessionOfRequestor(ExchangePrincipal target)
		{
			return base.MailboxIdentityMailboxSession;
		}

		private static IMailboxSession NoHostOwnedMailboxSession(ExchangePrincipal target)
		{
			return null;
		}

		private PhotoHandlers GetHandlersToSkip()
		{
			return PhotosDiagnostics.Instance.GetHandlersToSkip(base.CallContext.HttpContext.Request);
		}

		private void InitializeRequestTrackingInformation()
		{
			this.requestId = this.ComputeRequestId();
			this.clientRequestId = this.ComputeClientRequestId();
		}

		private string ComputeClientRequestId()
		{
			if (!string.IsNullOrEmpty(this.clientRequestId))
			{
				return this.clientRequestId;
			}
			if (base.CallContext.ProtocolLog == null || base.CallContext.ProtocolLog.ActivityScope == null)
			{
				return this.ComputeRequestId();
			}
			string text = base.CallContext.ProtocolLog.ActivityScope.ClientRequestId;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return this.ComputeRequestId();
		}

		private string ComputeRequestId()
		{
			if (!string.IsNullOrEmpty(this.requestId))
			{
				return this.requestId;
			}
			if (base.CallContext.ProtocolLog == null)
			{
				return Guid.NewGuid().ToString();
			}
			if (!Guid.Empty.Equals(base.CallContext.ProtocolLog.ActivityId))
			{
				return base.CallContext.ProtocolLog.ActivityId.ToString();
			}
			return Guid.NewGuid().ToString();
		}

		private bool IsSelfPhotoRequest()
		{
			if (base.CallContext.AccessingPrincipal == null)
			{
				return false;
			}
			if (base.CallContext.AccessingPrincipal.ObjectId.Equals(this.adObjectId))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Requestor is requesting his/her own photo: target AD object ID matches requestor's.");
				return true;
			}
			PhotoPrincipal photoPrincipal = new PhotoPrincipal
			{
				EmailAddresses = base.CallContext.AccessingPrincipal.GetAllEmailAddresses()
			};
			PhotoPrincipal other = new PhotoPrincipal
			{
				EmailAddresses = new string[]
				{
					this.email
				}
			};
			bool flag = photoPrincipal.IsSame(other);
			if (flag)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Requestor is requesting his/her own photo: target and requestor are same principal.");
			}
			return flag;
		}

		private IPhotoRequestOutboundWebProxyProvider GetProxyProviderForOutboundRequest()
		{
			return new PhotoRequestOutboundWebProxyProviderUsingLocalServerConfiguration(this.requestTracer);
		}

		private Stream TraceExceptionAndReturnInternalServerError(IOutgoingWebResponseContext webContext, Exception caughtException)
		{
			this.tracer.TraceError<Exception>((long)this.GetHashCode(), "Request failed with exception: {0}", caughtException);
			webContext.StatusCode = HttpStatusCode.InternalServerError;
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.GetPersonaPhotoFailed, true);
			return new MemoryStream(Array<byte>.Empty);
		}

		private IRemoteForestPhotoRetrievalPipelineFactory GetRemoteForestPipelineFactory(IRecipientSession recipientSession)
		{
			return new RemoteForestPhotoRetrievalPipelineUsingAvailabilityServiceFactory(GetPersonaPhoto.PhotosConfiguration, recipientSession, this.requestTracer);
		}

		private ClientContext CreateClientContext()
		{
			ClientContext result;
			using (new StopwatchPerformanceTracker("CreateClientContext", this.perfLogger))
			{
				using (new ADPerformanceTracker("CreateClientContext", this.perfLogger))
				{
					ADUser adUser;
					ADIdentityInformationCache.Singleton.TryGetADUser(base.CallContext.EffectiveCallerSid, base.CallContext.ADRecipientSessionContext, out adUser);
					ClientContext clientContext = ClientContext.Create(base.CallContext.EffectiveCaller.ClientSecurityContext, null, EWSSettings.RequestTimeZone, EWSSettings.ClientCulture, this.GetMessageIdForRequestTracking(), adUser);
					clientContext.RequestSchemaVersion = Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ExchangeVersionType.Exchange2012;
					clientContext.RequestId = this.clientRequestId;
					result = clientContext;
				}
			}
			return result;
		}

		private string GetMessageIdForRequestTracking()
		{
			if (string.IsNullOrEmpty(base.CallContext.MessageId))
			{
				return string.Format(CultureInfo.InvariantCulture, "urn:uuid:{0}", new object[]
				{
					Guid.NewGuid()
				});
			}
			return base.CallContext.MessageId;
		}

		private const string GetPersonaPhotoActionName = "GetPersonaPhoto";

		private const string IfNoneMatchHeaderName = "If-None-Match";

		private const string CertificateValidationComponentId = "GetPersonaPhoto";

		private const string ExpiresHeaderName = "Expires";

		private const string LocationHeaderName = "Location";

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static readonly PhotoRequestLog RequestLog = new PhotoRequestLogFactory(GetPersonaPhoto.PhotosConfiguration, ExchangeSetupContext.InstalledVersion.ToString()).Create();

		private readonly ADObjectId adObjectId;

		private readonly StoreObjectId contactId;

		private readonly string email;

		private readonly Microsoft.Exchange.Services.Core.Types.UserPhotoSize size;

		private readonly IPerformanceDataLogger perfLogger;

		private readonly IOutgoingWebResponseContext responseContext;

		private PersonId personId;

		private ITracer tracer = ExTraceGlobals.GetPersonaPhotoTracer;

		private ITracer requestTracer = NullTracer.Instance;

		private string requestId;

		private string clientRequestId;
	}
}

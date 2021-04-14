using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.InfoWorker.Common.UserPhotos;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal class GetUserPhoto : SingleStepServiceCommand<GetUserPhotoRequest, Stream>
	{
		static GetUserPhoto()
		{
			CertificateValidationManager.RegisterCallback("GetUserPhoto", new RemoteCertificateValidationCallback(CommonCertificateValidationCallbacks.InternalServerToServer));
		}

		public GetUserPhoto(CallContext callContext, GetUserPhotoRequest request, ITracer upstreamTracer) : base(callContext, request)
		{
			this.InitializeRequestTrackingInformation();
			this.InitializeTracers(upstreamTracer);
			this.perfLogger = new PhotoRequestPerformanceLogger(base.CallContext.ProtocolLog, this.requestTracer);
			OwsLogRegistry.Register("GetUserPhoto", typeof(PhotosLoggingMetadata), new Type[0]);
			base.Request.CacheId = base.CallContext.HttpContext.Request.Headers["If-None-Match"];
			if (base.Request.IsPostRequest)
			{
				this.OutgoingResponse = base.CallContext.CreateWebResponseContext();
				return;
			}
			this.OutgoingResponse = request.OutgoingResponse;
		}

		private IOutgoingWebResponseContext OutgoingResponse { get; set; }

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.SharingCalendarFreeBusy;
			}
		}

		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			if (base.Request.IsPostRequest)
			{
				return new GetUserPhotoResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value, this.hasChanged, this.photoContentType);
			}
			GetUserPhotoResponse getUserPhotoResponse = new GetUserPhotoResponse();
			getUserPhotoResponse.AddResponse(new GetUserPhotoResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value, this.hasChanged, this.photoContentType));
			return getUserPhotoResponse;
		}

		internal override bool InternalPreExecute()
		{
			if (base.Request.IsPostRequest)
			{
				base.Request.Validate();
			}
			return base.InternalPreExecute();
		}

		internal override ServiceResult<Stream> Execute()
		{
			ServiceResult<Stream> result;
			try
			{
				this.tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "GetUserPhoto.Execute: executing on host {0}.  Request-Id: {1}.  Client-Request-Id: {2}", this.GetIncomingRequestHost(), this.requestId, this.clientRequestId);
				using (new StopwatchPerformanceTracker("GetUserPhotoTotal", this.perfLogger))
				{
					using (new ADPerformanceTracker("GetUserPhotoTotal", this.perfLogger))
					{
						using (new StorePerformanceTracker("GetUserPhotoTotal", this.perfLogger))
						{
							ServiceResult<Stream> serviceResult = this.ExecuteGetUserPhoto();
							if (base.Request.FallbackToClearImage && this.OutgoingResponse.StatusCode == HttpStatusCode.NotFound)
							{
								this.tracer.TraceDebug((long)this.GetHashCode(), "Since client is requesting to fallback on clear image and a photo not found, returning a clear 1x1 GIF image.");
								if (serviceResult.Value != null)
								{
									serviceResult.Value.Dispose();
								}
								result = this.GetClearImage();
							}
							else
							{
								result = serviceResult;
							}
						}
					}
				}
			}
			catch (IOException caughtException)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException);
			}
			catch (Win32Exception caughtException2)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException2);
			}
			catch (UnauthorizedAccessException caughtException3)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException3);
			}
			catch (TimeoutException caughtException4)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException4);
			}
			catch (StorageTransientException caughtException5)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException5);
			}
			catch (StoragePermanentException caughtException6)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException6);
			}
			catch (TransientException caughtException7)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException7);
			}
			catch (ADOperationException caughtException8)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException8);
			}
			catch (ServicePermanentException caughtException9)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException9);
			}
			catch (Exception caughtException10)
			{
				this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException10);
				throw;
			}
			finally
			{
				this.LogPhotoRequestTraces();
			}
			return result;
		}

		private ServiceResult<Stream> GetClearImage()
		{
			this.OutgoingResponse.ContentType = (this.photoContentType = "image/gif");
			this.OutgoingResponse.StatusCode = HttpStatusCode.OK;
			return new ServiceResult<Stream>(new MemoryStream(GetUserPhoto.Clear1x1GIF));
		}

		private static ICollection<string> GetEmailAddressesOfRequestor(ExchangePrincipal requestor, ClientContext requestorContext)
		{
			return GetUserPhoto.GetEmailAddressesFromExchangePrincipal(requestor).Union(GetUserPhoto.GetEmailAddressesFromClientContext(requestorContext));
		}

		private static ICollection<string> GetEmailAddressesFromExchangePrincipal(ExchangePrincipal requestor)
		{
			if (requestor == null)
			{
				return Array<string>.Empty;
			}
			return requestor.GetAllEmailAddresses();
		}

		private static ICollection<string> GetEmailAddressesFromClientContext(ClientContext clientContext)
		{
			if (clientContext == null)
			{
				return Array<string>.Empty;
			}
			if (clientContext is PersonalClientContext)
			{
				return new string[]
				{
					((PersonalClientContext)clientContext).EmailAddress.ToString()
				};
			}
			if (clientContext is OrganizationalClientContext)
			{
				return new string[]
				{
					((OrganizationalClientContext)clientContext).EmailAddress.ToString()
				};
			}
			return Array<string>.Empty;
		}

		private ServiceResult<Stream> ExecuteGetUserPhoto()
		{
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.PhotoSize, base.Request.SizeRequested);
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.TargetEmailAddress, base.Request.Email);
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.ExecutedV2Implementation, true);
			this.request = this.AssemblePhotoRequest();
			MemoryStream memoryStream = new MemoryStream();
			ServiceResult<Stream> result;
			using (DisposeGuard disposeGuard = memoryStream.Guard())
			{
				IRecipientSession recipientSession = this.CreateRecipientSession(this.request.Requestor.OrganizationId);
				PhotoResponse photoResponse = new OrganizationalPhotoRetrievalPipeline(GetUserPhoto.PhotosConfiguration, "GetUserPhoto", this.GetMailboxSessionClientInfo(), recipientSession, this.GetProxyProviderForOutboundRequest(), this.GetRemoteForestPipelineFactory(recipientSession), XSOFactory.Default, this.downstreamTracer).Retrieve(this.request, memoryStream);
				this.photoContentType = photoResponse.ContentType;
				if (string.IsNullOrEmpty(this.photoContentType))
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "GetUserPhoto: ContentType of response is null or empty");
				}
				base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.ResponseContentType, this.photoContentType);
				this.OutgoingResponse.StatusCode = photoResponse.Status;
				this.OutgoingResponse.ETag = photoResponse.ETag;
				if (base.Request.IsPostRequest)
				{
					this.OutgoingResponse.ContentType = "text/xml; charset=utf-8";
				}
				else
				{
					this.OutgoingResponse.ContentType = photoResponse.ContentType;
				}
				string text = string.IsNullOrEmpty(photoResponse.HttpExpiresHeader) ? UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, photoResponse.Status, GetUserPhoto.PhotosConfiguration) : photoResponse.HttpExpiresHeader;
				this.OutgoingResponse.Expires = text;
				this.ComputeHasChangedNodeOfSoapResponseAndResetStatusCode();
				memoryStream.Seek(0L, SeekOrigin.Begin);
				this.tracer.TraceDebug((long)this.GetHashCode(), "GetUserPhoto: request completed.  Status: {0};  ETag: {1};  Content-Type: {2};  Content-Length: {3};  Expires: {4}", new object[]
				{
					photoResponse.Status,
					photoResponse.ETag,
					photoResponse.ContentType,
					photoResponse.ContentLength,
					text
				});
				disposeGuard.Success();
				result = new ServiceResult<Stream>(memoryStream);
			}
			return result;
		}

		private ClientContext CreateClientContext()
		{
			ClientContext clientContext = this.CreateContextBasedOnClientLocation();
			clientContext.RequestSchemaVersion = Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ExchangeVersionType.Exchange2012;
			clientContext.RequestId = this.clientRequestId;
			return clientContext;
		}

		private ClientContext CreateContextBasedOnClientLocation()
		{
			ClientContext result;
			using (new StopwatchPerformanceTracker("CreateClientContext", this.perfLogger))
			{
				using (new ADPerformanceTracker("CreateClientContext", this.perfLogger))
				{
					base.CallerBudget.EndLocal();
					try
					{
						if (base.CallContext.IsExternalUser)
						{
							ExternalCallContext externalCallContext = (ExternalCallContext)base.CallContext;
							result = ClientContext.Create(externalCallContext.EmailAddress, externalCallContext.ExternalId, externalCallContext.WSSecurityHeader, externalCallContext.SharingSecurityHeader, externalCallContext.Budget, EWSSettings.RequestTimeZone, EWSSettings.ClientCulture);
						}
						else
						{
							ADUser adUser;
							ADIdentityInformationCache.Singleton.TryGetADUser(base.CallContext.EffectiveCallerSid, base.CallContext.ADRecipientSessionContext, out adUser);
							result = ClientContext.Create(base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallerBudget, EWSSettings.RequestTimeZone, EWSSettings.ClientCulture, this.GetMessageIdForRequestTracking(), adUser);
						}
					}
					catch (AuthzException ex)
					{
						this.tracer.TraceError<AuthzException>((long)this.GetHashCode(), "Unable to retrieve client context due to authorization exception: {0}", ex);
						throw new ServiceAccessDeniedException(ex);
					}
					finally
					{
						base.CallerBudget.StartLocal("GetUserPhoto.CreateClientContext", default(TimeSpan));
					}
				}
			}
			return result;
		}

		private PhotoRequest CreatePhotoRequestFromContext(HttpContext context)
		{
			return new HttpPhotoRequestBuilder(GetUserPhoto.PhotosConfiguration, this.downstreamTracer).Parse(context.Request, this.perfLogger);
		}

		private void InitializeTracers(ITracer upstreamTracer)
		{
			this.requestTracer = new InMemoryTracer(ExTraceGlobals.UserPhotosTracer.Category, ExTraceGlobals.UserPhotosTracer.TraceTag);
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(this.requestTracer).Compose(upstreamTracer);
			this.downstreamTracer = this.requestTracer.Compose(upstreamTracer);
		}

		private void LogPhotoRequestTraces()
		{
			if (!this.ShouldTraceRequest())
			{
				return;
			}
			PhotosDiagnostics.Instance.StampTracesOnGetUserPhotosResponse(this.requestTracer, base.CallContext.HttpContext.Response);
			if (this.requestTracer != null && !NullTracer.Instance.Equals(this.requestTracer))
			{
				((InMemoryTracer)this.requestTracer).Dump(new PhotoRequestLogWriter(GetUserPhoto.RequestLog, this.requestId));
			}
		}

		private bool ShouldTraceRequest()
		{
			return (this.request != null && this.request.Trace) || PhotosDiagnostics.Instance.ShouldTraceGetUserPhotoRequest(base.CallContext.HttpContext.Request);
		}

		private bool IsSelfPhotoRequest()
		{
			if (string.IsNullOrEmpty(base.Request.Email) && base.Request.AdObjectId == null)
			{
				return true;
			}
			if (this.RequestWasProxied())
			{
				return false;
			}
			if (base.CallContext.AccessingPrincipal == null)
			{
				return false;
			}
			PhotoPrincipal photoPrincipal = new PhotoPrincipal
			{
				EmailAddresses = GetUserPhoto.GetEmailAddressesOfRequestor(base.CallContext.AccessingPrincipal, null)
			};
			PhotoPrincipal other = new PhotoPrincipal
			{
				EmailAddresses = new string[]
				{
					base.Request.Email
				}
			};
			return photoPrincipal.IsSame(other);
		}

		private string GetIncomingRequestHost()
		{
			if (base.CallContext == null || base.CallContext.HttpContext == null || base.CallContext.HttpContext.Request == null || base.CallContext.HttpContext.Request.Headers == null)
			{
				return string.Empty;
			}
			return base.CallContext.HttpContext.Request.Headers["Host"];
		}

		private bool RequestWasProxied()
		{
			return base.CallContext.AvailabilityProxyRequestType != null;
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

		private void InitializeRequestTrackingInformation()
		{
			this.requestId = this.ComputeRequestId();
			this.clientRequestId = this.ComputeClientRequestId();
		}

		private void ComputeHasChangedNodeOfSoapResponseAndResetStatusCode()
		{
			if (!base.Request.IsPostRequest)
			{
				return;
			}
			if (this.OutgoingResponse.StatusCode != HttpStatusCode.NotModified)
			{
				this.hasChanged = true;
				return;
			}
			this.hasChanged = false;
			this.OutgoingResponse.StatusCode = HttpStatusCode.OK;
			this.tracer.TraceDebug((long)this.GetHashCode(), "Photo has NOT changed.  Setting HTTP status code to 200 and <HasChanged> node to FALSE in POST response.");
		}

		private bool IsRequestViaHttpGetAndKnownToBeLocalToThisBackend()
		{
			return !base.Request.IsPostRequest && base.CallContext.AvailabilityProxyRequestType != null && base.CallContext.AvailabilityProxyRequestType.Value.Equals(ProxyRequestType.CrossSite);
		}

		private ServiceResult<Stream> TraceExceptionAndReturnInternalServerError(IOutgoingWebResponseContext webContext, Exception caughtException)
		{
			this.tracer.TraceError<Exception>((long)this.GetHashCode(), "Request failed with exception: {0}", caughtException);
			webContext.StatusCode = HttpStatusCode.InternalServerError;
			base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.GetUserPhotoFailed, true);
			return new ServiceResult<Stream>(new MemoryStream(Array<byte>.Empty));
		}

		private string GetMailboxSessionClientInfo()
		{
			return StoreSessionCacheBase.BuildMapiApplicationId(base.CallContext, "Action=GetUserPhoto");
		}

		private IMailboxSession GetMailboxSessionOfAccessingPrincipal(ExchangePrincipal target)
		{
			return base.GetMailboxSession(base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
		}

		private IMailboxSession GetMailboxSessionOfUserKnownToBeLocalToThisBackend(ExchangePrincipal otherUser)
		{
			return null;
		}

		private static IMailboxSession NoHostOwnedMailboxSession(ExchangePrincipal target)
		{
			return null;
		}

		private PhotoHandlers GetHandlersToSkip()
		{
			return PhotosDiagnostics.Instance.GetHandlersToSkip(base.CallContext.HttpContext.Request);
		}

		private PhotoRequest AssemblePhotoRequest()
		{
			if (this.IsSelfPhotoRequest())
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Assembled request for SELF-PHOTO.");
				base.CallContext.ProtocolLog.Set(PhotosLoggingMetadata.IsSelfPhoto, true);
				return new PhotoRequest
				{
					Requestor = new PhotoPrincipal
					{
						EmailAddresses = GetUserPhoto.GetEmailAddressesOfRequestor(base.CallContext.AccessingPrincipal, null),
						OrganizationId = base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId
					},
					RequestorFromExternalOrganization = false,
					ETag = base.Request.CacheId,
					Preview = base.Request.IsPreview,
					Size = base.Request.GetConvertedSizeRequested(),
					TargetSmtpAddress = base.Request.Email,
					TargetAdObjectId = base.Request.AdObjectId,
					TargetPrincipal = base.CallContext.AccessingPrincipal,
					PerformanceLogger = this.perfLogger,
					HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(this.GetMailboxSessionOfAccessingPrincipal),
					HandlersToSkip = this.GetHandlersToSkip(),
					Trace = this.ShouldTraceRequest(),
					Self = new bool?(true),
					IsTargetKnownToBeLocalToThisServer = new bool?(true),
					IsTargetMailboxLikelyOnThisServer = new bool?(true),
					ClientRequestId = this.clientRequestId
				};
			}
			if (this.IsRequestViaHttpGetAndKnownToBeLocalToThisBackend())
			{
				PhotoRequest photoRequest = this.CreatePhotoRequestFromContext(base.CallContext.HttpContext);
				photoRequest.HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(this.GetMailboxSessionOfUserKnownToBeLocalToThisBackend);
				photoRequest.Self = new bool?(false);
				photoRequest.IsTargetKnownToBeLocalToThisServer = new bool?(true);
				photoRequest.IsTargetMailboxLikelyOnThisServer = new bool?(true);
				photoRequest.RequestorFromExternalOrganization = base.CallContext.IsExternalUser;
				photoRequest.ClientRequestId = this.clientRequestId;
				this.tracer.TraceDebug((long)this.GetHashCode(), "Assembled request for TARGET KNOWN TO BE LOCAL TO THIS SERVER.");
				return photoRequest;
			}
			ClientContext clientContext = this.CreateClientContext();
			this.tracer.TraceDebug((long)this.GetHashCode(), "Assembled request for ARBITRARY photo.");
			return new PhotoRequest
			{
				Requestor = new PhotoPrincipal
				{
					EmailAddresses = GetUserPhoto.GetEmailAddressesOfRequestor(base.CallContext.AccessingPrincipal, clientContext),
					OrganizationId = clientContext.OrganizationId
				},
				RequestorFromExternalOrganization = base.CallContext.IsExternalUser,
				ETag = base.Request.CacheId,
				Preview = base.Request.IsPreview,
				Size = base.Request.GetConvertedSizeRequested(),
				TargetSmtpAddress = base.Request.Email,
				TargetAdObjectId = base.Request.AdObjectId,
				PerformanceLogger = this.perfLogger,
				HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(GetUserPhoto.NoHostOwnedMailboxSession),
				HandlersToSkip = this.GetHandlersToSkip(),
				Trace = this.ShouldTraceRequest(),
				ClientRequestId = this.clientRequestId,
				IsTargetMailboxLikelyOnThisServer = new bool?(this.LikelyRoutedToTargetMailboxByFrontend()),
				ClientContextForRemoteForestRequests = clientContext
			};
		}

		private IRecipientSession CreateRecipientSession(OrganizationId organizationId)
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 982, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\GetUserPhoto.cs");
		}

		private IPhotoRequestOutboundWebProxyProvider GetProxyProviderForOutboundRequest()
		{
			return new PhotoRequestOutboundWebProxyProviderUsingLocalServerConfiguration(this.downstreamTracer);
		}

		private bool LikelyRoutedToTargetMailboxByFrontend()
		{
			if (base.CallContext == null || base.CallContext.HttpContext == null || base.CallContext.HttpContext.Request == null)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "Cannot determine whether request was likely routed by frontend because context has not been initialized.");
				return false;
			}
			return !base.CallContext.HttpContext.Request.Path.EndsWith("/GetPersonaPhoto", StringComparison.OrdinalIgnoreCase);
		}

		private IRemoteForestPhotoRetrievalPipelineFactory GetRemoteForestPipelineFactory(IRecipientSession recipientSession)
		{
			return new RemoteForestPhotoRetrievalPipelineUsingAvailabilityServiceFactory(GetUserPhoto.PhotosConfiguration, recipientSession, this.downstreamTracer);
		}

		private const string GetUserPhotoActionName = "GetUserPhoto";

		private const string HttpHostHeader = "Host";

		private const string CertificateValidationComponentId = "GetUserPhoto";

		private const string GetPersonaPhotoRequestPathSuffix = "/GetPersonaPhoto";

		private const string HttpPostContentType = "text/xml; charset=utf-8";

		private static readonly byte[] Clear1x1GIF = new byte[]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			1,
			0,
			1,
			0,
			128,
			0,
			0,
			0,
			0,
			0,
			byte.MaxValue,
			byte.MaxValue,
			byte.MaxValue,
			33,
			249,
			4,
			1,
			0,
			0,
			1,
			0,
			44,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			2,
			1,
			76,
			0,
			59
		};

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static readonly PhotoRequestLog RequestLog = new PhotoRequestLogFactory(GetUserPhoto.PhotosConfiguration, ExchangeSetupContext.InstalledVersion.ToString()).Create();

		private readonly IPerformanceDataLogger perfLogger = NullPerformanceDataLogger.Instance;

		private string requestId;

		private string clientRequestId;

		private bool hasChanged;

		private ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private ITracer requestTracer = NullTracer.Instance;

		private ITracer downstreamTracer;

		private PhotoRequest request;

		private string photoContentType;
	}
}

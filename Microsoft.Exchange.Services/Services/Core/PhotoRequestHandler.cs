using System;
using System.ComponentModel;
using System.IO;
using System.Net;
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
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequestHandler : IHttpHandler
	{
		public PhotoRequestHandler(RequestDetailsLogger protocolLogger)
		{
			ArgumentValidator.ThrowIfNull("protocolLogger", protocolLogger);
			this.protocolLogger = protocolLogger;
			this.InitializeProtocolLogger();
			OwsLogRegistry.Register("GetUserPhoto", typeof(PhotosLoggingMetadata), new Type[0]);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			context.Response.TrySkipIisCustomErrors = true;
			this.LogPreExecution();
			this.InitializeRequestTrackingInformation(context);
			this.InitializeTracers(context);
			this.perfLogger = new PhotoRequestPerformanceLogger(this.protocolLogger, this.requestTracer);
			try
			{
				this.tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "PHOTO REQUEST HANDLER: executing on host {0}.  Request-Id: {1}.  Client-Request-Id: {2}", this.GetIncomingRequestHost(context), this.requestId, this.clientRequestId);
				using (new StopwatchPerformanceTracker("GetUserPhotoTotal", this.perfLogger))
				{
					using (new ADPerformanceTracker("GetUserPhotoTotal", this.perfLogger))
					{
						using (new StorePerformanceTracker("GetUserPhotoTotal", this.perfLogger))
						{
							this.request = this.AssemblePhotoRequest(context);
							using (MemoryStream memoryStream = new MemoryStream())
							{
								PhotoResponse photoResponse = new OrganizationalPhotoRetrievalPipeline(PhotoRequestHandler.PhotosConfiguration, "GetUserPhoto", this.GetMailboxSessionClientInfo(context), this.CreateRecipientSession(this.request.Requestor.OrganizationId), this.GetProxyProviderForOutboundRequest(), this.GetRemoteForestPipelineFactory(), XSOFactory.Default, this.tracer).Then(new TransparentImagePhotoHandler(PhotoRequestHandler.PhotosConfiguration, this.tracer)).Retrieve(this.request, new PhotoResponse(memoryStream));
								context.Response.StatusCode = (int)photoResponse.Status;
								context.Response.Headers["ETag"] = photoResponse.ETag;
								context.Response.ContentType = photoResponse.ContentType;
								string text = string.IsNullOrEmpty(photoResponse.HttpExpiresHeader) ? UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, photoResponse.Status, PhotoRequestHandler.PhotosConfiguration) : photoResponse.HttpExpiresHeader;
								context.Response.Headers["Expires"] = text;
								memoryStream.Seek(0L, SeekOrigin.Begin);
								this.tracer.TraceDebug((long)this.GetHashCode(), "PHOTO REQUEST HANDLER: request completed.  Status: {0};  ETag: {1};  Content-Type: {2};  Content-Length: {3};  Expires: {4}", new object[]
								{
									photoResponse.Status,
									photoResponse.ETag,
									photoResponse.ContentType,
									photoResponse.ContentLength,
									text
								});
								memoryStream.WriteTo(context.Response.OutputStream);
								this.protocolLogger.Set(PhotosLoggingMetadata.ServedByPhotoRequestHandler, 1);
							}
						}
					}
				}
			}
			catch (TooComplexPhotoRequestException arg)
			{
				this.tracer.TraceDebug<TooComplexPhotoRequestException>((long)this.GetHashCode(), "PHOTO REQUEST HANDLER: request too complex.  Exception: {0}", arg);
				throw;
			}
			catch (IOException caughtException)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException);
			}
			catch (Win32Exception caughtException2)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException2);
			}
			catch (UnauthorizedAccessException caughtException3)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException3);
			}
			catch (TimeoutException caughtException4)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException4);
			}
			catch (StorageTransientException caughtException5)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException5);
			}
			catch (StoragePermanentException caughtException6)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException6);
			}
			catch (TransientException caughtException7)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException7);
			}
			catch (ADOperationException caughtException8)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException8);
			}
			catch (ArgumentException caughtException9)
			{
				this.TraceErrorAndReturnCode(context, caughtException9, HttpStatusCode.BadRequest);
			}
			catch (Exception caughtException10)
			{
				this.TraceErrorAndReturnInternalServerError(context, caughtException10);
				throw;
			}
			finally
			{
				this.LogRequestTraces(context);
			}
		}

		private PhotoRequest AssemblePhotoRequest(HttpContext context)
		{
			PhotoRequest photoRequest = this.CreatePhotoRequestFromContext(context);
			if (this.IsSelfPhotoRequest(photoRequest))
			{
				throw new TooComplexPhotoRequestException();
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "PHOTO REQUEST HANDLER: assembled request for ARBITRARY photo.");
			photoRequest.RequestorFromExternalOrganization = false;
			photoRequest.HostOwnedTargetMailboxSessionGetter = new Func<ExchangePrincipal, IMailboxSession>(PhotoRequestHandler.NoHostOwnedMailboxSession);
			photoRequest.ClientRequestId = this.clientRequestId;
			photoRequest.IsTargetMailboxLikelyOnThisServer = new bool?(this.LikelyRoutedToTargetMailboxByFrontend(context));
			return photoRequest;
		}

		private void InitializeTracers(HttpContext context)
		{
			this.requestTracer = new InMemoryTracer(ExTraceGlobals.UserPhotosTracer.Category, ExTraceGlobals.UserPhotosTracer.TraceTag);
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(this.requestTracer);
		}

		private void LogRequestTraces(HttpContext context)
		{
			if (!this.ShouldTraceRequest(context))
			{
				return;
			}
			PhotosDiagnostics.Instance.StampTracesOnGetUserPhotosResponse(this.requestTracer, context.Response);
			if (this.requestTracer != null && !NullTracer.Instance.Equals(this.requestTracer))
			{
				((InMemoryTracer)this.requestTracer).Dump(new PhotoRequestLogWriter(PhotoRequestHandler.RequestLog, this.requestId));
			}
		}

		private bool ShouldTraceRequest(HttpContext context)
		{
			return (this.request != null && this.request.Trace) || PhotosDiagnostics.Instance.ShouldTraceGetUserPhotoRequest(context.Request);
		}

		private bool LikelyRoutedToTargetMailboxByFrontend(HttpContext context)
		{
			if (context.Request == null)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "PHOTO REQUEST HANDLER: cannot determine whether request was likely routed by frontend because context has not been initialized.");
				return false;
			}
			return !context.Request.Path.EndsWith("/GetPersonaPhoto", StringComparison.OrdinalIgnoreCase);
		}

		private static IMailboxSession NoHostOwnedMailboxSession(ExchangePrincipal target)
		{
			return null;
		}

		private string ComputeRequestId(HttpContext context)
		{
			if (!string.IsNullOrEmpty(this.requestId))
			{
				return this.requestId;
			}
			if (this.protocolLogger == null)
			{
				return Guid.NewGuid().ToString();
			}
			if (!Guid.Empty.Equals(this.protocolLogger.ActivityId))
			{
				return this.protocolLogger.ActivityId.ToString();
			}
			return Guid.NewGuid().ToString();
		}

		private string ComputeClientRequestId(HttpContext context)
		{
			if (!string.IsNullOrEmpty(this.clientRequestId))
			{
				return this.clientRequestId;
			}
			if (this.protocolLogger == null || this.protocolLogger.ActivityScope == null)
			{
				return this.ComputeRequestId(context);
			}
			string text = this.protocolLogger.ActivityScope.ClientRequestId;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return this.ComputeRequestId(context);
		}

		private void InitializeRequestTrackingInformation(HttpContext context)
		{
			this.requestId = this.ComputeRequestId(context);
			this.clientRequestId = this.ComputeClientRequestId(context);
		}

		private string GetMailboxSessionClientInfo(HttpContext context)
		{
			return string.Format("{0};{1};Action=GetUserPhoto", Global.DefaultMapiClientType, string.IsNullOrEmpty(context.Request.UserAgent) ? "UserAgent=[NoUserAgent]" : context.Request.UserAgent);
		}

		private IPhotoRequestOutboundWebProxyProvider GetProxyProviderForOutboundRequest()
		{
			return new PhotoRequestOutboundWebProxyProviderUsingLocalServerConfiguration(this.tracer);
		}

		private IRecipientSession CreateRecipientSession(OrganizationId organizationId)
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 525, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\PhotoRequestHandler.cs");
		}

		private PhotoRequest CreatePhotoRequestFromContext(HttpContext context)
		{
			PhotoRequest photoRequest = new HttpPhotoRequestBuilder(PhotoRequestHandler.PhotosConfiguration, this.tracer).Parse(context.Request, this.perfLogger);
			photoRequest.Requestor = this.ReadRequestorFromContext(context);
			return photoRequest;
		}

		private bool IsSelfPhotoRequest(PhotoRequest request)
		{
			PhotoPrincipal other = new PhotoPrincipal
			{
				EmailAddresses = new string[]
				{
					request.TargetSmtpAddress,
					request.TargetPrimarySmtpAddress
				}
			};
			return request.Requestor.IsSame(other);
		}

		private PhotoPrincipal ReadRequestorFromContext(HttpContext context)
		{
			PhotoPrincipal photoPrincipal = new PhotoRequestorReader().Read(context);
			if (photoPrincipal == null)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "PHOTO REQUEST HANDLER:  bad request:  requestor MISSING.");
				throw new ArgumentException();
			}
			return photoPrincipal;
		}

		private void TraceErrorAndReturnInternalServerError(HttpContext context, Exception caughtException)
		{
			this.TraceErrorAndReturnCode(context, caughtException, HttpStatusCode.InternalServerError);
		}

		private void TraceErrorAndReturnCode(HttpContext context, Exception caughtException, HttpStatusCode code)
		{
			this.tracer.TraceError<Exception>((long)this.GetHashCode(), "PHOTO REQUEST HANDLER: request failed with exception: {0}", caughtException);
			context.Response.StatusCode = (int)code;
			this.protocolLogger.Set(PhotosLoggingMetadata.GetUserPhotoFailed, true);
		}

		private string GetIncomingRequestHost(HttpContext context)
		{
			if (context.Request.Headers == null)
			{
				return string.Empty;
			}
			return context.Request.Headers["Host"];
		}

		private void InitializeProtocolLogger()
		{
			if (this.protocolLogger == null || this.protocolLogger.ActivityScope == null)
			{
				return;
			}
			this.protocolLogger.ActivityScope.SetProperty(ExtensibleLoggerMetadata.EventId, "GetUserPhoto");
		}

		private void LogPreExecution()
		{
			if (this.protocolLogger == null || this.protocolLogger.ActivityScope == null)
			{
				return;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.protocolLogger, ServiceLatencyMetadata.PreExecutionLatency, this.protocolLogger.ActivityScope.TotalMilliseconds);
		}

		private IRemoteForestPhotoRetrievalPipelineFactory GetRemoteForestPipelineFactory()
		{
			return new RemoteForestPhotoRetrievalPipelineTooComplex(this.tracer);
		}

		private const string ExpiresHeaderName = "Expires";

		private const string ETagHeaderName = "ETag";

		private const string HttpHostHeader = "Host";

		private const string CertificateValidationComponentId = "GetUserPhoto";

		private const string GetPersonaPhotoRequestPathSuffix = "/GetPersonaPhoto";

		private const string GetUserPhotoActionName = "GetUserPhoto";

		private static readonly PhotosConfiguration PhotosConfiguration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);

		private static readonly PhotoRequestLog RequestLog = new PhotoRequestLogFactory(PhotoRequestHandler.PhotosConfiguration, ExchangeSetupContext.InstalledVersion.ToString()).Create();

		private readonly RequestDetailsLogger protocolLogger;

		private IPerformanceDataLogger perfLogger = NullPerformanceDataLogger.Instance;

		private string requestId;

		private string clientRequestId;

		private ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private ITracer requestTracer = NullTracer.Instance;

		private PhotoRequest request;
	}
}

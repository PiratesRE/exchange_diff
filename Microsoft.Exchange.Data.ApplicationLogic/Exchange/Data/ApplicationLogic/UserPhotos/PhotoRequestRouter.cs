using System;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PhotoRequestRouter
	{
		public PhotoRequestRouter(PhotosConfiguration configuration, string certificateValidationComponentId, string clientInfo, IRecipientSession recipientSession, IPhotoServiceLocatorFactory serviceLocatorFactory, IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider, IRemoteForestPhotoRetrievalPipelineFactory remoteForestPipelineFactory, IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNullOrEmpty("certificateValidationComponentId", certificateValidationComponentId);
			ArgumentValidator.ThrowIfNullOrEmpty("clientInfo", clientInfo);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("serviceLocatorFactory", serviceLocatorFactory);
			ArgumentValidator.ThrowIfNull("outgoingRequestProxyProvider", outgoingRequestProxyProvider);
			ArgumentValidator.ThrowIfNull("remoteForestPipelineFactory", remoteForestPipelineFactory);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.configuration = configuration;
			this.certificateValidationComponentId = certificateValidationComponentId;
			this.clientInfo = clientInfo;
			this.recipientSession = recipientSession;
			this.serviceLocatorFactory = serviceLocatorFactory;
			this.outgoingRequestProxyProvider = outgoingRequestProxyProvider;
			this.remoteForestPipelineFactory = remoteForestPipelineFactory;
			this.xsoFactory = xsoFactory;
			this.tracer = upstreamTracer;
		}

		public IPhotoHandler Route(PhotoRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			IPhotoHandler result;
			using (new StopwatchPerformanceTracker("RouterTotal", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("RouterTotal", request.PerformanceLogger))
				{
					if (this.IsSelfPhotoRequest(request))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "ROUTING: self-photo request.");
						result = new SelfPhotoRetrievalPipeline(this.configuration, this.clientInfo, this.recipientSession, this.xsoFactory, this.tracer);
					}
					else if (this.TargetKnownToBeLocalToThisServer(request))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "ROUTING: target known to be local to this server.");
						result = new LocalServerPhotoRetrievalPipeline(this.configuration, this.clientInfo, this.recipientSession, this.xsoFactory, this.tracer);
					}
					else if (request.RequestorFromExternalOrganization)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "ROUTING: requestor is from EXTERNAL organization.");
						result = new ExternalRequestorPhotoRetrievalPipeline(this.recipientSession, this.tracer);
					}
					else
					{
						result = this.LookupTargetInDirectoryAndRoute(request);
					}
				}
			}
			return result;
		}

		private bool IsSelfPhotoRequest(PhotoRequest request)
		{
			return request.Self ?? false;
		}

		private bool TargetKnownToBeLocalToThisServer(PhotoRequest request)
		{
			return request.IsTargetKnownToBeLocalToThisServer ?? false;
		}

		private IPhotoHandler LookupTargetInDirectoryAndRoute(PhotoRequest request)
		{
			ADRecipient adrecipient = this.LookupTargetInDirectory(request);
			if (adrecipient == null)
			{
				return new TargetNotFoundPhotoHandler(this.configuration, this.tracer);
			}
			return this.RouteTarget(request, adrecipient);
		}

		private ADRecipient LookupTargetInDirectory(PhotoRequest request)
		{
			ADRecipient result;
			using (new StopwatchPerformanceTracker("RouterLookupTargetInDirectory", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("RouterLookupTargetInDirectory", request.PerformanceLogger))
				{
					if (!string.IsNullOrEmpty(request.TargetSmtpAddress) && SmtpAddress.IsValidSmtpAddress(request.TargetSmtpAddress))
					{
						ADRecipient adrecipient = this.recipientSession.FindByProxyAddress(ProxyAddress.Parse(request.TargetSmtpAddress));
						if (adrecipient != null)
						{
							return adrecipient;
						}
					}
					if (request.TargetAdObjectId != null)
					{
						ADRecipient adrecipient2 = this.recipientSession.Read(request.TargetAdObjectId);
						if (adrecipient2 != null)
						{
							return adrecipient2;
						}
					}
					this.tracer.TraceDebug<string, ADObjectId>((long)this.GetHashCode(), "ROUTING: target not found in directory.  Search params were SMTP-address='{0}' OR ADObjectId='{1}'", request.TargetSmtpAddress, request.TargetAdObjectId);
					result = null;
				}
			}
			return result;
		}

		private IPhotoHandler RouteTarget(PhotoRequest request, ADRecipient target)
		{
			this.PopulateTargetInformationIntoRequest(request, target);
			switch (target.RecipientType)
			{
			case RecipientType.UserMailbox:
				return this.RouteTargetWithMailboxInLocalForest(request, (ADUser)target);
			case RecipientType.MailUser:
				return this.RouteTargetInRemoteForest();
			default:
				this.tracer.TraceDebug<RecipientType>((long)this.GetHashCode(), "ROUTING: UNEXPECTED/TOO COMPLEX target.  Recipient type: {0}", target.RecipientType);
				return new TargetNotFoundPhotoHandler(this.configuration, this.tracer);
			}
		}

		private void PopulateTargetInformationIntoRequest(PhotoRequest request, ADRecipient target)
		{
			request.TargetRecipient = (request.TargetRecipient ?? target);
			request.TargetAdObjectId = (request.TargetAdObjectId ?? target.Id);
			SmtpAddress primarySmtpAddress = target.PrimarySmtpAddress;
			request.TargetSmtpAddress = (string.IsNullOrEmpty(request.TargetSmtpAddress) ? target.PrimarySmtpAddress.ToString() : request.TargetSmtpAddress);
			request.TargetPrimarySmtpAddress = (string.IsNullOrEmpty(request.TargetPrimarySmtpAddress) ? target.PrimarySmtpAddress.ToString() : request.TargetPrimarySmtpAddress);
		}

		private IPhotoHandler RouteTargetWithMailboxInLocalForest(PhotoRequest request, ADUser target)
		{
			if (this.TargetLikelyOnThisServer(request))
			{
				return new LocalServerFallbackToOtherServerPhotoRetrievalPipeline(this.configuration, this.clientInfo, this.recipientSession, this.xsoFactory, this.certificateValidationComponentId, this.serviceLocatorFactory, this.outgoingRequestProxyProvider, this.tracer);
			}
			IPhotoServiceLocator serviceLocator = this.serviceLocatorFactory.CreateForLocalForest(request.PerformanceLogger);
			if (this.IsTargetMailboxOnThisServer(request, target, serviceLocator))
			{
				return this.RouteTargetOnThisServer();
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "ROUTING: target mailbox is in LOCAL forest but OTHER server.");
			return new LocalForestOtherServerPhotoRetrievalPipeline(this.configuration, this.certificateValidationComponentId, serviceLocator, this.recipientSession, this.outgoingRequestProxyProvider, this.tracer);
		}

		private IPhotoHandler RouteTargetOnThisServer()
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "ROUTING: target mailbox is on this server.");
			return new LocalServerPhotoRetrievalPipeline(this.configuration, this.clientInfo, this.recipientSession, this.xsoFactory, this.tracer);
		}

		private bool IsTargetMailboxOnThisServer(PhotoRequest request, ADUser target, IPhotoServiceLocator serviceLocator)
		{
			bool result;
			using (new StopwatchPerformanceTracker("RouterCheckTargetMailboxOnThisServer", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("RouterCheckTargetMailboxOnThisServer", request.PerformanceLogger))
				{
					result = serviceLocator.IsServiceOnThisServer(serviceLocator.Locate(target));
				}
			}
			return result;
		}

		private IPhotoHandler RouteTargetInRemoteForest()
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "ROUTING: target is in REMOTE forest.");
			return this.remoteForestPipelineFactory.Create();
		}

		private bool TargetLikelyOnThisServer(PhotoRequest request)
		{
			return request.IsTargetMailboxLikelyOnThisServer ?? false;
		}

		private readonly PhotosConfiguration configuration;

		private readonly string certificateValidationComponentId;

		private readonly string clientInfo;

		private readonly IRecipientSession recipientSession;

		private readonly IPhotoServiceLocatorFactory serviceLocatorFactory;

		private readonly IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider;

		private readonly IRemoteForestPhotoRetrievalPipelineFactory remoteForestPipelineFactory;

		private readonly IXSOFactory xsoFactory;

		private readonly ITracer tracer;
	}
}

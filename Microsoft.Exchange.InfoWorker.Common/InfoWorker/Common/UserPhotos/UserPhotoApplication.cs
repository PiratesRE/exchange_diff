using System;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	internal sealed class UserPhotoApplication : Application
	{
		public UserPhotoApplication(PhotoRequest photoRequest, PhotosConfiguration configuration, bool traceRequest, ITracer upstreamTracer) : base(false)
		{
			this.photoRequest = photoRequest;
			this.photosConfiguration = configuration;
			this.traceRequest = traceRequest;
			this.upstreamTracer = upstreamTracer;
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
		}

		public override IService CreateService(WebServiceUri webServiceUri, TargetServerVersion targetVersion, RequestType requestType)
		{
			switch (requestType)
			{
			case RequestType.Local:
			case RequestType.IntraSite:
			case RequestType.CrossSite:
				return this.CreateRestService(webServiceUri);
			}
			return this.CreateSoapService(webServiceUri);
		}

		public override IAsyncResult BeginProxyWebRequest(IService service, MailboxData[] mailboxArray, AsyncCallback callback, object asyncState)
		{
			if (mailboxArray == null)
			{
				throw new ArgumentNullException("mailboxArray");
			}
			if (mailboxArray.Length < 1)
			{
				throw new ArgumentOutOfRangeException("mailboxArray.Length");
			}
			this.tracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "Proxying GetUserPhoto request.  Target: {0}. Service endpoint: {1}. Timeout: {2}.", this.photoRequest.TargetSmtpAddress, service.Url, service.Timeout);
			this.proxyLatencyTracker = new StopwatchPerformanceTracker("ProxyRequest", this.photoRequest.PerformanceLogger);
			return service.BeginGetUserPhoto(this.photoRequest, this.photosConfiguration, callback, asyncState);
		}

		public override void EndProxyWebRequest(ProxyWebRequest proxyWebRequest, QueryList queryList, IService service, IAsyncResult asyncResult)
		{
			try
			{
				GetUserPhotoResponseMessageType getUserPhotoResponseMessageType = service.EndGetUserPhoto(asyncResult);
				queryList.SetResultInAllQueries(new UserPhotoQueryResult(getUserPhotoResponseMessageType.PictureData, getUserPhotoResponseMessageType.CacheId, getUserPhotoResponseMessageType.StatusCode, getUserPhotoResponseMessageType.Expires, getUserPhotoResponseMessageType.ContentType, this.upstreamTracer));
			}
			finally
			{
				this.proxyLatencyTracker.Stop();
			}
		}

		public override string GetParameterDataString()
		{
			return string.Empty;
		}

		public override LocalQuery CreateLocalQuery(ClientContext clientContext, DateTime requestCompletionDeadline)
		{
			return new UserPhotoLocalQuery(clientContext, requestCompletionDeadline, this.photoRequest, this.photosConfiguration, this.upstreamTracer);
		}

		public override BaseQueryResult CreateQueryResult(LocalizedException exception)
		{
			return new UserPhotoQueryResult(exception, this.upstreamTracer);
		}

		public override BaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return UserPhotoQuery.CreateFromUnknown(recipientData, exception, this.upstreamTracer);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			return UserPhotoQuery.CreateFromIndividual(recipientData, this.upstreamTracer);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return UserPhotoQuery.CreateFromIndividual(recipientData, exception, this.upstreamTracer);
		}

		public override AvailabilityException CreateExceptionForUnsupportedVersion(RecipientData recipient, int serverVersion)
		{
			ProxyServerWithMinimumRequiredVersionNotFound proxyServerWithMinimumRequiredVersionNotFound = new ProxyServerWithMinimumRequiredVersionNotFound(recipient.EmailAddress, serverVersion, Globals.E15Version);
			proxyServerWithMinimumRequiredVersionNotFound.Data["ThumbnailPhotoKey"] = recipient.ThumbnailPhoto;
			return proxyServerWithMinimumRequiredVersionNotFound;
		}

		public override BaseQuery CreateFromGroup(RecipientData recipientData, BaseQuery[] groupMembers, bool groupCapped)
		{
			throw new NotSupportedException();
		}

		public override bool EnabledInRelationship(OrganizationRelationship organizationRelationship)
		{
			return organizationRelationship.PhotosEnabled;
		}

		public override Offer OfferForExternalSharing
		{
			get
			{
				return Offer.SharingCalendarFreeBusy;
			}
		}

		public override ThreadCounter Worker
		{
			get
			{
				return UserPhotoApplication.UserPhotoWorker;
			}
		}

		public override ThreadCounter IOCompletion
		{
			get
			{
				return UserPhotoApplication.UserPhotoIOCompletion;
			}
		}

		public override int MinimumRequiredVersion
		{
			get
			{
				return Globals.E15Version;
			}
		}

		public override int MinimumRequiredVersionForExternalUser
		{
			get
			{
				return Globals.E15Version;
			}
		}

		public override bool SupportsPersonalSharingRelationship
		{
			get
			{
				return false;
			}
		}

		public override LocalizedString Name
		{
			get
			{
				return Strings.PhotosApplicationName;
			}
		}

		private IService CreateRestService(WebServiceUri webServiceUri)
		{
			this.photoRequest.PerformanceLogger.Log("ProxyOverRestService", string.Empty, 1U);
			return new RestService(HttpAuthenticator.NetworkService, webServiceUri, this.traceRequest, this.upstreamTracer);
		}

		private IService CreateSoapService(WebServiceUri webServiceUri)
		{
			return new Service(webServiceUri, this.traceRequest, this.upstreamTracer)
			{
				RequestServerVersionValue = new RequestServerVersion(),
				RequestServerVersionValue = 
				{
					Version = ExchangeVersionType.Exchange2012
				}
			};
		}

		internal const string ThumbnailPhotoKey = "ThumbnailPhotoKey";

		private readonly ITracer tracer;

		private readonly ITracer upstreamTracer;

		private readonly bool traceRequest;

		private readonly PhotoRequest photoRequest;

		private readonly PhotosConfiguration photosConfiguration;

		private StopwatchPerformanceTracker proxyLatencyTracker;

		public static readonly ThreadCounter UserPhotoWorker = new ThreadCounter();

		public static readonly ThreadCounter UserPhotoIOCompletion = new ThreadCounter();
	}
}

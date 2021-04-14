using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class Application
	{
		protected Application(bool shouldProcessGroup)
		{
			this.shouldProcessGroup = shouldProcessGroup;
		}

		private Application()
		{
		}

		public bool ShouldProcessGroup
		{
			get
			{
				return this.shouldProcessGroup;
			}
		}

		public virtual bool SupportsPersonalSharingRelationship
		{
			get
			{
				return true;
			}
		}

		public abstract int MinimumRequiredVersion { get; }

		public virtual int MinimumRequiredVersionForExternalUser
		{
			get
			{
				return this.MinimumRequiredVersion;
			}
		}

		public abstract LocalizedString Name { get; }

		public abstract IService CreateService(WebServiceUri webServiceUri, TargetServerVersion targetVersion, RequestType requestType);

		public abstract IAsyncResult BeginProxyWebRequest(IService service, MailboxData[] mailboxDataArray, AsyncCallback callback, object asyncState);

		public abstract void EndProxyWebRequest(ProxyWebRequest proxyWebRequest, QueryList queryList, IService service, IAsyncResult asyncResult);

		public abstract string GetParameterDataString();

		public abstract LocalQuery CreateLocalQuery(ClientContext clientContext, DateTime requestCompletionDeadline);

		public abstract BaseQueryResult CreateQueryResult(LocalizedException exception);

		public abstract BaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception);

		public abstract BaseQuery CreateFromIndividual(RecipientData recipientData);

		public abstract BaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception);

		public abstract AvailabilityException CreateExceptionForUnsupportedVersion(RecipientData recipient, int serverVersion);

		public abstract BaseQuery CreateFromGroup(RecipientData recipientData, BaseQuery[] groupMembers, bool groupCapped);

		public abstract Offer OfferForExternalSharing { get; }

		public abstract bool EnabledInRelationship(OrganizationRelationship organizationRelationship);

		public ExchangeVersion? GetRequestedVersionForAutoDiscover(AutodiscoverType autodiscoverType)
		{
			int minimumVersionByAutodiscoverType = this.GetMinimumVersionByAutodiscoverType(autodiscoverType);
			if (Globals.E14SP1Version == minimumVersionByAutodiscoverType)
			{
				return new ExchangeVersion?(ExchangeVersion.Exchange2010_SP1);
			}
			if (Globals.E14Version == minimumVersionByAutodiscoverType)
			{
				return new ExchangeVersion?(ExchangeVersion.Exchange2010);
			}
			if (Globals.E15Version == minimumVersionByAutodiscoverType)
			{
				return new ExchangeVersion?(ExchangeVersion.Exchange2013);
			}
			if (minimumVersionByAutodiscoverType == 0)
			{
				return null;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported minimum required version {0} for {1}, please add this to Application.GetRequestedVersionForAutoDiscover code.", new object[]
			{
				minimumVersionByAutodiscoverType,
				base.GetType()
			}));
		}

		public int GetAutodiscoverVersionBucket(AutodiscoverType autodiscoverType)
		{
			int minimumVersionByAutodiscoverType = this.GetMinimumVersionByAutodiscoverType(autodiscoverType);
			if (Globals.E15Version == minimumVersionByAutodiscoverType)
			{
				return 3;
			}
			if (Globals.E14SP1Version == minimumVersionByAutodiscoverType)
			{
				return 2;
			}
			if (Globals.E14Version == minimumVersionByAutodiscoverType)
			{
				return 1;
			}
			if (minimumVersionByAutodiscoverType == 0)
			{
				return 0;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported minimum required version {0} for {1}, please add this to Application.GetAutodiscoverVersionBucket code.", new object[]
			{
				minimumVersionByAutodiscoverType,
				base.GetType()
			}));
		}

		public virtual AsyncRequestWithQueryList CreateCrossForestAsyncRequestWithAutoDiscover(ClientContext clientContext, RequestLogger requestLogger, QueryList queryList, TargetForestConfiguration targetForestConfiguration)
		{
			return new ProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, targetForestConfiguration, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByDomain.CreateForCrossForest));
		}

		public virtual AsyncRequestWithQueryList CreateCrossForestAsyncRequestWithAutoDiscoverForRemoteMailbox(ClientContext clientContext, RequestLogger requestLogger, QueryList queryList, TargetForestConfiguration targetForestConfiguration)
		{
			return new ProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, targetForestConfiguration, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestXmlByUser.Create));
		}

		public virtual AsyncRequestWithQueryList CreateExternalAsyncRequestWithAutoDiscover(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, ExternalAuthenticationRequest autoDiscoverExternalAuthenticationRequest, ExternalAuthenticationRequest webProxyExternalAuthenticationRequest, Uri autoDiscoverUrl, SmtpAddress sharingKey)
		{
			return new ExternalProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverExternalAuthenticationRequest, webProxyExternalAuthenticationRequest, autoDiscoverUrl, sharingKey, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByDomain.CreateForExternal));
		}

		public virtual AsyncRequestWithQueryList CreateExternalAsyncRequestWithAutoDiscoverForRemoteMailbox(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, ExternalAuthenticationRequest autoDiscoverExternalAuthenticationRequest, ExternalAuthenticationRequest webProxyExternalAuthenticationRequest, Uri autoDiscoverUrl, SmtpAddress sharingKey)
		{
			return new ExternalProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverExternalAuthenticationRequest, webProxyExternalAuthenticationRequest, autoDiscoverUrl, sharingKey, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByUser.Create));
		}

		public virtual AsyncRequestWithQueryList CreateExternalByOAuthAsyncRequestWithAutoDiscover(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, Uri autoDiscoverUrl)
		{
			return new ExternalByOAuthProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverUrl, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByDomain.CreateForExternal));
		}

		public virtual AsyncRequestWithQueryList CreateExternalByOAuthAsyncRequestWithAutoDiscoverForRemoteMailbox(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, Uri autoDiscoverUrl)
		{
			return new ExternalByOAuthProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverUrl, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByUser.Create));
		}

		protected void HandleNullResponse(ProxyWebRequest proxyWebRequest)
		{
			Application.ProxyWebRequestTracer.TraceError((long)proxyWebRequest.GetHashCode(), "{0}: Proxy web request returned NULL response.", new object[]
			{
				TraceContext.Get()
			});
		}

		public abstract ThreadCounter Worker { get; }

		public abstract ThreadCounter IOCompletion { get; }

		public bool IsVersionSupported(int serverVersion)
		{
			return serverVersion >= this.MinimumRequiredVersion;
		}

		public bool IsVersionSupportedForExternalUser(int serverVersion)
		{
			return serverVersion >= this.MinimumRequiredVersionForExternalUser;
		}

		public void LogThreadsUsage(RequestLogger requestLogger)
		{
			int value;
			int value2;
			ThreadPool.GetAvailableThreads(out value, out value2);
			requestLogger.AppendToLog<int>("Threads.Worker.Available", value);
			requestLogger.AppendToLog<int>("Threads.Worker.InUse", this.Worker.Count);
			requestLogger.AppendToLog<int>("Threads.IO.Available", value2);
			requestLogger.AppendToLog<int>("Threads.IO.InUse", this.IOCompletion.Count);
		}

		private int GetMinimumVersionByAutodiscoverType(AutodiscoverType autodiscoverType)
		{
			switch (autodiscoverType)
			{
			case AutodiscoverType.Internal:
				return this.MinimumRequiredVersion;
			case AutodiscoverType.External:
				return this.MinimumRequiredVersionForExternalUser;
			default:
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported AutodiscoverType {0} encountered, please add this to Application.GetMinimumVersionFromAutodiscoverType code.", new object[]
				{
					autodiscoverType
				}));
			}
		}

		protected static readonly Trace ProxyWebRequestTracer = ExTraceGlobals.ProxyWebRequestTracer;

		private bool shouldProcessGroup;
	}
}

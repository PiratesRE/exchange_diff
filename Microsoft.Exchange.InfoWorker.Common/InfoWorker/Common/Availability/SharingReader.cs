using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class SharingReader
	{
		public SharingReader(ADUser requester, IBudget requesterBudget, DateTime queryPrepareDeadline, bool supportsPersonalSharing)
		{
			this.requester = requester;
			this.requesterBudget = requesterBudget;
			this.readDeadline = queryPrepareDeadline;
			this.supportsPersonalSharing = supportsPersonalSharing;
		}

		public SharingInformation Read(EmailAddress emailAddress, Application application)
		{
			if (this.requester == null)
			{
				SharingReader.RequestRoutingTracer.TraceError<object, string>((long)this.GetHashCode(), "{0}: Unable to get the requestor from the client context - address {1}", TraceContext.Get(), emailAddress.Address);
				return new SharingInformation(new InvalidClientSecurityContextException());
			}
			if (this.supportsPersonalSharing)
			{
				SharingSubscriptionData userSubscription = this.SubscriptionLoader.GetUserSubscription(emailAddress);
				if (!this.SubscriptionLoader.IsValid)
				{
					SharingReader.RequestRoutingTracer.TraceError<object, EmailAddress, Exception>((long)this.GetHashCode(), "{0}: SubscriptionLoader cannot open mailbox {1}. Exception: {2}", TraceContext.Get(), emailAddress, this.SubscriptionLoader.HandledException);
				}
				if (userSubscription != null)
				{
					SharingReader.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Found a personal relationship for {1}", TraceContext.Get(), emailAddress.Address);
					if (SmtpAddress.IsValidSmtpAddress(userSubscription.SubscriberIdentity) && SmtpAddress.IsValidSmtpAddress(userSubscription.SharingKey))
					{
						Uri sharingUrl = userSubscription.SharingUrl;
						return new SharingInformation(new SmtpAddress(userSubscription.SubscriberIdentity), new SmtpAddress(userSubscription.SharingKey), new TokenTarget(userSubscription.SharerIdentityFederationUri), new WebServiceUri(sharingUrl.OriginalString, sharingUrl.Scheme, UriSource.Directory, Globals.E14SP2Version), null);
					}
					SharingReader.RequestRoutingTracer.TraceError<object, EmailAddress>((long)this.GetHashCode(), "{0}: The subscriber information in the mailbox is invalid for address {1}. Personal subscription can't be used.", TraceContext.Get(), emailAddress);
				}
			}
			string domain = emailAddress.Domain;
			OrganizationId key = (this.requester.OrganizationId == null) ? OrganizationId.ForestWideOrgId : this.requester.OrganizationId;
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(key);
			SharingReader.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Looking for an Intra-Organization connector with domain {1}.", TraceContext.Get(), domain);
			IntraOrganizationConnector intraOrganizationConnector = organizationIdCacheValue.GetIntraOrganizationConnector(domain);
			WebServiceUri targetSharingEpr;
			if (intraOrganizationConnector != null && intraOrganizationConnector.Enabled)
			{
				Uri discoveryEndpoint = intraOrganizationConnector.DiscoveryEndpoint;
				int autodiscoverVersionBucket = application.GetAutodiscoverVersionBucket(AutodiscoverType.External);
				targetSharingEpr = RemoteServiceUriCache.Get(emailAddress, autodiscoverVersionBucket);
				return new SharingInformation(this.requester.PrimarySmtpAddress, targetSharingEpr, discoveryEndpoint);
			}
			SharingReader.RequestRoutingTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Looking for an Organization Relationship with domain {1}.", TraceContext.Get(), domain);
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
			if (organizationRelationship == null)
			{
				SharingReader.RequestRoutingTracer.TraceError<object, string, EmailAddress>((long)this.GetHashCode(), "{0}: Unable to find a organization Relationship with domain {1} for emailAddress {2}.", TraceContext.Get(), emailAddress.Domain, emailAddress);
				return null;
			}
			if (!organizationRelationship.Enabled)
			{
				SharingReader.RequestRoutingTracer.TraceError<object, OrganizationRelationship, string>((long)this.GetHashCode(), "{0}: Organization Relationship {1} is not enabled for access to domain {2}. Ignoring this relationship.", TraceContext.Get(), organizationRelationship, emailAddress.Domain);
				return null;
			}
			if (!application.EnabledInRelationship(organizationRelationship))
			{
				SharingReader.RequestRoutingTracer.TraceError((long)this.GetHashCode(), "{0}: Organization Relationship {1} is not enabled for application {2} to domain {3}. Ignoring this relationship.", new object[]
				{
					TraceContext.Get(),
					organizationRelationship,
					application.GetType(),
					emailAddress.Domain
				});
				return null;
			}
			if (!organizationRelationship.IsValidForRequestDispatcher())
			{
				SharingReader.RequestRoutingTracer.TraceError((long)this.GetHashCode(), "{0}: Organization Relationship is invalid for dispatching requests, TargetApplicationUri:{1}, TargetSharingEpr:{2}, AutoDiscoverEpr:{3}.", new object[]
				{
					TraceContext.Get(),
					organizationRelationship.TargetApplicationUri,
					organizationRelationship.TargetSharingEpr,
					organizationRelationship.TargetAutodiscoverEpr
				});
				return new SharingInformation(new InvalidOrganizationRelationshipForRequestDispatcherException(organizationRelationship.ToString()));
			}
			if (DateTime.UtcNow > this.readDeadline)
			{
				return new SharingInformation(new TimeoutExpiredException("OrganizationRelationship lookup"));
			}
			Uri targetSharingEpr2 = organizationRelationship.TargetSharingEpr;
			if (targetSharingEpr2 == null)
			{
				int autodiscoverVersionBucket2 = application.GetAutodiscoverVersionBucket(AutodiscoverType.External);
				targetSharingEpr = RemoteServiceUriCache.Get(emailAddress, autodiscoverVersionBucket2);
			}
			else
			{
				targetSharingEpr = new WebServiceUri(targetSharingEpr2.OriginalString, targetSharingEpr2.Scheme, UriSource.Directory, Globals.E14SP2Version);
			}
			return new SharingInformation(this.requester.PrimarySmtpAddress, SmtpAddress.Empty, organizationRelationship.GetTokenTarget(), targetSharingEpr, organizationRelationship.TargetAutodiscoverEpr);
		}

		private SubscriptionLoader SubscriptionLoader
		{
			get
			{
				if (this.subscriptionLoader == null)
				{
					this.subscriptionLoader = new SubscriptionLoader(this.requester, this.requesterBudget);
				}
				return this.subscriptionLoader;
			}
		}

		private IConfigurationSession ADConfigSession
		{
			get
			{
				if (this.configSession == null)
				{
					this.configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.requester.OrganizationId), 279, "ADConfigSession", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\SharingReader.cs");
				}
				return this.configSession;
			}
		}

		private ADUser requester;

		private SubscriptionLoader subscriptionLoader;

		private IConfigurationSession configSession;

		private DateTime readDeadline;

		private IBudget requesterBudget;

		private readonly bool supportsPersonalSharing;

		private static readonly Trace RequestRoutingTracer = ExTraceGlobals.RequestRoutingTracer;
	}
}

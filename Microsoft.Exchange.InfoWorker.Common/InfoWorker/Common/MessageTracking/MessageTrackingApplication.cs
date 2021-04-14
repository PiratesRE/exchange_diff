using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal abstract class MessageTrackingApplication : Application
	{
		public MessageTrackingApplication(bool shouldProcessGroup, bool supportsPublicFolder, ExchangeVersion ewsRequestedVersion) : base(false)
		{
			this.ewsRequestedVersion = ewsRequestedVersion;
		}

		public override IService CreateService(WebServiceUri webServiceUri, TargetServerVersion targetVersion, RequestType requestType)
		{
			Service service = new Service(webServiceUri);
			service.RequestServerVersionValue = new RequestServerVersion();
			service.RequestServerVersionValue.Version = VersionConverter.GetRdExchangeVersionType(service.ServiceVersion);
			return service;
		}

		public override LocalQuery CreateLocalQuery(ClientContext clientContext, DateTime requestCompletionDeadline)
		{
			TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Attempted to create local query in x-forest case.", new object[0]);
			TrackingError trackingError = new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Local autodiscover query for Cross-Forest disallowed", string.Empty);
			TrackingFatalException ex = new TrackingFatalException(trackingError, null, false);
			DiagnosticWatson.SendWatsonWithoutCrash(ex, "CreateLocalQuery", TimeSpan.FromDays(1.0));
			throw ex;
		}

		public override BaseQuery CreateFromGroup(RecipientData recipientData, BaseQuery[] groupMembers, bool groupCapped)
		{
			TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Attempted to create group query in x-forest case.", new object[0]);
			TrackingError trackingError = new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Group autodiscover query for Cross-Forest disallowed", string.Empty);
			TrackingFatalException ex = new TrackingFatalException(trackingError, null, false);
			DiagnosticWatson.SendWatsonWithoutCrash(ex, "CreateFromGroup", TimeSpan.FromDays(1.0));
			throw ex;
		}

		public override AsyncRequestWithQueryList CreateCrossForestAsyncRequestWithAutoDiscoverForRemoteMailbox(ClientContext clientContext, RequestLogger requestLogger, QueryList queryList, TargetForestConfiguration targetForestConfiguration)
		{
			return new ProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, targetForestConfiguration, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestXmlByUser.Create));
		}

		public override AsyncRequestWithQueryList CreateExternalAsyncRequestWithAutoDiscoverForRemoteMailbox(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, ExternalAuthenticationRequest autoDiscoverExternalAuthenticationRequest, ExternalAuthenticationRequest webProxyExternalAuthenticationRequest, Uri autoDiscoverUrl, SmtpAddress sharingKey)
		{
			return new ExternalProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverExternalAuthenticationRequest, webProxyExternalAuthenticationRequest, autoDiscoverUrl, sharingKey, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByUser.Create));
		}

		public override AsyncRequestWithQueryList CreateExternalByOAuthAsyncRequestWithAutoDiscoverForRemoteMailbox(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, Uri autoDiscoverUrl)
		{
			return new ExternalByOAuthProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverUrl, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByUser.Create));
		}

		public override bool EnabledInRelationship(OrganizationRelationship organizationRelationship)
		{
			return organizationRelationship.DeliveryReportEnabled;
		}

		public override AvailabilityException CreateExceptionForUnsupportedVersion(RecipientData recipient, int serverVersion)
		{
			return new ProxyServerWithMinimumRequiredVersionNotFound((recipient != null) ? recipient.EmailAddress : null, serverVersion, this.MinimumRequiredVersion);
		}

		internal static IList<RecipientData> CreateRecipientQueryResult(DirectoryContext directoryContext, DateTime queryPrepareDeadline, string proxyRecipient)
		{
			RecipientQuery recipientQuery = new RecipientQuery(directoryContext.ClientContext, directoryContext.TenantGalSession, queryPrepareDeadline, FindMessageTrackingQuery.RecipientProperties);
			Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress emailAddress = new Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress(string.Empty, proxyRecipient);
			Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress[] emailAddressArray = new Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress[]
			{
				emailAddress
			};
			IList<RecipientData> list = recipientQuery.Query(emailAddressArray);
			if (list[0].IsEmpty)
			{
				list = MessageTrackingApplication.CreateFakeRecipientQueryResult(ServerCache.Instance.GetOrgMailboxForDomain(emailAddress.Domain).ToString());
			}
			return list;
		}

		internal static IList<RecipientData> CreateFakeRecipientQueryResult(string address)
		{
			Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress emailAddress = new Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress(string.Empty, address);
			Dictionary<PropertyDefinition, object> propertyMap = new Dictionary<PropertyDefinition, object>
			{
				{
					ADRecipientSchema.RecipientType,
					RecipientType.MailContact
				},
				{
					ADRecipientSchema.PrimarySmtpAddress,
					new SmtpAddress(address)
				},
				{
					ADRecipientSchema.ExternalEmailAddress,
					new SmtpProxyAddress(address, true)
				}
			};
			RecipientData item = RecipientData.Create(emailAddress, propertyMap);
			return new List<RecipientData>
			{
				item
			};
		}

		public override Offer OfferForExternalSharing
		{
			get
			{
				return Offer.SharingCalendarFreeBusy;
			}
		}

		public override int MinimumRequiredVersion
		{
			get
			{
				switch (this.ewsRequestedVersion)
				{
				case ExchangeVersion.Exchange2010:
					return Globals.E14Version;
				case ExchangeVersion.Exchange2010_SP1:
					return Globals.E14SP1Version;
				case ExchangeVersion.Exchange2013:
					return Globals.E15Version;
				}
				throw new InvalidOperationException("Message tracking application has unexpected ewsRequestedVersion");
			}
		}

		private ExchangeVersion ewsRequestedVersion;
	}
}

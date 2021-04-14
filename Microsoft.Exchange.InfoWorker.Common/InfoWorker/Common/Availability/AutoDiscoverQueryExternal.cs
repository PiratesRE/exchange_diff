using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverQueryExternal : AutoDiscoverQuery
	{
		public AutoDiscoverQueryExternal(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri autoDiscoverUrl, ProxyAuthenticator proxyAuthenticator, AutoDiscoverQueryItem[] queryItems, CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest, QueryList queryList) : this(application, clientContext, requestLogger, autoDiscoverUrl, new AutoDiscoverAuthenticator(proxyAuthenticator), queryItems, 0, createAutoDiscoverRequest, queryList)
		{
		}

		private AutoDiscoverQueryExternal(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri autoDiscoverUrl, AutoDiscoverAuthenticator authenticator, AutoDiscoverQueryItem[] queryItems, int redirectionDepth, CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest, QueryList queryList) : base(application, clientContext, requestLogger, autoDiscoverUrl, authenticator, queryItems, redirectionDepth, createAutoDiscoverRequest, AutodiscoverType.External, queryList)
		{
		}

		protected override AutoDiscoverQuery CreateAutoDiscoverQuery(Uri autoDiscoverUrl, AutoDiscoverQueryItem[] queryItems, int redirectionDepth)
		{
			QueryList queryListFromQueryItems = base.GetQueryListFromQueryItems(queryItems);
			return new AutoDiscoverQueryExternal(base.Application, base.ClientContext, base.RequestLogger, autoDiscoverUrl, base.Authenticator, queryItems, redirectionDepth, base.CreateAutoDiscoverRequest, queryListFromQueryItems);
		}

		protected override AutoDiscoverQuery CreateAutoDiscoverQuery(string domain, AutoDiscoverQueryItem[] queryItems, int redirectionDepth)
		{
			AutoDiscoverQuery.AutoDiscoverTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Search for OrganizationRelationship for domain {1}", TraceContext.Get(), domain);
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(base.ClientContext.OrganizationId);
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
			if (organizationRelationship == null)
			{
				AutoDiscoverQuery.AutoDiscoverTracer.TraceError<object, string>((long)this.GetHashCode(), "{0}: OrganizationRelationship lookup for domain {1} found nothing", TraceContext.Get(), domain);
				throw new AutoDiscoverFailedException(Strings.descConfigurationInformationNotFound(domain), 54588U);
			}
			if (organizationRelationship.TargetAutodiscoverEpr == null)
			{
				AutoDiscoverQuery.AutoDiscoverTracer.TraceError<object, string, ADObjectId>((long)this.GetHashCode(), "{0}: OrganizationRelationship lookup for domain {1} found {2}, but it doesn't have TargetAutodiscoverEpr set", TraceContext.Get(), domain, organizationRelationship.Id);
				throw new AutoDiscoverFailedException(Strings.descMisconfiguredOrganizationRelationship(organizationRelationship.Id.ToString()), 42300U);
			}
			AutoDiscoverQuery.AutoDiscoverTracer.TraceDebug<object, string, ADObjectId>((long)this.GetHashCode(), "{0}: OrganizationRelationship lookup for domain {1} found {2}", TraceContext.Get(), domain, organizationRelationship.Id);
			QueryList queryListFromQueryItems = base.GetQueryListFromQueryItems(queryItems);
			return new AutoDiscoverQueryExternal(base.Application, base.ClientContext, base.RequestLogger, organizationRelationship.TargetAutodiscoverEpr, base.Authenticator, queryItems, redirectionDepth, base.CreateAutoDiscoverRequest, queryListFromQueryItems);
		}

		protected override void SetResult(AutoDiscoverQueryItem queryItem, WebServiceUri webServiceUri)
		{
			WebServiceUri webServiceUri2 = new WebServiceUri(webServiceUri, null, queryItem.EmailAddress);
			base.AddWebServiceUriToCache(queryItem, webServiceUri2);
			AutoDiscoverResult result;
			if (!base.Application.IsVersionSupportedForExternalUser(webServiceUri.ServerVersion))
			{
				AutoDiscoverQuery.AutoDiscoverTracer.TraceError<object, int, Type>((long)this.GetHashCode(), "{0}: Remote server version {1} is considered a legacy server by {2} application for external user.", TraceContext.Get(), webServiceUri.ServerVersion, base.Application.GetType());
				result = new AutoDiscoverResult(base.Application.CreateExceptionForUnsupportedVersion(queryItem.RecipientData, webServiceUri.ServerVersion));
			}
			else
			{
				result = new AutoDiscoverResult(webServiceUri2, base.Authenticator.ProxyAuthenticator);
			}
			queryItem.SetResult(result);
		}

		protected override void SetResult(AutoDiscoverQueryItem queryItem, LocalizedString exceptionString, uint locationIdentifier)
		{
			WebServiceUri webServiceUri = new WebServiceUri(null, exceptionString, queryItem.EmailAddress);
			base.AddWebServiceUriToCache(queryItem, webServiceUri);
			queryItem.SetResult(new AutoDiscoverResult(new AutoDiscoverFailedException(exceptionString, locationIdentifier)));
		}
	}
}

using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverQueryInternal : AutoDiscoverQuery
	{
		public AutoDiscoverQueryInternal(Application application, ClientContext clientContext, RequestLogger requestLogger, TargetForestConfiguration targetForestConfiguration, AutoDiscoverQueryItem[] queryItems, CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest, QueryList queryList) : this(application, clientContext, requestLogger, targetForestConfiguration.AutoDiscoverUrl, new AutoDiscoverAuthenticator(targetForestConfiguration.GetCredentialCache(targetForestConfiguration.AutoDiscoverUrl), targetForestConfiguration.Credentials), queryItems, 0, createAutoDiscoverRequest, targetForestConfiguration, queryList)
		{
		}

		private AutoDiscoverQueryInternal(Application application, ClientContext clientContext, RequestLogger requestLogger, Uri autoDiscoverUrl, AutoDiscoverAuthenticator authenticator, AutoDiscoverQueryItem[] queryItems, int redirectionDepth, CreateAutoDiscoverRequestDelegate createAutoDiscoverRequest, TargetForestConfiguration targetForestConfiguration, QueryList queryList) : base(application, clientContext, requestLogger, autoDiscoverUrl, authenticator, queryItems, redirectionDepth, createAutoDiscoverRequest, AutodiscoverType.Internal, queryList)
		{
			this.targetForestConfiguration = targetForestConfiguration;
		}

		protected override AutoDiscoverQuery CreateAutoDiscoverQuery(Uri autoDiscoverUrl, AutoDiscoverQueryItem[] queryItems, int redirectionDepth)
		{
			QueryList queryListFromQueryItems = base.GetQueryListFromQueryItems(queryItems);
			return new AutoDiscoverQueryInternal(base.Application, base.ClientContext, base.RequestLogger, autoDiscoverUrl, new AutoDiscoverAuthenticator(this.targetForestConfiguration.GetCredentialCache(autoDiscoverUrl), this.targetForestConfiguration.Credentials), queryItems, redirectionDepth, base.CreateAutoDiscoverRequest, this.targetForestConfiguration, queryListFromQueryItems);
		}

		protected override AutoDiscoverQuery CreateAutoDiscoverQuery(string domain, AutoDiscoverQueryItem[] queryItems, int redirectionDepth)
		{
			AutoDiscoverQuery.AutoDiscoverTracer.TraceDebug<object, string>((long)this.GetHashCode(), "{0}: Search for TargetForestConfiguration for domain {1}", TraceContext.Get(), domain);
			TargetForestConfiguration targetForestConfiguration = TargetForestConfigurationCache.FindByDomain(base.ClientContext.OrganizationId, domain);
			if (targetForestConfiguration.Exception != null)
			{
				AutoDiscoverQuery.AutoDiscoverTracer.TraceError<object, string, LocalizedException>((long)this.GetHashCode(), "{0}: Found TargetForestConfiguration lookup for domain {1}, but it is in failed state due exception: {2}", TraceContext.Get(), domain, targetForestConfiguration.Exception);
				throw targetForestConfiguration.Exception;
			}
			QueryList queryListFromQueryItems = base.GetQueryListFromQueryItems(queryItems);
			return new AutoDiscoverQueryInternal(base.Application, base.ClientContext, base.RequestLogger, targetForestConfiguration.AutoDiscoverUrl, new AutoDiscoverAuthenticator(targetForestConfiguration.GetCredentialCache(targetForestConfiguration.AutoDiscoverUrl), targetForestConfiguration.Credentials), queryItems, redirectionDepth, base.CreateAutoDiscoverRequest, targetForestConfiguration, queryListFromQueryItems);
		}

		protected override void SetResult(AutoDiscoverQueryItem queryItem, WebServiceUri webServiceUri)
		{
			WebServiceUri webServiceUri2 = new WebServiceUri(webServiceUri, this.targetForestConfiguration.Credentials, queryItem.EmailAddress);
			base.AddWebServiceUriToCache(queryItem, webServiceUri2);
			queryItem.SetResult(this.GetResult(queryItem.RecipientData, webServiceUri2));
		}

		protected override void SetResult(AutoDiscoverQueryItem queryItem, LocalizedString exceptionString, uint locationIdentifier)
		{
			WebServiceUri webServiceUri = new WebServiceUri(this.targetForestConfiguration.Credentials, exceptionString, queryItem.EmailAddress);
			base.AddWebServiceUriToCache(queryItem, webServiceUri);
			queryItem.SetResult(new AutoDiscoverResult(new AutoDiscoverFailedException(exceptionString, locationIdentifier)));
		}

		private AutoDiscoverResult GetResult(RecipientData recipientData, WebServiceUri webServiceUri)
		{
			if (!base.Application.IsVersionSupported(webServiceUri.ServerVersion))
			{
				AutoDiscoverQuery.AutoDiscoverTracer.TraceError<object, int, Type>((long)this.GetHashCode(), "{0}: Remote server version {1} is considered a legacy server by {2} application.", TraceContext.Get(), webServiceUri.ServerVersion, base.Application.GetType());
				return new AutoDiscoverResult(base.Application.CreateExceptionForUnsupportedVersion(recipientData, webServiceUri.ServerVersion));
			}
			AutoDiscoverQuery.AutoDiscoverTracer.TraceDebug<object, Uri, EmailAddress>((long)this.GetHashCode(), "{0}: Found availability service {1} that can fill request for mailbox {2}", TraceContext.Get(), webServiceUri.Uri, (recipientData != null) ? recipientData.EmailAddress : null);
			SerializedSecurityContext serializedSecurityContext = null;
			InternalClientContext internalClientContext = base.ClientContext as InternalClientContext;
			if (this.targetForestConfiguration.IsPerUserAuthorizationSupported && internalClientContext != null)
			{
				serializedSecurityContext = internalClientContext.SerializedSecurityContext;
			}
			ProxyAuthenticator proxyAuthenticatorForAutoDiscover = this.targetForestConfiguration.GetProxyAuthenticatorForAutoDiscover(webServiceUri.Uri, serializedSecurityContext, base.ClientContext.MessageId);
			return new AutoDiscoverResult(webServiceUri, proxyAuthenticatorForAutoDiscover);
		}

		private TargetForestConfiguration targetForestConfiguration;
	}
}

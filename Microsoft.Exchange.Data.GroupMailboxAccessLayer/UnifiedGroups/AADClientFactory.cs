using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.UnifiedGroups
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AADClientFactory
	{
		public static bool IsAADEnabled()
		{
			bool result;
			try
			{
				result = (AADClientFactory.graphUrl.Value != null);
			}
			catch (LocalizedException)
			{
				result = false;
			}
			return result;
		}

		public static AADClient Create(ADUser user)
		{
			if (AADClientFactory.graphUrl.Value == null)
			{
				AADClientFactory.Tracer.TraceDebug(0L, "No GraphURL available, cannot create AADClient");
				return null;
			}
			return new AADClient(AADClientFactory.graphUrl.Value, AADClientFactory.GetTenantContextId(user.OrganizationId), AADClientFactory.GetActAsUserCredentials(user), GraphProxyVersions.Version14);
		}

		public static IAadClient CreateIAadClient(ADUser user)
		{
			if (AADClientTestHooks.GraphApi_GetAadClient != null)
			{
				return AADClientTestHooks.GraphApi_GetAadClient();
			}
			if (AADClientFactory.graphUrl.Value == null)
			{
				AADClientFactory.Tracer.TraceDebug(0L, "No GraphURL available, cannot create AADClient");
				return null;
			}
			return new AADClient(AADClientFactory.graphUrl.Value, AADClientFactory.GetTenantContextId(user.OrganizationId), AADClientFactory.GetActAsUserCredentials(user), GraphProxyVersions.Version14);
		}

		public static AADClient Create(OrganizationId organizationId, GraphProxyVersions apiVersion = GraphProxyVersions.Version14)
		{
			if (AADClientFactory.graphUrl.Value == null)
			{
				AADClientFactory.Tracer.TraceDebug(0L, "No GraphURL available, cannot create AADClient");
				return null;
			}
			return new AADClient(AADClientFactory.graphUrl.Value, AADClientFactory.GetTenantContextId(organizationId), AADClientFactory.GetAppCredentials(organizationId), apiVersion);
		}

		public static AADClient Create(string smtpAddress, GraphProxyVersions apiVersion = GraphProxyVersions.Version14)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("smtpAddress", smtpAddress);
			SmtpAddress smtpAddress2 = new SmtpAddress(smtpAddress);
			if (!smtpAddress2.IsValidAddress)
			{
				AADClientFactory.Tracer.TraceDebug<string>(0L, "SMTP address {0} is not valid, cannot create AADClient", smtpAddress);
				return null;
			}
			OrganizationId organizationId = OrganizationId.FromAcceptedDomain(smtpAddress2.Domain);
			return AADClientFactory.Create(organizationId, apiVersion);
		}

		private static string GetTenantContextId(OrganizationId organizationId)
		{
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				return Guid.Empty.ToString();
			}
			return new Guid(organizationId.ToExternalDirectoryOrganizationId()).ToString();
		}

		private static string GetGraphUrl()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 168, "GetGraphUrl", "f:\\15.00.1497\\sources\\dev\\UnifiedGroups\\src\\UnifiedGroups\\Common\\AADClientFactory.cs");
			ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
			ServiceEndpoint endpoint;
			try
			{
				endpoint = endpointContainer.GetEndpoint(ServiceEndpointId.AADGraphAPI);
			}
			catch (ServiceEndpointNotFoundException)
			{
				AADClientFactory.Tracer.TraceDebug(0L, "Unable to get the URL for the Graph API because the service endpoint was not found");
				return null;
			}
			if (endpoint.Uri == null)
			{
				AADClientFactory.Tracer.TraceError(0L, "ServiceEndpoint for Graph API was found but the URL is not present");
				throw new ServiceEndpointNotFoundException(ServiceEndpointId.AADGraphAPI);
			}
			string text = endpoint.Uri.ToString();
			AADClientFactory.Tracer.TraceDebug<string>(0L, "Retrieved GraphURL from ServiceEndpoint: {0}", text);
			return text;
		}

		private static ICredentials GetAppCredentials(OrganizationId organizationId)
		{
			ICredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(organizationId, "dummyRealm");
			AADClientFactory.Tracer.TraceDebug<OrganizationId, ICredentials>(0L, "Created app credentials for {0}: {1}", organizationId, oauthCredentialsForAppToken);
			return oauthCredentialsForAppToken;
		}

		private static ICredentials GetActAsUserCredentials(ADUser user)
		{
			ICredentials oauthCredentialsForAppActAsToken = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(user.OrganizationId, user, null);
			AADClientFactory.Tracer.TraceDebug<string, ICredentials>(0L, "Created user credentials for {0}: {1}", user.UserPrincipalName, oauthCredentialsForAppActAsToken);
			return oauthCredentialsForAppActAsToken;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private static Lazy<string> graphUrl = new Lazy<string>(new Func<string>(AADClientFactory.GetGraphUrl), LazyThreadSafetyMode.PublicationOnly);
	}
}

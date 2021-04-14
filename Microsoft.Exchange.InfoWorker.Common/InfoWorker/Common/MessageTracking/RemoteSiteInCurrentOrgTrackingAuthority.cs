using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class RemoteSiteInCurrentOrgTrackingAuthority : ADAuthenticationTrackingAuthority
	{
		public ADObjectId SiteADObjectId
		{
			get
			{
				return this.siteADObjectId;
			}
		}

		public int ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		public static RemoteSiteInCurrentOrgTrackingAuthority Create(ADObjectId siteADObjectId, DirectoryContext directoryContext, int minServerVersionRequested, bool useServersCache)
		{
			string text = ServerCache.Instance.GetDefaultDomain(directoryContext.OrganizationId);
			int version = 0;
			Uri uri = null;
			if (useServersCache)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<ADObjectId>(0, "Creating remote tracking authority via ServersCache for site: {0}", siteADObjectId);
				try
				{
					MiniServer anyBackEndServerFromASite = ServersCache.GetAnyBackEndServerFromASite(siteADObjectId, minServerVersionRequested, false);
					version = anyBackEndServerFromASite.VersionNumber;
					TraceWrapper.SearchLibraryTracer.TraceDebug<string, ServerVersion>(0, "Found remote server {0} running {1}.", anyBackEndServerFromASite.Fqdn, anyBackEndServerFromASite.AdminDisplayVersion);
					if (anyBackEndServerFromASite.MajorVersion >= 15)
					{
						BackEndServer backEndServer = new BackEndServer(anyBackEndServerFromASite.Fqdn, version);
						uri = BackEndLocator.GetBackEndWebServicesUrl(backEndServer);
					}
					else
					{
						TraceWrapper.SearchLibraryTracer.TraceDebug(0, "Server found was E14, finding new tracking authority via ServiceTopology.", new object[0]);
						uri = ServerCache.Instance.GetCasServerUri(siteADObjectId, minServerVersionRequested, out version);
					}
					goto IL_118;
				}
				catch (BackEndLocatorException ex)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "Failed to acquire EWS URI from BackEndLocator with exception: {0}", ex.ToString());
					TrackingFatalException.RaiseET(ErrorCode.CASUriDiscoveryFailure, siteADObjectId.ToString());
					goto IL_118;
				}
				catch (ServerHasNotBeenFoundException ex2)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "Failed to locate remote backend server from ServersCache with exception: {0}", ex2.ToString());
					TrackingFatalException.RaiseET(ErrorCode.CASUriDiscoveryFailure, siteADObjectId.ToString());
					goto IL_118;
				}
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug(0, "Creating remote tracking authority via ServiceTopology.", new object[0]);
			uri = ServerCache.Instance.GetCasServerUri(siteADObjectId, minServerVersionRequested, out version);
			IL_118:
			if (null == uri)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "No suitable authority URI found.", new object[0]);
				TrackingFatalException.RaiseET(ErrorCode.CASUriDiscoveryFailure, siteADObjectId.ToString());
			}
			return new RemoteSiteInCurrentOrgTrackingAuthority(text, TrackingAuthorityKind.RemoteSiteInCurrentOrg, siteADObjectId, uri, version);
		}

		public static RemoteSiteInCurrentOrgTrackingAuthority Create(ServerInfo serverInfo, DirectoryContext directoryContext)
		{
			int version = serverInfo.AdminDisplayVersion.ToInt();
			TraceWrapper.SearchLibraryTracer.TraceDebug<string, ServerVersion>(0, "Creating remote tracking authority via BackEndLocator for server {0} running {1}.", serverInfo.Key, serverInfo.AdminDisplayVersion);
			string text = ServerCache.Instance.GetDefaultDomain(directoryContext.OrganizationId);
			BackEndServer backEndServer = new BackEndServer(serverInfo.Key, version);
			Uri backEndWebServicesUrl = BackEndLocator.GetBackEndWebServicesUrl(backEndServer);
			if (null == backEndWebServicesUrl)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "No suitable authority URI found.", new object[0]);
				TrackingFatalException.RaiseET(ErrorCode.CASUriDiscoveryFailure, serverInfo.ServerSiteId.ToString());
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<Uri>(0, "Using EWS URI: {0}", backEndWebServicesUrl);
			return new RemoteSiteInCurrentOrgTrackingAuthority(text, TrackingAuthorityKind.RemoteSiteInCurrentOrg, serverInfo.ServerSiteId, backEndWebServicesUrl, version);
		}

		public static RemoteSiteInCurrentOrgTrackingAuthority Create(ADObjectId siteADObjectId, DirectoryContext directoryContext, ADUser user)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug(0, "Creating remote tracking authority via BackEndLocator for user.", new object[0]);
			string text = ServerCache.Instance.GetDefaultDomain(directoryContext.OrganizationId);
			BackEndServer backEndServer = BackEndLocator.GetBackEndServer(user);
			Uri backEndWebServicesUrl = BackEndLocator.GetBackEndWebServicesUrl(backEndServer);
			int version = backEndServer.Version;
			if (null == backEndWebServicesUrl)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "No suitable authority URI found.", new object[0]);
				TrackingFatalException.RaiseET(ErrorCode.CASUriDiscoveryFailure, siteADObjectId.ToString());
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<Uri>(0, "Using EWS URI: {0}", backEndWebServicesUrl);
			return new RemoteSiteInCurrentOrgTrackingAuthority(text, TrackingAuthorityKind.RemoteSiteInCurrentOrg, siteADObjectId, backEndWebServicesUrl, version);
		}

		public override bool IsAllowedScope(SearchScope scope)
		{
			return scope == SearchScope.Forest || scope == SearchScope.Organization || scope == SearchScope.World;
		}

		public override string ToString()
		{
			return string.Format("Type=RemoteSiteInCurrentOrgTrackingAuthority,Site={0}", this.siteADObjectId);
		}

		public override SearchScope AssociatedScope
		{
			get
			{
				return SearchScope.Site;
			}
		}

		public override string Domain
		{
			get
			{
				return this.defaultDomain;
			}
		}

		private RemoteSiteInCurrentOrgTrackingAuthority(string defaultDomain, TrackingAuthorityKind responsibleTracker, ADObjectId siteADObjectId, Uri casServerUri, int serverVersion) : base(responsibleTracker, casServerUri)
		{
			this.defaultDomain = defaultDomain;
			this.siteADObjectId = siteADObjectId;
			this.serverVersion = serverVersion;
		}

		private string defaultDomain;

		private ADObjectId siteADObjectId;

		private int serverVersion;
	}
}

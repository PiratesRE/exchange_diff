using System;
using System.Web.Hosting;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class VirtualDirectoryConfiguration
	{
		public static bool IsClientCertificateRequired(Uri address)
		{
			return (VirtualDirectoryConfiguration.LoadAccessSslFlags(address.LocalPath) & MetabasePropertyTypes.AccessSSLFlags.AccessSSLRequireCert) != MetabasePropertyTypes.AccessSSLFlags.None;
		}

		private static MetabasePropertyTypes.AccessSSLFlags LoadAccessSslFlags(string localPath)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
				string text = HostingEnvironment.SiteName + localPath;
				ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/security/access", text);
				if (section != null && section["sslFlags"] != null)
				{
					return (MetabasePropertyTypes.AccessSSLFlags)((int)section["sslFlags"]);
				}
			}
			return MetabasePropertyTypes.AccessSSLFlags.None;
		}

		internal static Uri GetFailBackUrl(EcpService ecpService)
		{
			Uri uri = null;
			if (VirtualDirectoryConfiguration.failbackUrlCache.TryGetValue(ecpService.MetabasePath, out uri))
			{
				return uri;
			}
			string webSiteRoot = IisUtility.GetWebSiteRoot(ecpService.MetabasePath);
			foreach (ADOwaVirtualDirectory adowaVirtualDirectory in VirtualDirectoryConfiguration.FindVirtualDirectoriesForServer<ADOwaVirtualDirectory>(ecpService.ServerFullyQualifiedDomainName))
			{
				if (webSiteRoot.Equals(IisUtility.GetWebSiteRoot(adowaVirtualDirectory.MetabasePath), StringComparison.OrdinalIgnoreCase) && adowaVirtualDirectory.FailbackUrl != null)
				{
					uri = new UriBuilder(adowaVirtualDirectory.FailbackUrl)
					{
						Path = ecpService.Url.AbsolutePath,
						Query = ecpService.Url.Query
					}.Uri;
				}
			}
			VirtualDirectoryConfiguration.failbackUrlCache[ecpService.MetabasePath] = uri;
			return uri;
		}

		private static T[] FindVirtualDirectoriesForServer<T>(string serverFqdn) where T : ExchangeVirtualDirectory, new()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 124, "FindVirtualDirectoriesForServer", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\WebServices\\VirtualDirectoryConfiguration.cs");
			Server server = string.IsNullOrEmpty(serverFqdn) ? topologyConfigurationSession.FindLocalServer() : topologyConfigurationSession.FindServerByFqdn(serverFqdn);
			if (server == null)
			{
				return new T[0];
			}
			return topologyConfigurationSession.Find<T>(server.Id, QueryScope.SubTree, null, null, 0);
		}

		public static bool GetEcpAnonymousAuthenticationStatus()
		{
			using (ServerManager serverManager = new ServerManager())
			{
				Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
				string text = HostingEnvironment.SiteName + HostingEnvironment.ApplicationVirtualPath;
				ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/anonymousAuthentication", text);
				if (section != null && true.Equals(section["enabled"]))
				{
					return true;
				}
			}
			return false;
		}

		private const string AnonymousAuthenticationSection = "system.webServer/security/authentication/anonymousAuthentication";

		private static readonly MruDictionaryCache<string, Uri> failbackUrlCache = new MruDictionaryCache<string, Uri>(100, 30);

		public static bool EcpVirtualDirectoryAnonymousAuthenticationEnabled = true;
	}
}

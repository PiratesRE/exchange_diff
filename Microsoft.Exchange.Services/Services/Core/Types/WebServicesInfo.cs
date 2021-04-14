using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class WebServicesInfo
	{
		public Uri Url { get; private set; }

		public string ServerFullyQualifiedDomainName { get; private set; }

		public int ServerVersionNumber { get; private set; }

		public string SiteDistinguishedName { get; private set; }

		public string ServerDistinguishedName { get; private set; }

		public bool IsCafeUrl { get; private set; }

		private WebServicesInfo(Uri url, string serverFqdn, int serverVersionNumber, bool isCafeUrl, string serverDistinguishedName = null, string siteDistinguishedName = null)
		{
			this.Url = url;
			this.ServerFullyQualifiedDomainName = serverFqdn;
			this.ServerVersionNumber = ((serverVersionNumber >= Server.E15MinVersion) ? Server.E15MinVersion : serverVersionNumber);
			this.ServerDistinguishedName = serverDistinguishedName;
			this.SiteDistinguishedName = siteDistinguishedName;
			this.IsCafeUrl = isCafeUrl;
		}

		public static WebServicesInfo CreateFromWebServicesService(WebServicesService webServicesService)
		{
			return new WebServicesInfo(webServicesService.Url, webServicesService.ServerFullyQualifiedDomainName, webServicesService.ServerVersionNumber, false, webServicesService.ServerDistinguishedName, webServicesService.Site.DistinguishedName);
		}

		public static WebServicesInfo Create(Uri url, string serverFqdn, int serverVersionNumber, bool isCafeUrl)
		{
			return new WebServicesInfo(url, serverFqdn, serverVersionNumber, isCafeUrl, null, null);
		}
	}
}

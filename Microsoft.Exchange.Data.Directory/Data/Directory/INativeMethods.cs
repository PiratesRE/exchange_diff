using System;
using System.Collections.Specialized;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface INativeMethods
	{
		string GetSiteNameHookable(bool throwOnErrorNoSite = false);

		string GetDomainNameHookable();

		string GetForestNameHookable();

		StringCollection FindAllDomainControllersHookable(string domainFqdn, string siteName);

		StringCollection FindAllGlobalCatalogsHookable(string forestFqdn, string siteName = null);

		string ResetSecureChannelDCForDomainHookable(string domainFqdn, bool throwOnError = true);

		string GetSecureChannelDCForDomainHookable(string domainFqdn, bool throwOnError = true);
	}
}

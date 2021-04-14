using System;

namespace System.Security.Principal
{
	internal enum IdentifierAuthority : long
	{
		NullAuthority,
		WorldAuthority,
		LocalAuthority,
		CreatorAuthority,
		NonUniqueAuthority,
		NTAuthority,
		SiteServerAuthority,
		InternetSiteAuthority,
		ExchangeAuthority,
		ResourceManagerAuthority
	}
}

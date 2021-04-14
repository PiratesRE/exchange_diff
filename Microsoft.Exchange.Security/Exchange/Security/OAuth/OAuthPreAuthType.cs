using System;

namespace Microsoft.Exchange.Security.OAuth
{
	internal enum OAuthPreAuthType
	{
		OrganizationOnly,
		Smtp,
		WindowsLiveID,
		ExternalDirectoryObjectId,
		Puid
	}
}

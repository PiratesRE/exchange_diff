using System;

namespace Microsoft.Exchange.Security.Authentication
{
	internal delegate SecurityStatus ExternalProxyAuthentication(byte[] userid, byte[] password, Guid requestId, out string commonAccessToken, out IAccountValidationContext accountValidationContext);
}

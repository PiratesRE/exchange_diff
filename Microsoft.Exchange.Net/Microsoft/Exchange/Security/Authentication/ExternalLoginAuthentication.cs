using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authentication
{
	internal delegate SecurityStatus ExternalLoginAuthentication(byte[] userid, byte[] password, out WindowsIdentity windowsIdentity, out IAccountValidationContext accountValidationContext);
}

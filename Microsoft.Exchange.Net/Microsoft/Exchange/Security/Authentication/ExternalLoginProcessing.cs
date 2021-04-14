using System;

namespace Microsoft.Exchange.Security.Authentication
{
	internal delegate void ExternalLoginProcessing(byte[] domainName, byte[] userName, byte[] password);
}

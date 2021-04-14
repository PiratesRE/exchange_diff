using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal enum RevocationStatus : uint
	{
		Valid,
		Revoked = 4U,
		Unknown = 64U
	}
}

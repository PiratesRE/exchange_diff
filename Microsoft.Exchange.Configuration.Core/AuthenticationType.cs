using System;

namespace Microsoft.Exchange.Configuration.Core
{
	public enum AuthenticationType
	{
		Unknown,
		LiveIdBasic,
		LiveIdNego2,
		Certificate,
		CertificateLinkedUser,
		OAuth,
		Kerberos,
		RemotePowerShellDelegated
	}
}

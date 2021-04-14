using System;

namespace Microsoft.Exchange.Management.Metabase
{
	internal enum AuthnLevel
	{
		Default,
		None,
		Connect,
		Call,
		Pkt,
		PktIntegrity,
		PktPrivacy
	}
}

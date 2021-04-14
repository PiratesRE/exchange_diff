using System;

namespace Microsoft.Exchange.Management.Metabase
{
	internal enum Authn
	{
		None,
		DcePrivate,
		DcePublic,
		DecPublic = 4,
		GssNegotiate = 9,
		Winnt,
		GssSchannel = 14,
		GssKerberos = 16,
		Msn,
		Dpa,
		Mq = 100,
		Default = -1
	}
}

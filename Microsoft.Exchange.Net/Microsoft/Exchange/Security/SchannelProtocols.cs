using System;

namespace Microsoft.Exchange.Security
{
	[Flags]
	internal enum SchannelProtocols
	{
		Zero = 0,
		SP_PROT_PCT1_SERVER = 1,
		SP_PROT_PCT1_CLIENT = 2,
		SP_PROT_SSL2_SERVER = 4,
		SP_PROT_SSL2_CLIENT = 8,
		SP_PROT_SSL3_SERVER = 16,
		SP_PROT_SSL3_CLIENT = 32,
		SP_PROT_TLS1_0_SERVER = 64,
		SP_PROT_TLS1_0_CLIENT = 128,
		SP_PROT_TLS1_1_SERVER = 256,
		SP_PROT_TLS1_1_CLIENT = 512,
		SP_PROT_TLS1_2_SERVER = 1024,
		SP_PROT_TLS1_2_CLIENT = 2048
	}
}

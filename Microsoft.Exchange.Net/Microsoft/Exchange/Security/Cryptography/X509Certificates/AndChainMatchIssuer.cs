using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class AndChainMatchIssuer : ChainMatchIssuer
	{
		public AndChainMatchIssuer(Oid[] oids) : base(ChainMatchIssuer.Operator.And, oids)
		{
		}

		public static AndChainMatchIssuer PkixKpServerAuth = new AndChainMatchIssuer(new Oid[]
		{
			WellKnownOid.PkixKpServerAuth
		});

		public static AndChainMatchIssuer EmailProtection = new AndChainMatchIssuer(new Oid[]
		{
			WellKnownOid.EmailProtection
		});
	}
}

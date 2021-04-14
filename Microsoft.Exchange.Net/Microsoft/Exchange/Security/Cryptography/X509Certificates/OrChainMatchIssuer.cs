using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class OrChainMatchIssuer : ChainMatchIssuer
	{
		public OrChainMatchIssuer(Oid[] oids) : base(ChainMatchIssuer.Operator.Or, oids)
		{
		}
	}
}

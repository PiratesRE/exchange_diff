using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class ProofTokenNotFoundException : WSTrustException
	{
		public ProofTokenNotFoundException() : base(WSTrustStrings.ProofTokenNotFoundException)
		{
		}
	}
}

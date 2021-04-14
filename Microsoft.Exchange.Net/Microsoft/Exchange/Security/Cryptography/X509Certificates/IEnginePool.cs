using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal interface IEnginePool
	{
		void ReturnTo(SafeChainEngineHandle item);
	}
}

using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public interface IMapiObjectCounter
	{
		long GetCount();

		void IncrementCount();

		void DecrementCount();

		void CheckObjectQuota(bool mustBeStrictlyUnderQuota);
	}
}

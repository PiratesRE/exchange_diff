using System;

namespace Microsoft.Exchange.AirSync
{
	public interface IThreadIdProvider
	{
		int ManagedThreadId { get; }
	}
}

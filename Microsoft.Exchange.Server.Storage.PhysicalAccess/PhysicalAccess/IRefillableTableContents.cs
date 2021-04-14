using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IRefillableTableContents
	{
		bool CanRefill { get; }

		void MarkChunkConsumed();
	}
}

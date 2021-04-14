using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IStoreObjectChange
	{
		PropertyUpdate[] PropertyUpdates { get; set; }
	}
}

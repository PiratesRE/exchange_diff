using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public enum WellKnownCommandName
	{
		Unknown,
		CreateKey,
		DeleteKey,
		SetProperty,
		DeleteProperty,
		ExecuteBatch,
		ApplySnapshot,
		PromoteToLeader,
		DummyCmd
	}
}

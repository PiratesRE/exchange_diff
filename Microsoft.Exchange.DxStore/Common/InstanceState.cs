using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public enum InstanceState
	{
		Unknown,
		Initialized,
		Starting,
		Running,
		Failed,
		Stopping
	}
}

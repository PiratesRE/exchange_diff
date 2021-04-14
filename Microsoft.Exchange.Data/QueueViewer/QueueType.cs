using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public enum QueueType
	{
		Undefined,
		Delivery,
		Poison,
		Submission,
		Unreachable,
		Shadow
	}
}

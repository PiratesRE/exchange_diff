using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum IdProcessingInstruction : byte
	{
		Normal,
		Recurrence,
		Series
	}
}

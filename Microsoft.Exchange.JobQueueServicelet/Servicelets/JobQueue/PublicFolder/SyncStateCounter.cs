using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncStateCounter
	{
		public long BytesSent { get; set; }

		public long BytesReceived { get; set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"BytesSent=",
				this.BytesSent,
				",BytesReceived=",
				this.BytesReceived
			});
		}
	}
}

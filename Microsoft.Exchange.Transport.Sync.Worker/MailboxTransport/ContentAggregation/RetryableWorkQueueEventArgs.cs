using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RetryableWorkQueueEventArgs : EventArgs
	{
		internal RetryableWorkQueueEventArgs(int difference)
		{
			this.difference = difference;
		}

		internal int Difference
		{
			get
			{
				return this.difference;
			}
		}

		internal static readonly RetryableWorkQueueEventArgs IncrementByOneEventArgs = new RetryableWorkQueueEventArgs(1);

		internal static readonly RetryableWorkQueueEventArgs DecrementByOneEventArgs = new RetryableWorkQueueEventArgs(-1);

		private int difference;
	}
}

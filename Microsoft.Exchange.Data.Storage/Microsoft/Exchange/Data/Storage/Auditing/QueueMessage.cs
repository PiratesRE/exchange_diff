using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class QueueMessage<T> : IQueueMessage<T>
	{
		public T Data { get; set; }
	}
}

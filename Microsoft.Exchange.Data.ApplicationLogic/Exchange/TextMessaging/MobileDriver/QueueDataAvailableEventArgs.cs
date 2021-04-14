using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class QueueDataAvailableEventArgs<T> : EventArgs
	{
		public QueueDataAvailableEventArgs(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.Item = item;
		}

		public T Item { get; private set; }
	}
}

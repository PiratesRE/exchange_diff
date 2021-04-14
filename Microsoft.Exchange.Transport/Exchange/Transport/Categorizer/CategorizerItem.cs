using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class CategorizerItem
	{
		public CategorizerItem(TransportMailItem mailItem, int stage)
		{
			this.mailItem = mailItem;
			this.stage = stage;
		}

		public TransportMailItem TransportMailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		public int Stage
		{
			get
			{
				return this.stage;
			}
		}

		public void Reset(TransportMailItem mailItem, int stage)
		{
			this.mailItem = mailItem;
			this.stage = stage;
		}

		public const int QueuedLockedStageId = -1;

		public const int QueuedUnsafeStageId = -2;

		private TransportMailItem mailItem;

		private int stage;
	}
}

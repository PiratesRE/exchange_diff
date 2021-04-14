using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class MservChange
	{
		public MservRecord NewValue { get; private set; }

		public MservRecord OldValue { get; private set; }

		public MservChange(MservRecord newValue, MservRecord oldValue)
		{
			this.NewValue = newValue;
			this.OldValue = oldValue;
		}
	}
}

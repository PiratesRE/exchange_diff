using System;

namespace Microsoft.Exchange.Transport
{
	internal class PickupContext : PoisonContext
	{
		public PickupContext(string pickupFileName) : base(MessageProcessingSource.Pickup)
		{
			this.fileName = pickupFileName;
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		private string fileName;
	}
}

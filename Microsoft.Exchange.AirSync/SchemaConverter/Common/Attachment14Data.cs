using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class Attachment14Data : Attachment12Data
	{
		public Attachment14Data()
		{
			this.Duration = -1;
			this.Order = -1;
		}

		public int Duration { get; set; }

		public int Order { get; set; }
	}
}

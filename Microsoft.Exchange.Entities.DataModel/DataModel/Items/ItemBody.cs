using System;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public class ItemBody
	{
		public BodyType ContentType { get; set; }

		public string Content { get; set; }
	}
}

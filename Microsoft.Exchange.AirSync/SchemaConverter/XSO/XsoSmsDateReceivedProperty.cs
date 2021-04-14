using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoSmsDateReceivedProperty : XsoUtcDateTimeProperty
	{
		public XsoSmsDateReceivedProperty() : base(ItemSchema.ReceivedTime, new PropertyDefinition[]
		{
			ItemSchema.ReceivedTime,
			ItemSchema.SentTime
		})
		{
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.InternalCopyFromModified(srcProperty);
			base.XsoItem[ItemSchema.SentTime] = base.XsoItem[ItemSchema.ReceivedTime];
		}
	}
}

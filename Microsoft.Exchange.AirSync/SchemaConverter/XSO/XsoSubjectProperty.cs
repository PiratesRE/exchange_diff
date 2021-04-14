using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoSubjectProperty : XsoStringProperty
	{
		public XsoSubjectProperty() : base(ItemSchema.Subject)
		{
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			base.XsoItem.DeleteProperties(new PropertyDefinition[]
			{
				base.PropertyDef
			});
		}
	}
}

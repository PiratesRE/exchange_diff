using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoOrganizerNameProperty : XsoStringProperty
	{
		public XsoOrganizerNameProperty() : base(ItemSchema.SentRepresentingDisplayName)
		{
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
		}
	}
}

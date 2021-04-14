using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoMeetingStatusProperty : XsoIntegerProperty
	{
		public XsoMeetingStatusProperty(StorePropertyDefinition propertyDef) : base(propertyDef, PropertyType.ReadOnly)
		{
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
		}
	}
}

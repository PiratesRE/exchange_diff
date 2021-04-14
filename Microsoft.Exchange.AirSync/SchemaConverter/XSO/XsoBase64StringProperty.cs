using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoBase64StringProperty : XsoStringProperty
	{
		public XsoBase64StringProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public override string StringData
		{
			get
			{
				return Convert.ToBase64String((byte[])base.XsoItem.TryGetProperty(base.PropertyDef));
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = Convert.FromBase64String(((IStringProperty)srcProperty).StringData);
		}
	}
}

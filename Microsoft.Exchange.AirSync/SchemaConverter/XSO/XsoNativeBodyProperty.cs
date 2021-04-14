using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoNativeBodyProperty : XsoIntegerProperty
	{
		public XsoNativeBodyProperty() : base(null, PropertyType.ReadOnly)
		{
		}

		public override int IntegerData
		{
			get
			{
				Item item = base.XsoItem as Item;
				if (item == null)
				{
					return 0;
				}
				BodyType result;
				switch (item.Body.Format)
				{
				case BodyFormat.TextPlain:
					result = BodyType.PlainText;
					break;
				case BodyFormat.TextHtml:
					result = BodyType.Html;
					break;
				case BodyFormat.ApplicationRtf:
					result = BodyType.Rtf;
					break;
				default:
					throw new ConversionException("Unknown BodyFormat implemented by XSO");
				}
				return (int)result;
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
		}
	}
}

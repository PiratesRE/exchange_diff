using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoEvmCallerIdProperty : XsoEvmStringProperty
	{
		public XsoEvmCallerIdProperty(PropertyType type) : base(null, type, new PropertyDefinition[]
		{
			MessageItemSchema.SenderTelephoneNumber,
			MessageItemSchema.PstnCallbackTelephoneNumber
		})
		{
		}

		public override string StringData
		{
			get
			{
				string text = base.XsoItem.TryGetProperty(MessageItemSchema.PstnCallbackTelephoneNumber) as string;
				if (string.IsNullOrEmpty(text))
				{
					text = (base.XsoItem.TryGetProperty(MessageItemSchema.SenderTelephoneNumber) as string);
				}
				if (text != null && text.IndexOf("@", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					text = string.Empty;
				}
				if (string.IsNullOrEmpty(text))
				{
					base.State = PropertyState.SetToDefault;
				}
				return text;
			}
		}
	}
}

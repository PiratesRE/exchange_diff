using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoMessageClassProperty : XsoStringProperty
	{
		public XsoMessageClassProperty(PropertyType type) : base(null, type, new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass
		})
		{
		}

		public override string StringData
		{
			get
			{
				if (this.IsItemDelegated())
				{
					return "IPM.Note";
				}
				string text = base.XsoItem.GetValueOrDefault<string>(StoreObjectSchema.ItemClass);
				if (BodyConversionUtilities.IsMessageRestrictedAndDecoded((Item)base.XsoItem) && text != null && text.StartsWith("IPM.Note.RPMSG.Microsoft.Voicemail", StringComparison.OrdinalIgnoreCase))
				{
					text = text.ToLower();
					text = text.Replace(".rpmsg.", ".");
				}
				return text;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			throw new ConversionException("Message-class is a read-only property and should not be set!");
		}
	}
}

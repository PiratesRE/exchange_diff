using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.RecipientInfoCache
{
	internal class RecipientInfoCacheStringProperty : RecipientInfoCacheProperty, IStringProperty, IProperty
	{
		public RecipientInfoCacheStringProperty(RecipientInfoCacheEntryElements element)
		{
			switch (element)
			{
			case RecipientInfoCacheEntryElements.EmailAddress:
			case RecipientInfoCacheEntryElements.DisplayName:
			case RecipientInfoCacheEntryElements.Alias:
				base.State = PropertyState.Modified;
				this.element = element;
				return;
			default:
				throw new ArgumentException("The element " + element + " is not a string type!");
			}
		}

		public string StringData
		{
			get
			{
				string result = null;
				switch (this.element)
				{
				case RecipientInfoCacheEntryElements.EmailAddress:
					result = this.entry.SmtpAddress;
					break;
				case RecipientInfoCacheEntryElements.DisplayName:
					result = this.entry.DisplayName;
					break;
				case RecipientInfoCacheEntryElements.Alias:
					result = this.entry.Alias;
					break;
				}
				return result;
			}
		}

		public override void Bind(RecipientInfoCacheEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("Entry is null!");
			}
			this.entry = entry;
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			IStringProperty stringProperty = srcProperty as IStringProperty;
			if (stringProperty == null)
			{
				throw new UnexpectedTypeException("IStringProperty", srcProperty);
			}
			if (this.entry == null)
			{
				throw new ConversionException("Haven't been bound to an item yet! Element is: " + this.element);
			}
			switch (this.element)
			{
			case RecipientInfoCacheEntryElements.EmailAddress:
				throw new NotImplementedException("Can't change the email address!");
			case RecipientInfoCacheEntryElements.DisplayName:
				this.entry.DisplayName = stringProperty.StringData;
				return;
			case RecipientInfoCacheEntryElements.Alias:
				this.entry.Alias = stringProperty.StringData;
				return;
			default:
				return;
			}
		}

		private RecipientInfoCacheEntryElements element;

		private RecipientInfoCacheEntry entry;
	}
}

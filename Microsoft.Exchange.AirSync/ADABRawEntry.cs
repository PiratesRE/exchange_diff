using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ADABRawEntry : ABRawEntry
	{
		public ADABRawEntry(ADABSession ownerSession, ABPropertyDefinitionCollection properties, ADRawEntry rawEntry) : base(ownerSession, properties)
		{
			if (rawEntry == null)
			{
				throw new ArgumentNullException("rawEntry");
			}
			this.rawEntry = rawEntry;
		}

		protected override bool InternalTryGetValue(ABPropertyDefinition property, out object value)
		{
			ADPropertyDefinition adpropertyDefinition = ADABPropertyMapper.GetADPropertyDefinition(property);
			if (adpropertyDefinition != null)
			{
				return this.rawEntry.TryGetValueWithoutDefault(adpropertyDefinition, out value);
			}
			ADABObjectId adabobjectId;
			if (property == ABObjectSchema.Id && ADABUtils.GetId(this.rawEntry, out adabobjectId))
			{
				value = adabobjectId;
				return true;
			}
			string text;
			if (property == ABObjectSchema.EmailAddress && ADABUtils.GetEmailAddress(this.rawEntry, out text))
			{
				value = text;
				return true;
			}
			object obj;
			if (property == ABObjectSchema.CanEmail && this.rawEntry.TryGetValueWithoutDefault(ADRecipientSchema.RecipientType, out obj))
			{
				value = ADABUtils.CanEmailRecipientType((RecipientType)obj);
				return true;
			}
			if (property == ABGroupSchema.MembersCount)
			{
				value = null;
				return true;
			}
			ADABObjectId adabobjectId2;
			if (property == ABGroupSchema.OwnerId && ADABUtils.GetOwnerId(this.rawEntry, out adabobjectId2))
			{
				value = adabobjectId2;
				return true;
			}
			return base.InternalTryGetValue(property, out value);
		}

		private ADRawEntry rawEntry;
	}
}

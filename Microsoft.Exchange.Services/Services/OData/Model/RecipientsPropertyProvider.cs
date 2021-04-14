using System;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RecipientsPropertyProvider : EwsPropertyProvider
	{
		public RecipientsPropertyProvider(PropertyInformation propertyInformation) : base(propertyInformation)
		{
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			EmailAddressWrapper[] valueOrDefault = ewsObject.GetValueOrDefault<EmailAddressWrapper[]>(base.PropertyInformation);
			object value;
			if (valueOrDefault != null)
			{
				value = Array.ConvertAll<EmailAddressWrapper, Recipient>(valueOrDefault, (EmailAddressWrapper x) => x.ToRecipient());
			}
			else
			{
				value = null;
			}
			entity[property] = value;
		}

		protected override void SetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			ewsObject[base.PropertyInformation] = Array.ConvertAll<Recipient, EmailAddressWrapper>(entity[property] as Recipient[], (Recipient x) => x.ToEmailAddressWrapper());
		}
	}
}

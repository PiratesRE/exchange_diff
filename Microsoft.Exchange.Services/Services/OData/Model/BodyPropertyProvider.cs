using System;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class BodyPropertyProvider : EwsPropertyProvider
	{
		public BodyPropertyProvider(PropertyInformation propertyInforation) : base(propertyInforation)
		{
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			BodyContentType valueOrDefault = ewsObject.GetValueOrDefault<BodyContentType>(base.PropertyInformation);
			if (valueOrDefault != null)
			{
				entity[property] = valueOrDefault.ToItemBody();
			}
		}

		protected override void SetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			ItemBody itemBody = entity[property] as ItemBody;
			if (itemBody != null && !string.IsNullOrEmpty(itemBody.Content))
			{
				ewsObject[base.PropertyInformation] = itemBody.ToBodyContentType();
			}
		}
	}
}

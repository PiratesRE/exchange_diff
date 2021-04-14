using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public class RecipientSchema : TypeSchema
	{
		public RecipientSchema()
		{
			base.RegisterPropertyDefinition(RecipientSchema.StaticEmailAddressProperty);
			base.RegisterPropertyDefinition(RecipientSchema.StaticNameProperty);
			base.RegisterPropertyDefinition(RecipientSchema.StaticRoutingTypeProperty);
		}

		public TypedPropertyDefinition<string> EmailAddressProperty
		{
			get
			{
				return RecipientSchema.StaticEmailAddressProperty;
			}
		}

		public TypedPropertyDefinition<string> NameProperty
		{
			get
			{
				return RecipientSchema.StaticNameProperty;
			}
		}

		public TypedPropertyDefinition<string> RoutingTypeProperty
		{
			get
			{
				return RecipientSchema.StaticRoutingTypeProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticEmailAddressProperty = new TypedPropertyDefinition<string>("Recipient.EmailAddress", null, false);

		private static readonly TypedPropertyDefinition<string> StaticNameProperty = new TypedPropertyDefinition<string>("Recipient.Name", null, false);

		private static readonly TypedPropertyDefinition<string> StaticRoutingTypeProperty = new TypedPropertyDefinition<string>("Recipient.RoutingType", null, false);
	}
}

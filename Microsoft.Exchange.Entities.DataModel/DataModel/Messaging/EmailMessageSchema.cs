using System;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Messaging
{
	public abstract class EmailMessageSchema : ItemSchema
	{
		protected EmailMessageSchema()
		{
			base.RegisterPropertyDefinition(EmailMessageSchema.StaticIsReadProperty);
		}

		public TypedPropertyDefinition<bool> IsReadProperty
		{
			get
			{
				return EmailMessageSchema.StaticIsReadProperty;
			}
		}

		private static readonly TypedPropertyDefinition<bool> StaticIsReadProperty = new TypedPropertyDefinition<bool>("EmailMessage.IsRead", false, true);
	}
}

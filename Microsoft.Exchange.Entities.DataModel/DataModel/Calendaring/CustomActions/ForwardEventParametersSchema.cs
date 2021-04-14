using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class ForwardEventParametersSchema : EventWorkflowParametersSchema
	{
		public ForwardEventParametersSchema()
		{
			base.RegisterPropertyDefinition(ForwardEventParametersSchema.StaticForwardeesProperty);
		}

		public TypedPropertyDefinition<IList<Recipient<RecipientSchema>>> ForwardeesProperty
		{
			get
			{
				return ForwardEventParametersSchema.StaticForwardeesProperty;
			}
		}

		private static readonly TypedPropertyDefinition<IList<Recipient<RecipientSchema>>> StaticForwardeesProperty = new TypedPropertyDefinition<IList<Recipient<RecipientSchema>>>("ForwardEvent.Forwardees", null, true);
	}
}

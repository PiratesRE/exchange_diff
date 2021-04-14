using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class ForwardEventParameters : EventWorkflowParameters<ForwardEventParametersSchema>
	{
		public IList<Recipient<RecipientSchema>> Forwardees
		{
			get
			{
				return base.GetPropertyValueOrDefault<IList<Recipient<RecipientSchema>>>(base.Schema.ForwardeesProperty);
			}
			set
			{
				base.SetPropertyValue<IList<Recipient<RecipientSchema>>>(base.Schema.ForwardeesProperty, value);
			}
		}
	}
}

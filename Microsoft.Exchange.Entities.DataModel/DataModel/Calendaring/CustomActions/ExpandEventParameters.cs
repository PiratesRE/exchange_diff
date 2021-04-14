using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class ExpandEventParameters : SchematizedObject<ExpandEventParametersSchema>
	{
		public bool ReturnMaster
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.ReturnMasterProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.ReturnMasterProperty, value);
			}
		}

		public bool ReturnRegularOccurrences
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.ReturnRegularOccurrencesProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.ReturnRegularOccurrencesProperty, value);
			}
		}

		public bool ReturnExceptions
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.ReturnExceptionsProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.ReturnExceptionsProperty, value);
			}
		}

		public bool ReturnCancellations
		{
			get
			{
				return base.GetPropertyValueOrDefault<bool>(base.Schema.ReturnCancellationsProperty);
			}
			set
			{
				base.SetPropertyValue<bool>(base.Schema.ReturnCancellationsProperty, value);
			}
		}

		public ExDateTime WindowStart
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime>(base.Schema.WindowStartProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime>(base.Schema.WindowStartProperty, value);
			}
		}

		public ExDateTime WindowEnd
		{
			get
			{
				return base.GetPropertyValueOrDefault<ExDateTime>(base.Schema.WindowEndProperty);
			}
			set
			{
				base.SetPropertyValue<ExDateTime>(base.Schema.WindowEndProperty, value);
			}
		}
	}
}

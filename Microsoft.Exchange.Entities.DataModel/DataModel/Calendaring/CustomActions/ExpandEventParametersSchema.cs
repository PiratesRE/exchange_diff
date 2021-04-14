using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public sealed class ExpandEventParametersSchema : TypeSchema
	{
		public ExpandEventParametersSchema()
		{
			base.RegisterPropertyDefinition(ExpandEventParametersSchema.StaticReturnMasterProperty);
			base.RegisterPropertyDefinition(ExpandEventParametersSchema.StaticReturnRegularOccurrencesProperty);
			base.RegisterPropertyDefinition(ExpandEventParametersSchema.StaticReturnExceptionsProperty);
			base.RegisterPropertyDefinition(ExpandEventParametersSchema.StaticReturnCancellationsProperty);
			base.RegisterPropertyDefinition(ExpandEventParametersSchema.StaticWindowStartProperty);
			base.RegisterPropertyDefinition(ExpandEventParametersSchema.StaticWindowEndProperty);
		}

		public TypedPropertyDefinition<bool> ReturnMasterProperty
		{
			get
			{
				return ExpandEventParametersSchema.StaticReturnMasterProperty;
			}
		}

		public TypedPropertyDefinition<bool> ReturnRegularOccurrencesProperty
		{
			get
			{
				return ExpandEventParametersSchema.StaticReturnRegularOccurrencesProperty;
			}
		}

		public TypedPropertyDefinition<bool> ReturnExceptionsProperty
		{
			get
			{
				return ExpandEventParametersSchema.StaticReturnExceptionsProperty;
			}
		}

		public TypedPropertyDefinition<bool> ReturnCancellationsProperty
		{
			get
			{
				return ExpandEventParametersSchema.StaticReturnCancellationsProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> WindowStartProperty
		{
			get
			{
				return ExpandEventParametersSchema.StaticWindowStartProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> WindowEndProperty
		{
			get
			{
				return ExpandEventParametersSchema.StaticWindowEndProperty;
			}
		}

		private static readonly TypedPropertyDefinition<bool> StaticReturnMasterProperty = new TypedPropertyDefinition<bool>("ExpandEvent.ReturnMaster", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticReturnRegularOccurrencesProperty = new TypedPropertyDefinition<bool>("ExpandEvent.ReturnRegularOccurrences", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticReturnExceptionsProperty = new TypedPropertyDefinition<bool>("ExpandEvent.ReturnExceptions", false, true);

		private static readonly TypedPropertyDefinition<bool> StaticReturnCancellationsProperty = new TypedPropertyDefinition<bool>("ExpandEvent.ReturnCancellations", false, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticWindowStartProperty = new TypedPropertyDefinition<ExDateTime>("ExpandEvent.WindowStart", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticWindowEndProperty = new TypedPropertyDefinition<ExDateTime>("ExpandEvent.WindowEnd", default(ExDateTime), true);
	}
}

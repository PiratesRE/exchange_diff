using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StoreObjectValidationError : ValidationError
	{
		internal StoreObjectValidationError(ValidationContext context, PropertyDefinition propertyDefinition, object invalidData, StoreObjectConstraint constraint) : base(new LocalizedString(ServerStrings.ExStoreObjectValidationError))
		{
			this.propertyDefinition = propertyDefinition;
			this.invalidData = invalidData;
			this.constraint = constraint;
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		public object InvalidData
		{
			get
			{
				return this.invalidData;
			}
		}

		public StoreObjectConstraint Constraint
		{
			get
			{
				return this.constraint;
			}
		}

		public override string ToString()
		{
			return string.Format("Object Violation. Invalid property = {0}. Invalid data for that property = {1}. Constraint violated = {2}.", this.propertyDefinition, this.invalidData, this.constraint);
		}

		private readonly PropertyDefinition propertyDefinition;

		private readonly object invalidData;

		private readonly StoreObjectConstraint constraint;
	}
}

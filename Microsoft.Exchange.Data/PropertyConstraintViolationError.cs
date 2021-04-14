using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class PropertyConstraintViolationError : PropertyValidationError
	{
		public PropertyConstraintViolationError(LocalizedString description, PropertyDefinition propertyDefinition, object invalidData, PropertyDefinitionConstraint constraint) : base(description, propertyDefinition, invalidData)
		{
			if (constraint == null)
			{
				throw new ArgumentNullException("constraint");
			}
			this.constraint = constraint;
		}

		public PropertyDefinitionConstraint Constraint
		{
			get
			{
				return this.constraint;
			}
		}

		public bool Equals(PropertyConstraintViolationError other)
		{
			return other != null && object.Equals(this.Constraint, other.Constraint) && base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PropertyConstraintViolationError);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.Constraint.GetHashCode();
		}

		private PropertyDefinitionConstraint constraint;
	}
}

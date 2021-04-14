using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class PropertyConstraintValidatorMapping : ITypeMapping
	{
		public PropertyConstraintValidatorMapping(Type sourceType, Type validatorType) : this(sourceType, new PropertyConstraintValidatorCreator(sourceType, validatorType))
		{
		}

		public PropertyConstraintValidatorMapping(Type sourceType, IPropertyConstraintValidatorCreator validatorCreator)
		{
			if (sourceType == null)
			{
				throw new ArgumentNullException("sourceType");
			}
			if (!typeof(PropertyDefinitionConstraint).IsAssignableFrom(sourceType))
			{
				throw new ArgumentException("sourceType must be subtype of PropertyDefinitionConstraint", "sourceType");
			}
			if (validatorCreator == null)
			{
				throw new ArgumentNullException("validatorCreator");
			}
			this.SourceType = sourceType;
			this.ValidatorCreator = validatorCreator;
		}

		public Type SourceType { get; private set; }

		public IPropertyConstraintValidatorCreator ValidatorCreator { get; private set; }
	}
}

using System;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class PropertyConstraintValidatorCreator : IPropertyConstraintValidatorCreator
	{
		public PropertyConstraintValidatorCreator(Type constraintType, Type validatorType)
		{
			if (constraintType == null)
			{
				throw new ArgumentNullException();
			}
			if (!typeof(PropertyDefinitionConstraint).IsAssignableFrom(constraintType))
			{
				throw new ArgumentException();
			}
			if (validatorType == null)
			{
				throw new ArgumentNullException();
			}
			if (!typeof(ValidatorInfo).IsAssignableFrom(validatorType))
			{
				throw new ArgumentException();
			}
			this.ConstraintType = constraintType;
			this.ValidatorType = validatorType;
		}

		public virtual ValidatorInfo Create(ProviderPropertyDefinition propertyDefinition, PropertyDefinitionConstraint constraint)
		{
			if (this.ValidatorType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
			{
				this.ConstraintType
			}, null) != null)
			{
				return (ValidatorInfo)Activator.CreateInstance(this.ValidatorType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[]
				{
					constraint
				}, null);
			}
			if (this.ValidatorType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null) != null)
			{
				return (ValidatorInfo)Activator.CreateInstance(this.ValidatorType, true);
			}
			return null;
		}

		public Type ConstraintType { get; private set; }

		public Type ValidatorType { get; private set; }
	}
}

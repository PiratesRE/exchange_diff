using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class TypeMappingPropertyConstraintValidatorsGenerator
	{
		public TypeMappingPropertyConstraintValidatorsGenerator()
		{
			this.TypeMappingManager = new TypeMappingManager<PropertyConstraintValidatorMapping>();
		}

		public TypeMappingManager<PropertyConstraintValidatorMapping> TypeMappingManager { get; private set; }

		public void RegisterMapping(Type sourceType, Type validatorType)
		{
			this.RegisterMapping(new PropertyConstraintValidatorMapping(sourceType, validatorType));
		}

		public void RegisterMapping(Type sourceType, IPropertyConstraintValidatorCreator validatorCreator)
		{
			this.RegisterMapping(new PropertyConstraintValidatorMapping(sourceType, validatorCreator));
		}

		public void RegisterMapping(PropertyConstraintValidatorMapping mapping)
		{
			this.TypeMappingManager.RegisterMapping(mapping);
		}

		public IPropertyConstraintValidatorCreator[] GetValidatorCreators(Type sourceType)
		{
			List<IPropertyConstraintValidatorCreator> list = new List<IPropertyConstraintValidatorCreator>();
			PropertyConstraintValidatorMapping[] nearestMappings = this.TypeMappingManager.GetNearestMappings(sourceType);
			foreach (PropertyConstraintValidatorMapping propertyConstraintValidatorMapping in nearestMappings)
			{
				list.Add(propertyConstraintValidatorMapping.ValidatorCreator);
			}
			return list.ToArray();
		}

		public ValidatorInfo[] ValidatorsFromPropertyDefinition(ProviderPropertyDefinition propertyDefinition)
		{
			if (propertyDefinition != null)
			{
				List<ValidatorInfo> list = new List<ValidatorInfo>();
				foreach (PropertyDefinitionConstraint propertyDefinitionConstraint in propertyDefinition.AllConstraints)
				{
					IPropertyConstraintValidatorCreator[] validatorCreators = this.GetValidatorCreators(propertyDefinitionConstraint.GetType());
					IPropertyConstraintValidatorCreator[] array = validatorCreators;
					for (int i = 0; i < array.Length; i++)
					{
						IPropertyConstraintValidatorCreator propertyConstraintValidatorCreator = array[i];
						ValidatorInfo validator = propertyConstraintValidatorCreator.Create(propertyDefinition, propertyDefinitionConstraint);
						if (validator != null)
						{
							if (!list.Any((ValidatorInfo c) => c.Type == validator.Type))
							{
								if (propertyDefinition.IsMultivalued && !(propertyDefinitionConstraint is CollectionPropertyDefinitionConstraint))
								{
									list.Add(new CollectionItemValidatorInfo
									{
										ItemValidator = validator
									});
								}
								else
								{
									list.Add(validator);
								}
							}
						}
					}
				}
				return list.ToArray();
			}
			return new ValidatorInfo[0];
		}
	}
}

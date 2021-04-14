using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal interface IPropertyConstraintValidatorCreator
	{
		ValidatorInfo Create(ProviderPropertyDefinition propertyDefinition, PropertyDefinitionConstraint constraint);
	}
}

using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal interface IPropertyConstraintProvider
	{
		PropertyDefinitionConstraint[] GetPropertyDefinitionConstraints(string propertyName);
	}
}

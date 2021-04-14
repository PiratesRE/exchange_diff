using System;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NoInheritedACEsConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			RawSecurityDescriptor rawSecurityDescriptor = (RawSecurityDescriptor)value;
			if (rawSecurityDescriptor == null)
			{
				return null;
			}
			foreach (GenericAce genericAce in rawSecurityDescriptor.DiscretionaryAcl)
			{
				if ((byte)(genericAce.AceFlags & AceFlags.Inherited) == 16)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationSecurityDescriptorContainsInheritedACEs(rawSecurityDescriptor.GetSddlForm(AccessControlSections.All)), propertyDefinition, value, this);
				}
			}
			return null;
		}
	}
}

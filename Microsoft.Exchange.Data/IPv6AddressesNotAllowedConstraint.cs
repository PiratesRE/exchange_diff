using System;
using System.Net.Sockets;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class IPv6AddressesNotAllowedConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			IPRange iprange = value as IPRange;
			if (iprange != null && (iprange.LowerBound.AddressFamily == AddressFamily.InterNetworkV6 || iprange.UpperBound.AddressFamily == AddressFamily.InterNetworkV6))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationIpV6NotAllowed(iprange.ToString()), propertyDefinition, value, this);
			}
			return null;
		}
	}
}

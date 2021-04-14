using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class IPRangeConstraint : PropertyDefinitionConstraint
	{
		public IPRangeConstraint(ulong maxIPRange = 256UL)
		{
			this.maxIPRange = maxIPRange;
		}

		public ulong MaxIpRange
		{
			get
			{
				return this.maxIPRange;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			IPvxAddress pvxAddress = new IPvxAddress(0UL, this.maxIPRange);
			IPRange iprange = value as IPRange;
			if (iprange != null && pvxAddress.CompareTo(iprange.Size) < 0)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationIpRangeNotAllowed(iprange.ToString(), this.maxIPRange), propertyDefinition, value, this);
			}
			return null;
		}

		private const ulong DefaultMaxIPRange = 256UL;

		private readonly ulong maxIPRange;
	}
}

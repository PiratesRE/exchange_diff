using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RangeNumberValidatorInfo : RangeValidatorInfo<double>
	{
		public RangeNumberValidatorInfo(double minValue, double maxValue, bool acceptNull, bool acceptUnlimited) : base("RangeNumberValidator", minValue, maxValue, acceptNull, acceptUnlimited)
		{
		}

		internal RangeNumberValidatorInfo(RangedValueConstraint<int> constraint) : this((double)constraint.MinimumValue, (double)constraint.MaximumValue, RangeValidatorInfo<double>.IsNullable<int>(constraint), RangeValidatorInfo<double>.IsUnlimited<int>(constraint))
		{
		}

		internal RangeNumberValidatorInfo(RangedValueConstraint<uint> constraint) : this(constraint.MinimumValue, constraint.MaximumValue, RangeValidatorInfo<double>.IsNullable<uint>(constraint), RangeValidatorInfo<double>.IsUnlimited<uint>(constraint))
		{
		}
	}
}

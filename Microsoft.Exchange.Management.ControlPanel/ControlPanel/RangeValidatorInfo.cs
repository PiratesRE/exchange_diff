using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract(Name = "RangeValidatorInfo{0}")]
	public abstract class RangeValidatorInfo<TClientValue> : ValidatorInfo
	{
		protected RangeValidatorInfo(string validatorType, TClientValue minValue, TClientValue maxValue, bool acceptNull, bool acceptUnlimited) : base(validatorType)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.AcceptNull = acceptNull;
			this.AcceptUnlimted = acceptUnlimited;
		}

		[DataMember]
		public TClientValue MinValue { get; set; }

		[DataMember]
		public TClientValue MaxValue { get; set; }

		[DataMember]
		public bool AcceptNull { get; set; }

		[DataMember]
		public bool AcceptUnlimted { get; set; }

		internal static bool IsNullable<TServerValue>(RangedValueConstraint<TServerValue> constraint) where TServerValue : struct, IComparable
		{
			return constraint is RangedNullableValueConstraint<TServerValue> || constraint is RangedNullableUnlimitedConstraint<TServerValue>;
		}

		internal static bool IsUnlimited<TServerValue>(RangedValueConstraint<TServerValue> constraint) where TServerValue : struct, IComparable
		{
			return constraint is RangedUnlimitedConstraint<TServerValue> || constraint is RangedNullableUnlimitedConstraint<TServerValue>;
		}
	}
}

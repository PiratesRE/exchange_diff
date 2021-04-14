using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class StringLengthValidatorInfo : ValidatorInfo
	{
		public StringLengthValidatorInfo(int minLength, int maxLength) : this("StringLengthValidator", minLength, maxLength)
		{
		}

		internal StringLengthValidatorInfo(StringLengthConstraint constraint) : this(constraint.MinLength, constraint.MaxLength)
		{
		}

		internal StringLengthValidatorInfo(UIImpactStringLengthConstraint constraint) : this(constraint.MinLength, constraint.MaxLength)
		{
		}

		internal StringLengthValidatorInfo(LocalLongFullPathLengthConstraint constraint) : this(0, 247)
		{
		}

		protected StringLengthValidatorInfo(string validatorType, int minLength, int maxLength) : base(validatorType)
		{
			this.MinLength = minLength;
			this.MaxLength = maxLength;
		}

		[DataMember]
		public int MaxLength { get; set; }

		[DataMember]
		public int MinLength { get; set; }
	}
}

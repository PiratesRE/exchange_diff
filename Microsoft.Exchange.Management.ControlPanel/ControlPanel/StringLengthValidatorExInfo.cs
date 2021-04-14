using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class StringLengthValidatorExInfo : ValidatorInfo
	{
		public StringLengthValidatorExInfo(string binderControlForMinLength, string binderControlForMaxLength) : this("StringLengthValidatorEx", binderControlForMinLength, binderControlForMaxLength)
		{
		}

		protected StringLengthValidatorExInfo(string validatorType, string binderControlForMinLength, string binderControlForMaxLength) : base(validatorType)
		{
			this.BinderControlForMinLength = binderControlForMinLength;
			this.BinderControlForMaxLength = binderControlForMaxLength;
		}

		public string BinderControlForMinLength { get; set; }

		public string BinderControlForMaxLength { get; set; }
	}
}

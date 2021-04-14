using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class CompareValidatorInfo : ValidatorInfo
	{
		protected CompareValidatorInfo(string validatorType, string controlToCompare) : base(validatorType)
		{
			this.ControlToCompare = controlToCompare;
		}

		[DataMember]
		public string ControlToCompare { get; set; }
	}
}

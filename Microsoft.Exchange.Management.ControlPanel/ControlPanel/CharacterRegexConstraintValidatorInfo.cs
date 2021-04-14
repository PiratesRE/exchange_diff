using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CharacterRegexConstraintValidatorInfo : ValidatorInfo
	{
		internal CharacterRegexConstraintValidatorInfo(CharacterRegexConstraint constraint) : base("CharacterRegexConstraintValidator")
		{
			this.Pattern = constraint.Pattern;
		}

		[DataMember]
		public string Pattern { get; set; }
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CharactersConstraintValidatorInfo : ValidatorInfo
	{
		internal CharactersConstraintValidatorInfo(CharacterConstraint constraint) : base("CharactersConstraintValidator")
		{
			this.DisplayCharacters = ValidatorHelper.ToVisibleString(constraint.Characters);
			this.Characters = new string(constraint.Characters);
			this.ShowAsValid = constraint.ShowAsValid;
		}

		[DataMember]
		public string Characters { get; set; }

		[DataMember]
		public string DisplayCharacters { get; set; }

		[DataMember]
		public bool ShowAsValid { get; set; }
	}
}

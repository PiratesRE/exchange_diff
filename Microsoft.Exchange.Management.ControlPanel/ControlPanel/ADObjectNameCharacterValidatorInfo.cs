using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ADObjectNameCharacterValidatorInfo : ValidatorInfo
	{
		internal ADObjectNameCharacterValidatorInfo(ADObjectNameCharacterConstraint constraint) : base("ADObjectNameCharacterValidator")
		{
			this.DisplayCharacters = ValidatorHelper.ToVisibleString(constraint.Characters);
			this.InvalidCharacters = new string(constraint.Characters);
		}

		[DataMember]
		public string DisplayCharacters { get; set; }

		[DataMember]
		public string InvalidCharacters { get; set; }
	}
}

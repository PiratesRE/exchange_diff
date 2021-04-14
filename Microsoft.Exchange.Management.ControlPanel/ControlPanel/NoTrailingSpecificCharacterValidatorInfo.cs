using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NoTrailingSpecificCharacterValidatorInfo : ValidatorInfo
	{
		internal NoTrailingSpecificCharacterValidatorInfo(NoTrailingSpecificCharacterConstraint constraint) : base("NoTrailingSpecificCharacterValidator")
		{
			this.InvalidChar = constraint.InvalidChar;
		}

		[DataMember]
		public char InvalidChar { get; set; }
	}
}

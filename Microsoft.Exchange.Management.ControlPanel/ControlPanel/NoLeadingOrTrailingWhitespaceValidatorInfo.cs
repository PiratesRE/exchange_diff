using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NoLeadingOrTrailingWhitespaceValidatorInfo : ValidatorInfo
	{
		public NoLeadingOrTrailingWhitespaceValidatorInfo() : base("NoLeadingOrTrailingWhitespaceValidator")
		{
		}
	}
}

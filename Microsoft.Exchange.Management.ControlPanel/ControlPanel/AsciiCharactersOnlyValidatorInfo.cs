using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AsciiCharactersOnlyValidatorInfo : ValidatorInfo
	{
		public AsciiCharactersOnlyValidatorInfo() : base("AsciiCharactersOnlyValidator")
		{
		}
	}
}

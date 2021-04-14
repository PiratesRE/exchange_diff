using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ComputerNameCharacterValidatorInfo : ValidatorInfo
	{
		internal ComputerNameCharacterValidatorInfo(ComputerNameCharacterConstraint constraint) : base("ComputerNameCharacterValidator")
		{
		}
	}
}

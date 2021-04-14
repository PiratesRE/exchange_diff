using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ADObjectNameStringLengthValidatorInfo : StringLengthValidatorInfo
	{
		internal ADObjectNameStringLengthValidatorInfo(ADObjectNameStringLengthConstraint constraint) : base("ADObjectNameStringLengthValidator", constraint.MinLength, constraint.MaxLength)
		{
		}
	}
}

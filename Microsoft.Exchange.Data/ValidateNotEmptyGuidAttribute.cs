using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	internal class ValidateNotEmptyGuidAttribute : ValidateArgumentsAttribute
	{
		protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
		{
			if (arguments == null || Guid.Empty.Equals(arguments))
			{
				throw new ArgumentException(DataStrings.CmdletParameterEmptyValidationException);
			}
		}
	}
}

using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal sealed class AsciiCharactersOnlyConstraint : CharacterRegexConstraint
	{
		public AsciiCharactersOnlyConstraint() : base(AsciiCharactersOnlyConstraint.AsciiCharactersOnlyRegEx)
		{
		}

		protected override LocalizedString CustomErrorMessage(string value, PropertyDefinition propertyDefinition)
		{
			return DataStrings.ConstraintViolationStringDoesContainsNonASCIICharacter(value);
		}

		public static string AsciiCharactersOnlyRegEx = "[^\u0080-￿]";
	}
}

using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class ValidatorHelper
	{
		static ValidatorHelper()
		{
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(NotNullOrEmptyConstraint), typeof(NotNullOrEmptyValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(NotNullOrEmptyStrictConstraint), typeof(NotNullOrEmptyValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(CharacterRegexConstraint), typeof(CharacterRegexConstraintValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(CharacterConstraint), typeof(CharactersConstraintValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(ADObjectNameCharacterConstraint), typeof(ADObjectNameCharacterValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(AsciiCharactersOnlyConstraint), typeof(AsciiCharactersOnlyValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(ComputerNameCharacterConstraint), typeof(ComputerNameCharacterValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(ContainingNonWhitespaceConstraint), typeof(ContainingNonWhitespaceValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(NoLeadingOrTrailingWhitespaceConstraint), typeof(NoLeadingOrTrailingWhitespaceValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(NoSurroundingWhiteSpaceConstraint), typeof(NoLeadingOrTrailingWhitespaceValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(NoTrailingSpecificCharacterConstraint), typeof(NoTrailingSpecificCharacterValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(StringLengthConstraint), typeof(StringLengthValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(UIImpactStringLengthConstraint), typeof(StringLengthValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(MandatoryStringLengthConstraint), typeof(StringLengthValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(ADObjectNameStringLengthConstraint), typeof(ADObjectNameStringLengthValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(RegexConstraint), typeof(RegexValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(UriKindConstraint), typeof(UriKindValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(RangedValueConstraint<int>), typeof(RangeNumberValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(RangedValueConstraint<uint>), typeof(RangeNumberValidatorInfo));
			ValidatorHelper.propertyConstraintValidatorsGenerator.RegisterMapping(typeof(LocalLongFullPathLengthConstraint), typeof(StringLengthValidatorInfo));
		}

		public static Dictionary<string, ValidatorInfo[]> GenerateValidators(params ProviderPropertyDefinition[] propertyDefinitions)
		{
			Dictionary<string, ValidatorInfo[]> dictionary = new Dictionary<string, ValidatorInfo[]>(propertyDefinitions.Length);
			foreach (ProviderPropertyDefinition providerPropertyDefinition in propertyDefinitions)
			{
				ValidatorInfo[] array = ValidatorHelper.ValidatorsFromPropertyDefinition(providerPropertyDefinition);
				if (!array.IsNullOrEmpty())
				{
					dictionary.Add(providerPropertyDefinition.Name, array);
				}
			}
			return dictionary;
		}

		public static ValidatorInfo[] ValidatorsFromPropertyDefinition(ProviderPropertyDefinition propertyDefinition)
		{
			List<ValidatorInfo> list = new List<ValidatorInfo>();
			if (propertyDefinition != null)
			{
				if (propertyDefinition.IsMandatory)
				{
					list.Add(new RequiredFieldValidatorInfo());
				}
				list.AddRange(ValidatorHelper.propertyConstraintValidatorsGenerator.ValidatorsFromPropertyDefinition(propertyDefinition));
			}
			return list.ToArray();
		}

		internal static string ToVisibleString(char[] chars)
		{
			if (chars == null || chars.Length == 0)
			{
				return null;
			}
			List<string> list = new List<string>(chars.Length);
			foreach (char c in chars)
			{
				list.Add(char.IsControl(c) ? string.Format("'0x{0:X2}'", (int)c) : string.Format("'{0}'", c));
			}
			return string.Join(",", list);
		}

		private static TypeMappingPropertyConstraintValidatorsGenerator propertyConstraintValidatorsGenerator = new TypeMappingPropertyConstraintValidatorsGenerator();
	}
}

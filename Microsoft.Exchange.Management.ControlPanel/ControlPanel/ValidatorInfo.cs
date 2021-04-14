using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(ADObjectNameCharacterValidatorInfo))]
	[DataContract]
	[KnownType(typeof(RequiredFieldValidatorInfo))]
	[KnownType(typeof(NotNullOrEmptyValidatorInfo))]
	[KnownType(typeof(AsciiCharactersOnlyValidatorInfo))]
	[KnownType(typeof(CharacterRegexConstraintValidatorInfo))]
	[KnownType(typeof(CharactersConstraintValidatorInfo))]
	[KnownType(typeof(ComputerNameCharacterValidatorInfo))]
	[KnownType(typeof(ContainingNonWhitespaceValidatorInfo))]
	[KnownType(typeof(NoLeadingOrTrailingWhitespaceValidatorInfo))]
	[KnownType(typeof(NoTrailingSpecificCharacterValidatorInfo))]
	[KnownType(typeof(StringLengthValidatorInfo))]
	[KnownType(typeof(ADObjectNameStringLengthValidatorInfo))]
	[KnownType(typeof(RegexValidatorInfo))]
	[KnownType(typeof(UriKindValidatorInfo))]
	[KnownType(typeof(RangeNumberValidatorInfo))]
	[KnownType(typeof(CompareStringValidatorInfo))]
	[KnownType(typeof(CollectionValidatorInfo))]
	[KnownType(typeof(CollectionItemValidatorInfo))]
	public class ValidatorInfo
	{
		public ValidatorInfo(string typeName)
		{
			this.Type = typeName;
		}

		[DataMember]
		public string Type { get; private set; }
	}
}

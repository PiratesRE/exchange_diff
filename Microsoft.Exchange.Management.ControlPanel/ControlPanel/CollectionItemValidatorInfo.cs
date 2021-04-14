using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(UriKindValidatorInfo))]
	[KnownType(typeof(NoLeadingOrTrailingWhitespaceValidatorInfo))]
	[KnownType(typeof(NotNullOrEmptyValidatorInfo))]
	[KnownType(typeof(CharacterRegexConstraintValidatorInfo))]
	[KnownType(typeof(AsciiCharactersOnlyValidatorInfo))]
	[KnownType(typeof(CharactersConstraintValidatorInfo))]
	[KnownType(typeof(ADObjectNameCharacterValidatorInfo))]
	[KnownType(typeof(ComputerNameCharacterValidatorInfo))]
	[KnownType(typeof(ContainingNonWhitespaceValidatorInfo))]
	[KnownType(typeof(ADObjectNameStringLengthValidatorInfo))]
	[KnownType(typeof(NoTrailingSpecificCharacterValidatorInfo))]
	[KnownType(typeof(StringLengthValidatorInfo))]
	[KnownType(typeof(StringLengthValidatorExInfo))]
	[KnownType(typeof(RangeNumberValidatorInfo))]
	[KnownType(typeof(RegexValidatorInfo))]
	[KnownType(typeof(CompareStringValidatorInfo))]
	[DataContract]
	public class CollectionItemValidatorInfo : CollectionValidatorInfo
	{
		public CollectionItemValidatorInfo() : base("CollectionItemValidator")
		{
		}

		[DataMember]
		public ValidatorInfo ItemValidator { get; set; }
	}
}

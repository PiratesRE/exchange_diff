using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal enum RunKind : uint
	{
		Invalid,
		Text = 67108864U,
		StartLexicalUnitFlag = 2147483648U,
		MajorKindMask = 2080374784U,
		MajorKindMaskWithStartLexicalUnitFlag = 4227858432U,
		MinorKindMask = 50331648U,
		KindMask = 4278190080U
	}
}

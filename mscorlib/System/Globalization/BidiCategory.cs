using System;

namespace System.Globalization
{
	[Serializable]
	internal enum BidiCategory
	{
		LeftToRight,
		LeftToRightEmbedding,
		LeftToRightOverride,
		RightToLeft,
		RightToLeftArabic,
		RightToLeftEmbedding,
		RightToLeftOverride,
		PopDirectionalFormat,
		EuropeanNumber,
		EuropeanNumberSeparator,
		EuropeanNumberTerminator,
		ArabicNumber,
		CommonNumberSeparator,
		NonSpacingMark,
		BoundaryNeutral,
		ParagraphSeparator,
		SegmentSeparator,
		Whitespace,
		OtherNeutrals,
		LeftToRightIsolate,
		RightToLeftIsolate,
		FirstStrongIsolate,
		PopDirectionIsolate
	}
}
